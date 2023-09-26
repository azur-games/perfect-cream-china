using BoGD;
using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BoGD;


public class GameplayRoomUI : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] Text[] _texts;

    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private CanvasGroup _gameCanvasGroup;
    [SerializeField] private OptionsPanel _optionsPanel;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private CustomButton _backgroundButton;

    [SerializeField] private TextMeshProUGUI _level;

    [SerializeField] private Image _progressFill;
    [SerializeField] private ProgressBarController _progressBarController;
    [SerializeField] private CoinsBoxProgressController _coinsBoxProgressController;

    [Space]
    [Header("Hold to... settings")]
    [SerializeField] private Transform _holdToRoot;
    [SerializeField] private Transform _holdToPlayLabel;
    [SerializeField] private TextMeshProUGUI _holdToDecorateLabel;
    [SerializeField] private float _holdToDelay = 1.0f;

    [Space]
    [Header("Fever settings")]
    [SerializeField] private FeverUIHelper feverUI;

    private float           _holdToTimer = 0.0f;
    private bool            _isHoldToTimerActive = false;
    private bool            _isFinilizing = false;
    private int             mistakes = 0;

    private bool IsUIEnabled {
        get => CustomPlayerPrefs.GetBool("isUIEnabled", true);
        set => CustomPlayerPrefs.SetBool("isUIEnabled", value);
    }

    private Score _score;



    private GameplayRoom _gameplayRoom;
    private GameplayRoom GamePlayRoom => Env.Instance.Rooms.GameplayRoom;

    #endregion



    #region Unity lifecycle

    private void Awake()
    {
        Env.Instance.Rooms.GameplayRoom.OnUILoaded(this);

        InitMainMenu();
    }


    private void Start()
    {
        if (!BalanceDataProvider.Instance.IsCoinsBoxEnabled)
            _coinsBoxProgressController.gameObject.SetActive(false);

        GamePlayRoom.Controller.CompleteLevel += CompleteLevel;
        GamePlayRoom.Controller.OnReadyToDecorate += GameplayController_OnReadyToDecorate;
        GamePlayRoom.Controller.OnReadyToFinalize += GameplayController_OnReadyToFinalize;
        GamePlayRoom.Controller.OnFinalized += GameplayController_OnFinalize;

        _score = GamePlayRoom.Controller.Score;
        _score.Charged += ScoreChargedHandler;
        _score.Reseted += ScoreResetedHandler;

        _level.text = "label_level".Translate() + (Env.Instance.Inventory.CurrentLevelIndex + 1);

        _progressBarController.InitProgressBar(GamePlayRoom.Controller.CurrentLevelAsset.Icon, GamePlayRoom.Controller.CurrentLevelAsset.BaseUIColor);

        _progressFill.color = GamePlayRoom.Controller.CurrentLevelAsset.BaseUIColor;

        _holdToRoot.gameObject.SetActive(true);
        _holdToPlayLabel.gameObject.SetActive(true);
        _holdToDecorateLabel.gameObject.SetActive(false);

        _backgroundButton.PointerDown += () =>
        {
            if (_holdToRoot.gameObject.activeSelf)
            {
                _holdToRoot.gameObject.SetActive(false);

                if (GamePlayRoom.Controller.State == GameplayController.GameplayState.Process)
                {
                    _gamePanel.SetActive(true);
                    GamePlayRoom.Controller.StartGame();
                }
            }

            _holdToTimer = 0.0f;
            _isHoldToTimerActive = !_isFinilizing && (GamePlayRoom.Controller.State == GameplayController.GameplayState.FinishingAnimation);

            if (null != GamePlayRoom) GamePlayRoom.Controller.UserTouchGameplay = true;
        };

        _backgroundButton.PointerUp += () =>
        {
            if (null != GamePlayRoom) GamePlayRoom.Controller.UserTouchGameplay = false;

            if (GamePlayRoom.Controller.State == GameplayController.GameplayState.FinishingSupplying)
            {
                _isHoldToTimerActive = true;
            }
        };

        AdvertisingManager.Instance.OnAdClick += AdvertisingManager_OnAdClick;
        LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;

        Env.Instance.SendStart();

        Env.Instance.Rooms.GameplayRoom.Controller.PerfectsController.OnChunkFinishedAction += GameplayController_OnChunkFinished;
        Env.Instance.Rooms.GameplayRoom.Controller.OnCreamLost += GameplayController_OnCreamLost;
        Env.continues = 0;
    }


    private void Update()
    {
        if (_isHoldToTimerActive)
        {
            _holdToTimer += Time.deltaTime;

            CheckHoldToTimer();
        }

        if (!GameplayController.IsGameplayActive)
            return;

        if (GamePlayRoom == null)
            return;

        _progressFill.fillAmount = GamePlayRoom.Controller.ProgressWithOffset;
    }


    private void OnDestroy()
    {
        AdvertisingManager.Instance.OnAdClick -= AdvertisingManager_OnAdClick;
        LLApplicationStateRegister.OnApplicationEnteredBackground -= LLApplicationStateRegister_OnApplicationEnteredBackground;
        Env.Instance.Rooms.GameplayRoom.Controller.PerfectsController.OnChunkFinishedAction -= GameplayController_OnChunkFinished;
        Env.Instance.Rooms.GameplayRoom.Controller.OnCreamLost -= GameplayController_OnCreamLost;
    }

    #endregion



    #region Methods

    public void OnFeverModeStarts()
    {
        feverUI.PlayStart();
    }
    
    public void OnFeverModeFinished(int prizeCoinsCount, bool isPerfect)
    {
        if (0 == prizeCoinsCount) return;
        feverUI.PlayFinish(prizeCoinsCount, isPerfect, () =>
        {

        });
    }

    private void InitMainMenu()
    {
        _optionsButton.onClick.AddListener(() =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
            Env.Instance.Sound.StopMusic();
            GameplayController.IsGameplayActive = false;

            if (GamePlayRoom != null)
            {
                Env.Instance.Rooms.GameplayRoom.Controller.StopSound();
                GamePlayRoom.Controller.UserTouchGameplay = false;

                if (GamePlayRoom.Controller.State != GameplayController.GameplayState.FinishingSupplying)
                {
                    GamePlayRoom.Controller.Valve.Supply(false);
                }
            }

            _optionsPanel.Show();
        });

        _quitButton.onClick.AddListener(() =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
            Env.Instance.Sound.FadeOutCurrentlyPlayingMusic();

            MetagameRoomContext quitContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.Failed);

            if (GamePlayRoom != null)
            {
                Env.Instance.Rooms.GameplayRoom.Controller.StopSound();
                GamePlayRoom.Controller.UserTouchGameplay = false;

                //GamePlayRoom.Controller.TrackLevelFinish(Modules.Analytics.CommonEvents.LevelResult.Exit);
            }

            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
            {
                Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(true, quitContext, () =>
                {
                    overlayInstance.Close();
                });
            });

            Env.Instance.SendFinish("leave", 0, 0, (int)(Env.Instance.Rooms.GameplayRoom.Controller.ProgressWithOffset * 100), 0);
        });
    }


    private void ToggleUI(bool isOn)
    {
        _gameCanvasGroup.alpha = (isOn) ? 1.0f : 0.0f;
    }


    private void UpdateFontColors(Color color)
    {
        foreach (var t in _texts)
        {
            if(t) t.color = color;
        }

    }


    private void CheckHoldToTimer()
    {
        if (_holdToTimer >= _holdToDelay && (GamePlayRoom.Controller.State == GameplayController.GameplayState.FinishingSupplying || 
                                             GamePlayRoom.Controller.State == GameplayController.GameplayState.FinishingAnimation))
        {
            _holdToRoot.gameObject.SetActive(true);
            _holdToDecorateLabel.gameObject.SetActive(true);
            _holdToPlayLabel.gameObject.SetActive(false);

            _holdToTimer = 0.0f;
            _isHoldToTimerActive = false;
        }
        else if (!(GamePlayRoom.Controller.State == GameplayController.GameplayState.FinishingSupplying ||
                   GamePlayRoom.Controller.State == GameplayController.GameplayState.FinishingAnimation))
        {
            _holdToTimer = 0.0f;
            _isHoldToTimerActive = false;
        }
    }


    [ContextMenu("Complete level")]
    private void CompleteLevel(MetagameRoomContext mrcontext)
    {
        int stars = GamePlayRoom.Controller.PerfectsController.LevelStarsCount;

        if (Env.IsUABuild && !IsUIEnabled)
        {
            MetagameRoomContext quitContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.Completed, mrcontext.LastItemReceived);
            quitContext.TypeOfDeliveredObject = mrcontext.TypeOfDeliveredObject;
            quitContext.DeliveredObjectColor1 = mrcontext.DeliveredObjectColor1;
            quitContext.DeliveredObjectColor2 = mrcontext.DeliveredObjectColor2;

            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
            {
                Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(true, quitContext, () =>
                {
                    overlayInstance.Close();
                });
            });
            return;
        }

        _gamePanel.SetActive(false);

        stars = Mathf.Max(stars, GamePlayRoom.Controller.CurrentLevelAsset.MinStarsCount);

        if (stars > 0)
        {
            MetagameRoomContext metaContext;
            LevelCompleteBox.LevelCompleteBoxType screenType;

            if (GamePlayRoom.Controller.IsExtraLevel)
            {
                metaContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.CompletedExtraLevel, mrcontext.LastItemReceived);
                screenType = LevelCompleteBox.LevelCompleteBoxType.Extra;
            }
            else
            {
                metaContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.Completed, mrcontext.LastItemReceived);
                screenType = LevelCompleteBox.LevelCompleteBoxType.Default;
            }

            metaContext.TypeOfDeliveredObject = mrcontext.TypeOfDeliveredObject;
            metaContext.DeliveredObjectColor1 = mrcontext.DeliveredObjectColor1;
            metaContext.DeliveredObjectColor2 = mrcontext.DeliveredObjectColor2;

            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
            {
                int bucksReward = Env.Instance.Rooms.GameplayRoom.Controller.CurrentLevelBucksReward;
                Env.Instance.UI.Messages.ShowLevelCompletion(stars, bucksReward, metaContext, screenType, (levelQuitContext) =>
                {
                    Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
                    {
                        MetagameRoomContext quitContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.Completed,
                                                                                  levelQuitContext.LastItemReceived);
                        quitContext.TypeOfDeliveredObject = levelQuitContext.TypeOfDeliveredObject;
                        quitContext.DeliveredObjectColor1 = levelQuitContext.DeliveredObjectColor1;
                        quitContext.DeliveredObjectColor2 = levelQuitContext.DeliveredObjectColor2;

                        Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(true, quitContext, () =>
                        {
                            overlayInstance.Close();
                        });
                    });
                });

                overlay.Close();
            });
        }
        else
        {
            _gamePanel.SetActive(false);

            GameplayController.IsGameplayActive = false;

            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
            {
                Env.Instance.UI.Messages.ShowLevelResultFailBox(() =>
                {
                    Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
                    {
                        MetagameRoomContext quitContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.Failed);
                        Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(true, quitContext, () =>
                        {
                            overlayInstance.Close();
                        });
                    });
                });

                overlay.Close();
            });
        }

        //var progress = (int)GamePlayRoom.Controller.ProgressWithOffset * 100;
        //var reward = Env.Instance.Rooms.GameplayRoom.Controller.CurrentLevelBucksReward;
        //Env.Instance.SendFinish(stars > 0 ? "win" : "lose", stars, mistakes, progress, reward);
    }    

    private void GameplayController_OnChunkFinished(PerfectsController.ChunkResultType result)
    {
        if(result == PerfectsController.ChunkResultType.Negative)
        {
        }
    }

    private void GameplayController_OnCreamLost()
    {
        //mistakes++;
    }

    /*
    private void FailLevel()
    {
        if (Env.IsUABuild && !IsUIEnabled)
        {
            Env.Instance.Rooms.SwitchToRoom(typeof(GameplayRoom).ToString(), true, null);
            return;
        }

        _gamePanel.SetActive(false);

        GameplayController.IsGameplayActive = false;
        Env.Instance.UI.Overlay.Set(this, new Color(0.1f, 0.1f, 0.1f, 0.75f), (grayOverlay) =>
        {
            Env.Instance.UI.Messages.ShowLevelFail(_score.Total, GameplayController.BestScore, () =>
            {
                grayOverlay.Close();

                Env.Instance.UI.Overlay.Set(this, Color.white, (overlayInstance) =>
                {
                    Env.Instance.Rooms.SwitchToRoom(typeof(GameplayRoom).ToString(), true, () =>
                    {
                        overlayInstance.Close();
                    });
                });
            }, null);
        });
    }*/


    private void ScoreChargedHandler()
    {
        //_totalScore.text = "" + _score.Total.ToString();
        //_comboPanel.Charge(_score);
       
    }

    private void ScoreResetedHandler()
    {
        //_totalScore.text = "" + _score.Total.ToString();
        //_comboPanel.Reset();
    }


    private void UIVisibilityChangeHandler(bool isVisible)
    {
        IsUIEnabled = isVisible;
        ToggleUI(IsUIEnabled);
    }


    private void FontColorChangeHandler(Color color)
    {
        UpdateFontColors(color);
    }


    private void GameplayController_OnReadyToDecorate()
    {
        if (!GamePlayRoom.Controller.UserTouchGameplay)
        {
            var result = "label_hold_to_decorate".Translate();

            _holdToDecorateLabel.text = result;

            _holdToTimer = 0.0f;
            _isHoldToTimerActive = true;
        }
    }


    private void GameplayController_OnReadyToFinalize()
    {
        _holdToDecorateLabel.text = "label_tap_to_finalize".Translate();

        _holdToTimer = 0.0f;
        _isHoldToTimerActive = true;
    }


    private void GameplayController_OnFinalize()
    {
        _holdToRoot.gameObject.SetActive(false);
        _isFinilizing = true;
        _holdToTimer = 0.0f;
        _isHoldToTimerActive = false;
    }


    private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEntered)
    {
        if (isEntered && GamePlayRoom != null)
        {
            GamePlayRoom.Controller.UserTouchGameplay = false;
        }
    }


    private void AdvertisingManager_OnAdClick(AdModule adModule)
    {
        if (adModule == AdModule.Banner)
        {
            GameplayController.IsGameplayActive = false;
            _optionsPanel.Show();
        }
    }

    #endregion
}