using System.Collections.Generic;
using BoGD;
using DG.Tweening;
using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestsBox : UIMessageBox
{
    [SerializeField] private VideoButton _openForAd;
    [SerializeField] private Button _buyKeysButton;
    [SerializeField] private GameObject _buyKeysButtonLock;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private int _price;
    [SerializeField] private Button _noThanks;
    
    [SerializeField] private CanvasGroup _noThanksGroup;

    [SerializeField] private GameObject _askPanel;
    [SerializeField] private GameObject _rewardPanel;
    [SerializeField] private GameObject _bestPrizePanel;
    [SerializeField] private Image _rewardIcon;
    [SerializeField] private Image _bestPrizeIcon;
    [SerializeField] CakesPanel _cakesPanel;
    [SerializeField] private Button _get;

    [SerializeField] List<ChestItem> _chests;

    [SerializeField] ParticleSystem _particles;

    [Space]
    [Header("Dynamics settings")]
    [SerializeField] private float _chestsAppearanceOffset = 0.1f;
    [SerializeField] private float _noThanksDelay = 1.0f;
    [SerializeField] private float _noThanksFadeDuration = 0.3f;
    [SerializeField] private float _coinsHapticInterval = 0.05f;
    [SerializeField] private int _coinsHapticCount = 3;

    private bool advertisementViewed = false;
    int _bestPrizeIndex;

    private int _openedForInterstitialCount = 0;
    private int _openedChests = 0;
    private HashSet<int> _openingChests = new HashSet<int>();

    private ContentAsset _bestPrize;
    private MetagameRoomContext mrContext;
    private bool mboxClosing = false;
    private int chestCount = 0;

    protected override void Awake()
    {
        base.Awake();

        SubscribeAskPanel();

        for (int i = 0; i < _chests.Count; i++)
        {
            int index = i;
            _chests[i].Safe.gameObject.SetActive(false);
            _chests[i].Button.onClick.AddListener(()=> {
                Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

                if (_cakesPanel.TrySpendCake())
                {
                    OpenChest(index);
                }
                else
                {
                    TryToShowInterstitialForKeys(index);
                }
            });
        }

        UpdatePanels();
        
        _buyKeysButton.onClick.AddListener((() =>
        {
            if (Env.Instance.Inventory.Bucks >= _price)
            {
                Env.Instance.Inventory.TrySpendBucks(_price);
                
                const int keysRewardCount = 3;
                advertisementViewed = true;
                Env.Instance.Inventory.AddKeys(keysRewardCount);
                UpdatePanels();
                CheckBuyButtonAvailable();
            }
            
        }));
    }


    void OnDestroy()
    {
        DOTween.Kill(this);

        Scheduler.Instance.UnscheduleMethod(this, NoThanksButtonAppearance);
    }


    public void Init(ContentAsset prize, MetagameRoomContext mrContext)
    {
        this.mrContext = mrContext;

        _bestPrize = prize;
        _bestPrizeIndex = _bestPrize == null ? -1 : Random.Range(0, _chests.Count);

        if (_bestPrize == null)
        {
            _bestPrizePanel.gameObject.SetActive(false);
        }
        else
        {
            _bestPrizeIcon.sprite = _bestPrize.Icon;

            _rewardIcon.rectTransform.sizeDelta = new Vector2(_rewardIcon.rectTransform.rect.width,
                (float)_rewardIcon.rectTransform.rect.width *
                (float)_bestPrize.Icon.rect.height /
                (float)_bestPrize.Icon.rect.width);

            _bestPrizeIcon.rectTransform.sizeDelta = new Vector2(_bestPrizeIcon.rectTransform.rect.width,
                (float)_bestPrizeIcon.rectTransform.rect.width *
                (float)_bestPrize.Icon.rect.height /
                (float)_bestPrize.Icon.rect.width);
        }

        _cakesPanel.gameObject.SetActive(false);

        Sequence sequence = DOTween.Sequence();
        int chestsCount = _chests.Count;
        int chestIndex = 0;
        for (int i = 0; i < chestsCount; i++)
        {
            _chests[i].TurnOffButtonUntilAnimationEvent();
            sequence.InsertCallback(i * _chestsAppearanceOffset, () =>
            {
                _chests[chestIndex++].Safe.gameObject.SetActive(true);
            });
        }

        sequence.AppendCallback(() => 
        {
            UpdatePanels();
        });

        sequence.SetAutoKill(true).SetTarget(this).Play();
        chestCount = 0;
        _priceText.text = _price.ToString();
        
        CheckBuyButtonAvailable();
    }

    private void CheckBuyButtonAvailable()
    {
        _buyKeysButtonLock.SetActive(Env.Instance.Inventory.Bucks < _price);
    }

    private bool inInterstitialProgress = false;
    private void TryToShowInterstitialForKeys(int chestIndex)
    {
        if (inInterstitialProgress) return;
        inInterstitialProgress = true;

        AdvertisingManager.Instance.TryShowAdByModule(AdModule.Interstitial, AdPlacementType.NineChests, (result) =>
            {
                inInterstitialProgress = false;

                if (result == AdActionResultType.Success)
                {
                    _openedForInterstitialCount++;
                    advertisementViewed = true;

                    OpenChest(chestIndex);

                    UpdatePanels();
                }
            });

    }


    private bool alreadyClosing = false;
    private void UpdatePanels()
    {
        if (mboxClosing) return;

        bool isCakes = Env.Instance.Inventory.Keys > 0;
        bool areClosedChestsAvailable = (9 != _openedChests);

        if (_cakesPanel.gameObject.activeSelf != isCakes)
        {
            if (isCakes)
            {
                _cakesPanel.gameObject.SetActive(true);
                _cakesPanel.Show();
            }
            else
            {
                _cakesPanel.Hide(() => 
                {
                    _cakesPanel.gameObject.SetActive(false);
                });
            }
        }

        bool askPanelWasVisible = _askPanel.activeSelf;
        bool isAskPanelRequired = !isCakes && areClosedChestsAvailable;
        bool isNoThanksAnimationRequired = !_askPanel.gameObject.activeSelf && isAskPanelRequired;
        _askPanel.SetActive(isAskPanelRequired);

        _openForAd.Reset();

        if (isNoThanksAnimationRequired)
        {
            _noThanks.gameObject.SetActive(false);
            _noThanksGroup.alpha = 0.0f;
            Scheduler.Instance.CallMethodWithDelay(this, NoThanksButtonAppearance, _noThanksDelay);
        }

        bool shouldQuit = (!areClosedChestsAvailable && (_openingChests.Count == 0));

        if (!alreadyClosing && shouldQuit)
        {
            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
            {
                alreadyClosing = true;
                _askPanel.SetActive(false);

                if (!_rewardPanel.gameObject.activeSelf)
                {
                    Scheduler.Instance.CallMethodWithDelay(this, () => 
                    {
                        overlay.Close();

                        mboxClosing = true;
                        CloseSelf();
                    }, 0.75f);
                }
            });
        }
        
        CheckBuyButtonAvailable();
    }

    private void CloseSelf()
    {
        Close();
        chestCount = 0;
    }

    private List<int> FindClosedChests()
    {
        List<int> chestIndices = new List<int>();

        for (int i = 0; i < _chests.Count; i++)
        {
            if (_chests[i].Button.interactable)
            {
                chestIndices.Add(i);
            }
        }

        return chestIndices;
    }

    private void WatchAdButtonClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        // AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo,
        //     "multiple_chests", (resultType) =>
        //     {
        //         switch (resultType)
        //         {
        //             case AdActionResultType.Success:
        //                 int keysRewardCount = 3;
        //                 advertisementViewed = true;
        //                 Env.Instance.Inventory.AddKeys(keysRewardCount);
        //                 
        //                 break;
        //
        //             case AdActionResultType.NoInternet:
        //                 Env.Instance.UI.Messages.ShowInfoBox("label_no_video".Translate(), () => { });
        //                 break;
        //         }
        //
        //         UpdatePanels();
        //     });
    }

    private void SubscribeAskPanel()
    {
        // _openForAd.Init(AdModule.RewardedVideo, "multiple_chests", WatchAdButtonClick);

        _noThanks.onClick.AddListener(() =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            mboxClosing = true;
            CloseSelf();
        });

        _get.onClick.AddListener(() => {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
            {
                overlay.Close();

                _rewardPanel.gameObject.SetActive(false);
                _openingChests.Remove(_bestPrizeIndex);
                UpdatePanels();
            });
        });
    }

    private void OpenChest(int chestIndex)
    {
        if (!advertisementViewed && (chestIndex == _bestPrizeIndex))
        //if ((chestIndex == _bestPrizeIndex) && (_openedChests < 8))
        {
            List<int> closedChests = FindClosedChests();
            closedChests.Remove(_bestPrizeIndex);
            _bestPrizeIndex = closedChests[Random.Range(0, closedChests.Count)];
        }

        _openedChests++;
        _openingChests.Add(chestIndex);

        if (OptionsPanel.IsVibroEnabled)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        string prize = "";
        if (chestIndex == _bestPrizeIndex)
        {
            prize = _bestPrize.Name;
            _chests[chestIndex].Open(_bestPrize.Icon, () =>
            {
                if (OptionsPanel.IsVibroEnabled)
                    MMVibrationManager.Haptic(HapticTypes.LightImpact);

                _rewardIcon.sprite = _bestPrize.Icon;
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
                {
                    overlay.Close();

                    _rewardPanel.gameObject.SetActive(true);

                    Env.Instance.Inventory.Delivery.ApplyPrize(_bestPrize);

                    List<ContentItemInfo> itemsInfos = Env.Instance.Content.GetAvailableInfos(_bestPrize.GetAssetType(), (info) => 
                    {
                        return info.Name.Equals(_bestPrize.Name);
                    });

                    if (!itemsInfos.IsNullOrEmpty())
                    {
                        mrContext.LastItemReceived = itemsInfos.First();
                    }

                    // _cakesPanel.gameObject.SetActive(false);

                    _particles.gameObject.SetActive(true);

                    Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);
                });
            });
        }
        else
        {
            int prizeCoins = Random.Range(2, 20) * 5;
            prize = "soft_" + prizeCoins;
            _chests[chestIndex].Open(prizeCoins, () =>
            {
                if (OptionsPanel.IsVibroEnabled)
                    MMVibrationManager.Haptic(HapticTypes.LightImpact);
                Env.Instance.Inventory.AddBucks(
                    prizeCoins, 
                    _chests[chestIndex].CoinsRoot, 
                    () =>
                    {
                        CoinsHaptic();

                        _openingChests.Remove(chestIndex);
                        UpdatePanels();
                    }, category: "reward", itemId: "open_lootbox");
            });
        }

        chestCount++;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["lootbox_id"] = "keys";
        data["lootbox_count"] = chestCount;
        data["reward"] = prize.ToLower().Replace(' ', '_');
    }


    void NoThanksButtonAppearance()
    {
        _noThanks.gameObject.SetActive(true);
        _noThanksGroup.DOFade(1.0f, _noThanksFadeDuration).SetTarget(this).SetAutoKill(true);
    }


    Sequence CoinsHaptic()
    {
        Sequence sequence = DOTween.Sequence();

        if (_coinsHapticCount > 0)
        {
            sequence.AppendCallback(() => 
            {
                if (OptionsPanel.IsVibroEnabled)
                    MMVibrationManager.Haptic(HapticTypes.LightImpact);
            });

            for (int i = 0; i < _coinsHapticCount; i++)
            {
                sequence.AppendInterval(_coinsHapticInterval);

                sequence.AppendCallback(() => 
                {
                    if (OptionsPanel.IsVibroEnabled)
                        MMVibrationManager.Haptic(HapticTypes.LightImpact);
                });
            }
        }

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    /*void AdvertisingManager_OnAdRespond(MoPubModule adModule, int delay,
        MoPubAdActionResultType responseResultType, string adIdentifier)
    {
        if (adModule == AdModule.RewardedVideo)
        {
            askPanelAdsLoader.SetLoaderState(responseResultType != MoPubAdActionResultType.Success);
            _openForAd.interactable = responseResultType == MoPubAdActionResultType.Success;
        }
    }*/
}
