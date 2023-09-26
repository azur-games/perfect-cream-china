using AbTest;
using DG.Tweening;
using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using MoreMountains.NiceVibrations;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameplayController : MonoBehaviour
{
    #region Nested types

    public enum ValveState
    {
        None,
        FinishingTimelaps,
        Off,
        Manual,
    }

    public enum GameplayState
    {
        Process,
        CameraFinishMoving,
        WaitingForValve,
        FinishingSupplying,
        PreFinishingAnimation,
        FinishingAnimation,
    }

    #endregion


    #region Fields

    public Conveyor Conveyour;
    public Valve Valve;
    public SceneVisual SceneVisual;
    private ColorScheme visualColorScheme;

    public Camera Camera;
    [SerializeField] private CamShake _camShake;
    public Transform spawnPoint;

    private static bool isGameplayActive = false;
    private static bool previousGameplayActiveValue = false;

    public static int lastLevel;
    public static int attempt;

    public event Action OnReadyToDecorate;
    public event Action OnReadyToFinalize;
    public event Action OnFinalized;
    public event Action OnCreamLost;
    public PerfectsController PerfectsController { get; } = new PerfectsController();

    public event Action<MetagameRoomContext> CompleteLevel;
    private Shape _currentShape;
    private LevelAsset _currentLevel;
    private int _raycastMask;
    private float _breakTime = 1f;
    private bool _isStarted = false;
    private bool _isResultCalculation = false;
    private float _penaltyTimer = 0;
    private float _currentLevelStartTime;
    private ValveState currentValveState = ValveState.Manual;
    private int obstaclesCollisionCounter = 0;
    private float speedScale = 1.0f;
    [SerializeField] private GameObject speedParticles;
    [SerializeField] private Transform keyShineEffectRoot;
    [SerializeField] private Transform coinShineEffectRoot;

    private float finishingTimer = 0.0f;
    private Vector3 finishingCameraOffset = Vector3.zero;
    private CreamSkinAsset finishingSkin;
    private FinalJar finalJar;

    [SerializeField] SpeedFXController speedFXController;

    [Header("Key effect settings")]
    [SerializeField] Transform keyConfettiRoot;
    [SerializeField] ParticleSystem keyConfettiEffect;

    public FeverModeHelper FeverModeHelper { get; private set; }
    public bool IsTubeInFeverZone { get; private set; } = false;
    private Guid? feverModeSoundGuid = null;
    #endregion



    #region Properties

    public static bool IsGameplayActive
    {
        get
        {
            return isGameplayActive;
        }
        set
        {
            previousGameplayActiveValue = isGameplayActive;
            isGameplayActive = value;
        }
    }

    public GameplayState State { get; private set; } = GameplayState.Process;
    
    public float ProgressWithOffset { get { return Score.ProgressWithOffset; } }

    public Score Score { get; private set; }

    public LevelAsset CurrentLevelAsset { get { return _currentLevel; } }

    public int CurrentLevelBucksReward
    {
        get
        {
            int result = 0;

            if (_currentLevel != null & _currentLevel.OverrideCoinsReward >= 0)
            {
                result = _currentLevel.OverrideCoinsReward;
            }
            else
            {
                GameplayRoomConfig gameplayRoomConfig = Env.Instance.Rooms.GameplayRoom.Config as GameplayRoomConfig;
                result = gameplayRoomConfig.BaseLevelCoinsReward;
            }

            return result;
        }
    }

    public bool IsExtraLevel
    {
        get
        {
            return Env.Instance.Rooms.GameplayRoom.Context.IsExtraLevel;
        }
    }


    public bool IsFreeSingleChestReceived => PlayerPrefs.HasKey("ShowBubbleIconFirstTime");

    #endregion



    #region Unity lifecycle

    private void Awake()
    {
        Env.Instance.Rooms.GameplayRoom.OnGameplayLoaded(this);

        keyShineEffectRoot.gameObject.SetActive(false);
        coinShineEffectRoot.gameObject.SetActive(false);
        
        AdvertisingManager.Instance.OnAdShow += AdvertisingManager_OnAdShow;
        AdvertisingManager.Instance.OnAdHide += AdvertisingManager_OnAdHide;
        //AdvertisingManager.Instance.On += AdvertisingManager_OnAdHide;
        LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;
    }


    private void OnDestroy()
    {
        AdvertisingManager.Instance.OnAdShow -= AdvertisingManager_OnAdShow;
        AdvertisingManager.Instance.OnAdHide -= AdvertisingManager_OnAdHide;

        LLApplicationStateRegister.OnApplicationEnteredBackground -= LLApplicationStateRegister_OnApplicationEnteredBackground;
    }

    #endregion

    
    #region Public methods

    public void CreateScene(LevelAsset levelAsset)
    {
        _raycastMask = LayerMask.GetMask("Gameplay");

        _currentLevel = PrefabTools.Instantiate(spawnPoint, levelAsset);

        FeverModeHelper = new FeverModeHelper();
        
        var timeToFinish = BalanceDataProvider.Instance.GetTimeToFinish(_currentLevel);
    
        speedScale = BalanceDataProvider.Instance.GameplaySpeed;
        speedParticles.gameObject.SetActive(speedScale > 1.0001f);

        if (!Env.Instance.Rules.Effects.Value)
        {
            speedParticles.SetActive(false);
        }

        float averageChunksCount = Mathf.Max(1.0f, speedScale * timeToFinish * 0.33333f - 1.0f); // 1 chunk per 3 seconds

        Conveyour.Init(_currentLevel.Generator, 
                       Valve.GetCreamCreator(), 
                       averageChunksCount, 
                       Conveyour_OnChunkFinished, 
                       Conveyour_OnKeyTouched, 
                       Conveyour_OnCoinTouched);

        Valve.Init(Conveyour, levelAsset.ValveAssetOverrideName, CreamCollisionHandler);

        IsGameplayActive = true;

        if ((null != _currentLevel.ColorSchemes) && (_currentLevel.ColorSchemes.Count > 0))
            SceneVisual.ApplyRandomColorScheme(_currentLevel.ColorSchemes);
        else
            SceneVisual.ApplyNextColorSchemeWithPeriod();
        visualColorScheme = SceneVisual.CurrentScheme;

        if (!string.IsNullOrEmpty(_currentLevel.CreamSkinName))
        {
            Valve.GetCreamCreator().SetCreamSkinWithoutSaving(_currentLevel.CreamSkinName);
        }
        int scoresPerSec = Env.Instance.Rules.FastGame.Value ? Env.Instance.Rules.ScorePerTickFastGame.Value : Env.Instance.Rules.ScorePerTickNormalGame.Value;
        Score = new Score(timeToFinish, scoresPerSec, speedScale);
        this.gameObject.GetComponentInChildren<SupplyAnimation>().StopFailAnimation();

        State = GameplayState.Process;
        
        finishingSkin = Env.Instance.Content.LoadRandomContentItem(ContentAsset.AssetType.CreamSkin, true, Env.Instance.Rules.CreamSkin.Value).Asset as CreamSkinAsset;
        ValveAsset finishingValve = Env.Instance.Content.LoadRandomContentItem(ContentAsset.AssetType.Valve, true, Env.Instance.Inventory.ValveName, "pony", "cat").Asset as ValveAsset;
        Valve.SetUnderstudySkin(finishingValve);

        if (null != CurrentLevelAsset.FinalJar)
        {
            finalJar = GameObject.Instantiate(CurrentLevelAsset.FinalJar);
            finalJar.gameObject.SetActive(false);
        }

        if (!PopupManager.Instance.IsSubscriptionPopupActive)
            Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicGameplay);
    }

    private void Conveyour_OnChunkFinished(ShapesChunk chunk, float filling)
    {
        if (!_isStarted) return;

        if (!_isResultCalculation)
        {
            _isResultCalculation = true;
            return;
        }

        bool fillingSuccessfully = PerfectsController.AddResult(filling);

        if (!IsTubeInFeverZone)
        {
            if (fillingSuccessfully)
                speedFXController.TryToIncrease();
            else
                speedFXController.TryToDecrease();
        }

        if (chunk.IsFeverChunk)
        {
            FeverModeHelper.OnChunkFinished(chunk, filling, out string shapeCaption);
            if (Env.Instance.Rules.FeverNumbers.Value)
            {
                chunk.GetFirstShape().ShowText(shapeCaption);
            }

            if (!string.IsNullOrEmpty(shapeCaption))
                Conveyour.AddShine(chunk);
        }

        if (chunk.isLastFeverChunk)
        {
            IsTubeInFeverZone = false;
            speedFXController.SetNormalColor();
            Conveyour.SetNormalColor();
            SceneVisual.ApplyColorScheme(visualColorScheme);

            FeverModeHelper.Finish(out int prizeCoinsCount, out bool isPerfect);

            Env.Instance.Inventory.CurrentLevelFeverCompletions++;

            Env.Instance.Inventory.IncreaseBucksBoxValue(prizeCoinsCount * 2);
            Env.Instance.Rooms.GameplayRoom.UI.OnFeverModeFinished(prizeCoinsCount, isPerfect);

            Env.Instance.Sound.StopSound(feverModeSoundGuid);
            feverModeSoundGuid = null;
            if (prizeCoinsCount > 0)
                Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.FeverMode_off);
        }
    }

    private void OnFeverModeActuallyStarted(Collider collider)
    {
        if (IsTubeInFeverZone) return;
        IsTubeInFeverZone = true;

        RibbonController ribbonController = collider.GetComponentInParent<RibbonController>();
        ribbonController?.StartRibbonAnimation();

        speedFXController.SetMax();
        speedFXController.SetFeverColor();
        Conveyour.SetFeverColor();

        SceneVisual.ApplyColorScheme(SceneVisual.FeverModeScheme);
        Env.Instance.Rooms.GameplayRoom.UI.OnFeverModeStarts();

        feverModeSoundGuid = Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.FeverMode_intrigue);
    }

    public void StopSound()
    {
        Env.Instance.Sound.StopSound(feverModeSoundGuid);
        feverModeSoundGuid = null;
    }

    private void Conveyour_OnKeyTouched(GameObject key)
    {
        if (OptionsPanel.IsVibroEnabled)
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);

        Env.Instance.Inventory.CurrentLevelPickedUpKeys++;

        Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

        if (Env.Instance.Rules.Effects.Value)
        {
            keyConfettiRoot.position = key.transform.position;
            keyConfettiEffect.Clear();
            keyConfettiEffect.Play();
        }

        key.transform.SetParent(transform);

        if (Env.Instance.Rules.Effects.Value)
        {
            keyShineEffectRoot.gameObject.SetActive(true);
            keyShineEffectRoot.transform.SetParent(key.transform);
            keyShineEffectRoot.transform.localPosition = Camera.transform.forward;
            keyShineEffectRoot.transform.localRotation = Quaternion.LookRotation(Camera.transform.forward);
        }

        DOTween.Sequence().AppendCallback(() =>
        {
            Vector3 forwardVec = (key.transform.position - Camera.transform.position).normalized;
            keyShineEffectRoot.transform.localPosition = forwardVec;
            keyShineEffectRoot.transform.localRotation = Quaternion.LookRotation(forwardVec);
        }).SetLoops(-1, LoopType.Restart).SetTarget(key).SetAutoKill(true); // fast solution for updating shine position and rotation to achieve parallax effect

        Env.Instance.Inventory.AddKeys(1, key.transform, () =>
        {
            DOTween.Kill(key);

            keyShineEffectRoot.gameObject.SetActive(false);
            keyShineEffectRoot.transform.SetParent(transform);
            keyShineEffectRoot.transform.localPosition = Vector3.zero;

            Destroy(key);
        });
    }

    private void Conveyour_OnCoinTouched(GameObject coin)
    {
        if (OptionsPanel.IsVibroEnabled)
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);

        Env.Instance.Inventory.CurrentLevelPickedUpCoins++;

        Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

        if (Env.Instance.Rules.Effects.Value)
        {
            keyConfettiRoot.position = coin.transform.position;
            keyConfettiEffect.Clear();
            keyConfettiEffect.Play();
        }

        Destroy(coin);

        Env.Instance.Inventory.IncreaseBucksBoxValue( BalanceDataProvider.Instance.CoinsBoxExchange);
    }

    public void StartGame()
    {
        _isStarted = true;

        Env.Instance.PassLevel.lastLevelIndex = Env.Instance.Inventory.CurrentLevelIndex;

        TrackLevelStart();

        Conveyour.Go();
    }

    #endregion



    #region Private methods
    public bool UserTouchGameplay { get; set; } = false;
    public Vector3 addoffset = Vector3.zero;
    private void Update()
    {
        if (!_isStarted)
        {
            Conveyour.UpdateSelf(speedScale);
            return;
        }

        if (!IsGameplayActive)
            return;

        if (!Input.GetMouseButton(0))
            UserTouchGameplay = false;

        bool needValving = false;
        bool impressivePushing = true;
        bool updateTime = false;
        switch (currentValveState)
        {
            case ValveState.FinishingTimelaps:
                _penaltyTimer = 0.0f;
                needValving = true;
                impressivePushing = false;
                updateTime = UserTouchGameplay;

                if (!updateTime)
                {
                    Valve.GetCreamCreator().StopSound();
                }
                else
                {
                    Valve.GetCreamCreator().StartSound();
                }
                break;

            case ValveState.Off:
                needValving = false;
                updateTime = true;
                break;

            case ValveState.Manual:
                needValving = UserTouchGameplay;
                updateTime = true;
                break;
        }

        if ((State == GameplayState.CameraFinishMoving) ||
            (State == GameplayState.WaitingForValve))
        {
            needValving = false;
            updateTime = true;
        }

        if (State == GameplayState.CameraFinishMoving)
        {
            //updateTime = false;
            needValving = false;
            Valve.Supply(false);
        }

        if (_penaltyTimer > 0)
        {
            _penaltyTimer -= Time.deltaTime;
            needValving = false;
        }

        if (updateTime)
        {
            Conveyour.UpdateSelf(speedScale);

            if (!IsTubeInFeverZone)
            {
                Score.Update();
            }
        }

        RaycastTheTape();

        if (updateTime)
        {
            Valve.Supply(needValving, impressivePushing);
        }

        Valve.TurnParticles(updateTime && needValving);

        if (State == GameplayState.CameraFinishMoving)
        {
            finishingTimer += Time.deltaTime;
            float step = Mathf.Clamp01(finishingTimer * 1.1f);
            step = 0.5f * (1.0f - Mathf.Cos(step * Mathf.PI));
            Vector3 newViewPosition = Vector3.Lerp(finishingCameraOffset, Vector3.left * 0.28f, step);
            newViewPosition.y = SceneVisual.transform.position.y;
            SceneVisual.transform.localPosition = newViewPosition;

            if (step >= 0.9999999f)
            {
                State = GameplayState.WaitingForValve;

                if (!IsExtraLevel)
                {
                    Valve.GetCreamCreator().SetCreamSkinWithoutSaving(finishingSkin.Name);
                }
            }
        }

        if ((State == GameplayState.FinishingSupplying) ||
            (State == GameplayState.PreFinishingAnimation))
        {
            SceneVisual.transform.parent.Rotate(Vector3.up, 30.0f * Time.deltaTime);
        }

        if (State == GameplayState.FinishingAnimation)
        {
            MakeFinishingAnimation();
        }

        if (Conveyour.FeverModeEnabled)
        {
            FeverModeHelper.Start(Conveyour.FeverModeChunksCount);
        }

        if (Score.IsComplete && !Conveyour.FeverModeEnabled && !FeverModeHelper.IsActuallyEnabled)
        {
            Conveyour.CreateFinish();
        }
    }

    private void RaycastTheTape()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(Valve.SpawnPos + Vector3.up, Vector3.down), out hit, 5.0f, _raycastMask))
        {
            switch (hit.collider.tag)
            {
                case "feverStarter":
                    OnFeverModeActuallyStarted(hit.collider);
                    break;

                case "obstacle":
                    if (Valve.IsSupplyingInProcess())
                    {
                        obstaclesCollisionCounter++;
                        if (obstaclesCollisionCounter >  BalanceDataProvider.Instance.GetNonFatalHitsNumber(_currentLevel))
                        {
                            // ProcessFatalObstacle();
                            ProcessNoFatalObstacle();
                        }
                        else
                        {
                            ProcessNoFatalObstacle();
                        }
                    }
                    break;

                case "prefinish":
                    if (State == GameplayState.Process)
                    {
                        currentValveState = ValveState.Off;
                        State = GameplayState.CameraFinishMoving;
                        speedScale = 1.0f;
                        speedParticles.gameObject.SetActive(false);

                        Transform finishingRotationTransform = (new GameObject("RTF")).transform;
                        finishingRotationTransform.parent = Conveyour.Shapes.Last().transform;
                        finishingRotationTransform.localScale = Vector3.one;
                        finishingRotationTransform.localRotation = Quaternion.identity;
                        finishingRotationTransform.localPosition = Vector3.zero;
                        SceneVisual.transform.parent = finishingRotationTransform;
                        finishingCameraOffset = SceneVisual.transform.localPosition;
                        BreforeFinishPreparing();
                    }
                    break;

                case "finish":
                    if (State != GameplayState.FinishingSupplying)
                    {
                        finishingCameraOffset = SceneVisual.transform.position - Conveyour.Shapes.Last().transform.position;
                        currentValveState = ValveState.FinishingTimelaps;
                        State = GameplayState.FinishingSupplying;
                        Valve.GetCreamCreator().gameObject.SetActive(true);

                        OnReadyToDecorate?.Invoke();
                    }
                    break;

                case "complete":
                    if (currentValveState != ValveState.Off)
                    {
                        SwitchToFinishingAnimation();
                    }
                    break;
            }
        }
    }

    private void ProcessFatalObstacle()
    {
        Env.Instance.Sound.StopMusic();
        this.gameObject.GetComponentInChildren<SupplyAnimation>().PlayFailAnimation();
        _camShake.Play(1.5f);

        IsGameplayActive = false;

        speedFXController.TryToDecrease();

        Valve.Supply(false);
        Valve.SetSupplyAnimationTimeScale(0.0f);

        Guid? brokenSoundGuid = Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.BreakStick);

        Env.Instance.UI.Messages.ShowLevelFail(0, 0,
            () =>
            {
                if (brokenSoundGuid.HasValue && Env.Instance.Sound.IsPlaybackActive(brokenSoundGuid.Value))
                    Env.Instance.Sound.StopSound(brokenSoundGuid.Value);

                this.gameObject.GetComponentInChildren<SupplyAnimation>().StopFailAnimation();
                Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicGameplay);
                ProcessNoFatalObstacle();
                IsGameplayActive = true;
            },
            () =>
            {
                if (brokenSoundGuid.HasValue && Env.Instance.Sound.IsPlaybackActive(brokenSoundGuid.Value))
                    Env.Instance.Sound.StopSound(brokenSoundGuid.Value);
                this.gameObject.GetComponentInChildren<SupplyAnimation>().StopFailAnimation();

                MetagameRoomContext quitContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.Failed);

                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
                {
                    Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(true, quitContext, () =>
                    {
                        overlayInstance.Close();
                    });
                });
            });
    }

    [ContextMenu("Process Obstacle")]
    private void ProcessNoFatalObstacle()
    {
        Score.ResetCombos();
        _penaltyTimer = 2f;
        Valve.Supply(false);
        speedFXController.TryToDecrease();

        Animator animator = Valve.gameObject.transform.parent.gameObject.GetComponentInChildren<Animator>();
        if (null != animator)
        {
            Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.Block);

            Valve.SetSupplyAnimationTimeScale(0.0f);
            animator.SetTrigger("Go");

            AutonomousTimer.Create(1.0f, () =>
            {
                _camShake.Play(1.5f);
                Valve.SetSupplyAnimationTimeScale(1.0f);
                if (OptionsPanel.IsVibroEnabled)
                    MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Failure);
            });
        }
        else
        {
            _camShake.Play(1.5f);
            if (OptionsPanel.IsVibroEnabled)
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Failure);
        }
    }

    private void CreamCollisionHandler(Collider c, MonoBehaviour p, Vector3 v)
    {
        if (c.tag.Equals("tape"))
        {
            if (c.transform.position.x > -2)
            {
                Score.ResetCombos();
                OnCreamLost?.Invoke();
                PerfectsController.AddResult(0.0f, false);

                if (!IsTubeInFeverZone)
                    speedFXController.TryToDecrease();
            }
        }

        Shape collidedShape = c.GetComponentInParent<Shape>();
        if (null != collidedShape)
        {
            Conveyour.OnShapeCollidedByCream(collidedShape, v);
        }
    }

    [ContextMenu("Complete Level")]
    private void OnLevelComplete(MetagameRoomContext mrContext)
    {
        IsGameplayActive = false;

        if (IsExtraLevel || (Env.Instance.Inventory.CurrentLevelIndex <= 0))
        {
            Complete(mrContext);
            return;
        }
        
        // Lottery flow
        if (IsLotteryAvailable())
        {
            var prizes = Env.Instance.Inventory.Delivery.GetPrizes( BalanceDataProvider.Instance.CountOfAdditionalPrizes + 1);
            if (prizes.Count == 0)
            {
                Complete(mrContext);
                return;
            }

            Env.Instance.UI.Overlay.Set(this, new Color(0.16f, 0.59f, 0.953f, 1.0f), overlay =>
            {
                Env.Instance.UI.Messages.ShowLottery(mrContext, prizes, () =>
                {
                    Complete(mrContext);
                });
                overlay.Close();
            });

            return;
        }
        
        // Single chest flow
        if (!IsSingleChestAvailable())
        {
            Complete(mrContext);
            return;
        }

        var prize = Env.Instance.Inventory.Delivery.GetPrize(false);
        Debug.Log("Prize for chest:" + (prize ? prize.Name : "Null"));
        
        if(prize == null)
        {
           Complete(mrContext);
           return;
        }

        Env.Instance.UI.Overlay.Set(this, new Color(0.16f, 0.59f, 0.953f, 1.0f), overlay =>
        {
            Env.Instance.UI.Messages.ShowSingleChest(
                prize, mrContext,
                (!IsFreeSingleChestReceived || IsFreeSingleChestAvailable()),
                () => { Complete(mrContext); });
            overlay.Close();
        });
    }


    private bool IsLotteryAvailable()
    {
        var result = BalanceDataProvider.Instance.IsLotteryEnabled;
        result &= (Env.Instance.Inventory.CurrentLevelIndex % BalanceDataProvider.Instance.LotteryFrequency == 0);

        return result;
    }
    

    private bool IsSingleChestAvailable()
    {
        bool result = AdvertisingManager.Instance.GetPlacementSettings(AdsPlacements.CHEST).showAdsState != RewardedVideoShowingAdsState.None;

        result &= !IsFreeSingleChestReceived || AdvertisingManager.Instance.IsAdModuleByPlacementAvailable(AdModule.RewardedVideo, AdsPlacements.CHEST);
        result &= 3 > Env.Instance.Inventory.Keys;

        return result;
    }


    private bool IsFreeSingleChestAvailable()
    {
        bool result = AdvertisingManager.Instance.GetPlacementSettings(AdsPlacements.CHEST)
            .showAdsState == RewardedVideoShowingAdsState.FreeReward;

        return result;
    }


    private void Complete(MetagameRoomContext mrcontext)
    {
        CompleteLevel?.Invoke(mrcontext);
        
        var stars = Mathf.Max(PerfectsController.LevelStarsCount, CurrentLevelAsset.MinStarsCount);

        Env.Instance.Inventory.CurrentLevelIndex++;
        Env.Instance.Inventory.Save();
    }


    private void BreforeFinishPreparing()
    {
        Shape preLastShape = Conveyour.Shapes.Last();
        if (Conveyour.Shapes.Count > 1) preLastShape = Conveyour.Shapes[Conveyour.Shapes.Count - 2];

        FinishingFlowConfig finishingFlowConfig = (Env.Instance.Rooms.CurrentRoom.Config as GameplayRoomConfig).FinishingFlowConfig;

        CreamCreator creamCreator = Valve.GetCreamCreator();
        creamCreator.DetachCreamMesh().transform.parent = preLastShape.transform;
        creamCreator.TurnOff();
        creamCreator.gameObject.SetActive(false);
        creamCreator.StopSound();

        // move shapes down
        List<Shape> unneededShapes = Conveyour.Shapes.GetRange(0, Conveyour.Shapes.Count - 1);
        foreach (Shape shape in unneededShapes)
        {
            MovingAnimation.AddTo(
                shape.gameObject,
                finishingFlowConfig.SceneItemsMoveOutGraph,
                shape.gameObject.transform.position + Vector3.down * 10.0f, null);
        }

        // move conveyour down
        ConveyourRoot conveyourRoot = this.gameObject.GetComponentInChildren<ConveyourRoot>();
        MovingAnimation.AddTo(
            conveyourRoot.gameObject,
            finishingFlowConfig.SceneItemsMoveOutGraph,
            conveyourRoot.gameObject.transform.position + Vector3.down * 10.0f, null);

        GameObject supplyerObject = this.gameObject.GetComponentInChildren<SupplyAnimation>().gameObject;

        MovingAnimation.AddTo(
            supplyerObject,
            finishingFlowConfig.SceneItemsMoveOutGraph,
            supplyerObject.transform.position + Vector3.up * 4.0f,
            () =>
            {
                Valve.SwapToUnderstudySkin();

                MovingAnimation.AddTo(
                    supplyerObject,
                    finishingFlowConfig.SceneItemsMoveInGraph,
                    supplyerObject.transform.position - Vector3.up * 4.0f,
                    () =>
                    {
                        Valve.ForcedDownAndStop();
                    });
            });
    }


    private void SwitchToFinishingAnimation()
    {
        State = GameplayState.PreFinishingAnimation;
        currentValveState = ValveState.Off;

        Shape lastShape = Conveyour.Shapes.Last();
        CreamCreator creamCreator = Valve.GetCreamCreator();
        creamCreator.StopSound();

        FinishingFlowConfig finishingFlowConfig = (Env.Instance.Rooms.CurrentRoom.Config as GameplayRoomConfig)?.FinishingFlowConfig;
        GameObject supplyerObject = this.gameObject.GetComponentInChildren<SupplyAnimation>().gameObject;

        Env.Instance.Sound.PlaySound(AudioKeys.UI.Win);
        Conveyour.Stop();
        MovingAnimation.AddTo(
            supplyerObject,
            finishingFlowConfig.SceneItemsMoveOutGraph,
            supplyerObject.transform.position + Vector3.up * 4.0f,
            () =>
            {
                if (null != CurrentLevelAsset.FinalJar)
                {
                    State = GameplayState.FinishingAnimation;
                    finalJar.transform.parent = lastShape.transform;
                    finalJar.transform.localPosition = Vector3.zero;
                    finalJar.transform.localRotation = Quaternion.identity;
                    finalJar.transform.localScale = Vector3.one;
                    finalJar.gameObject.SetActive(true);

                    OnReadyToFinalize?.Invoke();
                }
                else
                {
                    FinishAnimations();
                }
            });
    }

    private bool screenWasPressed = true;
    private int jarSteps = 0;
    private MeshSurfacePoints meshSurfacePoints;
    private bool rotateInFinishing = true;
    private float prevDelta300 = 1.0f;
    private void MakeFinishingAnimation()
    {
        SceneVisual.transform.parent.Rotate(Vector3.up, 35.0f * Time.deltaTime);

        CreamCreator creamCreator = Valve.GetCreamCreator();
        Shape lastShape = Conveyour.Shapes.Last();
        FinishingFlowConfig finishingFlowConfig = (Env.Instance.Rooms.CurrentRoom.Config as GameplayRoomConfig)?.FinishingFlowConfig;

        if (null == meshSurfacePoints)
        {
            meshSurfacePoints = new MeshSurfacePoints(creamCreator.CreamMeshObject.GetComponent<MeshFilter>().mesh);
        }

        bool onPressed = !screenWasPressed && UserTouchGameplay;
        screenWasPressed = UserTouchGameplay;

        bool increaseJarSteps = false;
        if (onPressed)
        {
            increaseJarSteps = true;
            finalJar.Press(() => { });
        }

        if (increaseJarSteps)
        {
            jarSteps++;
            if (jarSteps == 1)
            {
                // Confiture falling
                ConfitureFalling.Create(
                    lastShape.transform,
                    lastShape.gameObject.layer,
                    finishingFlowConfig);

                // Confiture static
                Confiture.Create(
                    creamCreator.CreamMeshObject.transform,
                    lastShape.gameObject.layer,
                    finishingFlowConfig,
                    meshSurfacePoints);
            }

            if (jarSteps == 2)
            {
                // Confiture falling pieces
                ConfitureFallingPieces.Create(
                    true, true,
                    creamCreator.CreamMeshObject.transform,
                    lastShape.gameObject.layer,
                    finishingFlowConfig,
                    creamCreator.CreamMeshObject,
                    meshSurfacePoints, out bool hasSmallPieces);

                float delay = hasSmallPieces ? 1.0f : 0.0f;

                AutonomousTimer.Create(delay + 1.0f, () =>
                {
                    finalJar.GetOut();
                });

                AutonomousTimer.Create(delay + 1.7f, () =>
                {
                    FinishAnimations();
                });

                OnFinalized?.Invoke();
            }
        }
    }

    private void FinishAnimations()
    {
        MetagameRoomContext mrContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.None);
        mrContext.TypeOfDeliveredObject = _currentLevel.TypeOfDeliveredObject;

        Shape lastShape = Conveyour.Shapes.Last();
        GameObject animationObject = Instantiate(_currentLevel.FinishAnimationObject.gameObject, lastShape.transform, false);
        animationObject.transform.localPosition = Vector3.up * 0.30f;
        animationObject.transform.localScale = Vector3.one;
        animationObject.transform.localRotation = Quaternion.identity;

        FinalAnimatedBox finalAnimatedBox = animationObject.GetComponent<FinalAnimatedBox>();
        FinalAnimatedBox.BoxColor? colorVariant = finalAnimatedBox.ApplyRandomColors();

        if (colorVariant.HasValue)
        {
            mrContext.DeliveredObjectColor1 = colorVariant.Value.Box;
            mrContext.DeliveredObjectColor2 = colorVariant.Value.Tape;
        }

        float delay = 2.25f;
        switch (finalAnimatedBox.TypeOfBox)
        {
            case FinalAnimatedBox.BoxType.Common:
                Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.Box);
                break;

            case FinalAnimatedBox.BoxType.Plastic:
                Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.BoxPlastic);
                break;

            case FinalAnimatedBox.BoxType.Metall:
                delay += 1.0f;
                Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.BoxGold);
                break;

            case FinalAnimatedBox.BoxType.FastFood:
                Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.BoxBurger);
                break;
        }

        AutonomousTimer.Create(delay, () =>
        {
            Env.Instance.Sound.FadeOutCurrentlyPlayingMusic();
            OnLevelComplete(mrContext);
        });
    }


    private void TrackLevelStart()
    {
        _currentLevelStartTime = Time.realtimeSinceStartup;
    }

    #endregion



    #region Event handler

    private void AdvertisingManager_OnAdShow(AdModule adModule, AdActionResultType responseResultType)
    {
        if ((adModule == AdModule.Interstitial || adModule == AdModule.RewardedVideo) && responseResultType == AdActionResultType.Success)
        {
            //IsGameplayActive = false;
        }
    }


    private void AdvertisingManager_OnAdHide(AdModule adModule, AdActionResultType responseResultType)
    {
        if (adModule == AdModule.Interstitial || adModule == AdModule.RewardedVideo)
        {
            IsGameplayActive = true;
        }
    }


    private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEntered)
    {
         IsGameplayActive = previousGameplayActiveValue;
    }
    
    #endregion
}
