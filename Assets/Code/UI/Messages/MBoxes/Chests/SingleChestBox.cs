using System.Collections.Generic;
using BoGD;
using DG.Tweening;
using Modules.General;
using UnityEngine;
using UnityEngine.UI;

public class SingleChestBox : UIMessageBox
{
    [SerializeField] private List<ParticleSystem> _particleSystems;
    [SerializeField] private DOTweenAnimation _shineAnimation;
    [SerializeField] private Button _openForAd;
    [SerializeField] private Button _freeOpenButton;
    [SerializeField] private Button _noThanks;

    [SerializeField] private CanvasGroup _noThanksGroup;

    [SerializeField] private GameObject _askPanel;
    [SerializeField] private GameObject _rewardPanel;
    [SerializeField] private Button _get;

    [SerializeField] private GameObject _chest;
    [SerializeField] private Image _icon;
    [SerializeField] private DOTweenAnimation _iconIdleAnimation;

    [SerializeField] private Safe _safe;

    [SerializeField] private GameObject bubblePlate;
    [SerializeField] private Image bubbleIcon;

    [Space]
    [Header("Dynamics settings")]
    [SerializeField] private float _noThanksDelay = 1.0f;
    [SerializeField] private float _noThanksFadeDuration = 0.3f;
    
    [Header("Shine settings")]
    [SerializeField] private AnimationCurve _shineScaleCurve;
    [SerializeField] private float _shineScaleDuration;
    [SerializeField] private float _shineScaleDelay;

    [Header("Icon settings")]
    [SerializeField] private AnimationCurve _iconJumpCurve;
    [SerializeField] private float _iconFirstJumpOffset = 100.0f;
    [SerializeField] private float _iconSecondJumpOffset = 50.0f;
    [SerializeField] private float _iconFirstJumpDuration = 0.4f;
    [SerializeField] private float _iconSecondJumpDuration = 0.4f;
    [SerializeField] private float _iconJumpDelay = 0.1f;
    [SerializeField] private float _iconScaleDuration = 0.1f;

    private ContentAsset _prize;
    private MetagameRoomContext mrContext;


    protected override void Awake()
    {
        base.Awake();

        // _openForAd.Init(AdModule.RewardedVideo, "single_chest", VideoButton_OnClick);
        _openForAd.onClick.AddListener(VideoButton_OnClick);

        _noThanks.onClick.AddListener(() =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
            
            RequestVideoShowController.Instance.CancelRequest();

            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
            {
                overlay.Close();

                Close();
            });
        });

        _freeOpenButton.onClick.AddListener(() =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
            
            OpenChest();
        });

        _get.onClick.AddListener(() => {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            ReceivePrizeAndClose();
        });

        _shineAnimation.transform.localScale = Vector3.zero;
    }

    void OnDestroy()
    {
        Scheduler.Instance.UnscheduleMethod(this, NoThanksButtonAppearance);

        DOTween.Kill(this);
    }

    public void Init(ContentAsset asset, MetagameRoomContext mrContext, bool isFree = false)
    {
        // var freeOpen = false;
        this.mrContext = mrContext;
        _prize = asset;

        ShowBubbleIconFirstTime();

        _icon.sprite = asset.Icon;

        if (!isFree && _askPanel.gameObject.activeSelf)
        {
            _noThanks.gameObject.SetActive(false);
            _noThanksGroup.alpha = 0.0f;
            Scheduler.Instance.CallMethodWithDelay(this, NoThanksButtonAppearance, _noThanksDelay);
        }

        if (isFree)
        {
            _noThanks.gameObject.SetActive(false);
        }

        _openForAd.gameObject.SetActive(!isFree);
        _freeOpenButton.gameObject.SetActive(isFree);

        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        
        Env.Instance.SendWindow("single_chest");
    }


    private void ShowBubbleIconFirstTime()
    {
        const string ShowBubbleIconFirstTimePlayerPrefsKey = "ShowBubbleIconFirstTime";

        bool bubbleiConAlreadyShown = PlayerPrefs.HasKey(ShowBubbleIconFirstTimePlayerPrefsKey);

        if (bubbleiConAlreadyShown)
        {
            bubblePlate.gameObject.SetActive(false);
            return;
        }

        PlayerPrefs.SetInt(ShowBubbleIconFirstTimePlayerPrefsKey, 1);

        bubblePlate.gameObject.SetActive(true);

        bubbleIcon.sprite = _prize.Icon;
    }

    private void OpenChest()
    {
        _noThanks.interactable = false;
        _askPanel.gameObject.SetActive(false);

        _safe.Open();

        bubblePlate.gameObject.SetActive(false);

        Env.Instance.Sound.PlaySound(AudioKeys.UI.ChestOpen);

        AutonomousTimer.Create(1.0f, () =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

            int particleSystemsCount = _particleSystems.Count;
            for (int i = 0; i < particleSystemsCount; i++)
            {
                _particleSystems[i].Clear();
                _particleSystems[i].Play();
            }

            _rewardPanel.gameObject.SetActive(true);

            Debug.Log("Prize Received:" + _prize.Name);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["lootbox_id"] = "single";
            data["lootbox_count"] = 1;
            data["reward"] = _prize.Name.ToLower().Replace(' ', '_');

            Env.Instance.Inventory.Delivery.ApplyPrize(_prize);

            List<ContentItemInfo> itemsInfos = Env.Instance.Content.GetAvailableInfos(_prize.GetAssetType(), (info) => 
            {
                return info.Name.Equals(_prize.Name);
            });

            if (!itemsInfos.IsNullOrEmpty())
            {
                mrContext.LastItemReceived = itemsInfos.First();
            }

            _icon.transform.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();

            sequence.Insert(0.0f, _icon.transform.DOScale(1.0f, _iconScaleDuration).SetAutoKill(true)
                                                                                   .SetTarget(this));

            sequence.Insert(_iconJumpDelay, _icon.transform.DOMoveY(_icon.transform.position.y + _iconFirstJumpOffset, 0.5f * _iconFirstJumpDuration).SetEase(_iconJumpCurve)
                                                                                                                                                     .SetAutoKill(true)
                                                                                                                                                     .SetTarget(this)
                                                                                                                                                     .SetLoops(2, LoopType.Yoyo));

            sequence.Insert(_iconJumpDelay + _iconFirstJumpDuration, _icon.transform.DOMoveY(_icon.transform.position.y + _iconSecondJumpOffset, 0.5f * _iconSecondJumpDuration).SetEase(_iconJumpCurve)
                                                                                                                                                                                .SetAutoKill(true)
                                                                                                                                                                                .SetTarget(this)
                                                                                                                                                                                .SetLoops(2, LoopType.Yoyo));

            sequence.AppendCallback(() => { _iconIdleAnimation.DOPlay(); });
            sequence.AppendInterval(_shineScaleDelay);
            sequence.Append(_shineAnimation.transform.DOScale(1.0f, _shineScaleDuration).SetEase(_shineScaleCurve)
                                                                                        .SetTarget(this)
                                                                                        .SetAutoKill(true)
                                                                                        .OnComplete(() => { _shineAnimation.DOPlay(); }));                                                                            

            sequence.SetAutoKill(true).SetTarget(this);
        });
    }

    private void ReceivePrizeAndClose()
    {
        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            overlay.Close();

            Close();
        });
    }


    void NoThanksButtonAppearance()
    {
        _noThanks.gameObject.SetActive(true);
        _noThanksGroup.DOFade(1.0f, _noThanksFadeDuration).SetTarget(this).SetAutoKill(true);
    }


    #region Events handling

    void VideoButton_OnClick()
    {
        if (Env.Instance.Inventory.Bucks < 500) return;
        
        Env.Instance.Inventory.TrySpendBucks(500);
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
        OpenChest();
    }
    
    #endregion
}
