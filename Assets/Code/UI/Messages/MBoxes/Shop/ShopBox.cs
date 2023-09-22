using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Advertising;
using Modules.Analytics;
using Modules.General.Abstraction;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.NiceVibrations;
using BoGD;
using Code;
using Gadsme;


[Serializable]
public class ShopBox : UIMessageBox
{
    #region Fields

    [SerializeField] Button          closeButton;
    [SerializeField] Button          openNowButton;
    [SerializeField] Transform       shapeProgressRoot;
    [SerializeField] TextMeshProUGUI openNowPriceText;
    [SerializeField] VideoButton     watchAdButton;
    [SerializeField] TextMeshProUGUI watchAdPrizeText;

    [Space]
    [Header("Tabs settings")]
    [SerializeField] ShopTabsHandler tabsHandler;
    
    [Space]
    [Header("Open now button settings")]
    [SerializeField] Image openNowButtonImage;
    [SerializeField] Sprite openNowButtonSpriteOn;
    [SerializeField] Sprite openNowButtonSpriteOff;

    [SerializeField]
    private UnityEngine.EventSystems.EventSystem eventSystem = null;

    bool isOpenButtonEnabled = true;
    bool isVideoButtonEnabled = true;
    
    private string valveNameAtStart;
    private string creamNameAtStart;
    private string creamSprinklingNameAtStart;
    private GadsmePlacementVideo _gadsmePlacementVideo;

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        closeButton.onClick.AddListener(CloseButton_OnClick);
        openNowButton.onClick.AddListener(OpenNowButton_OnClick);

        tabsHandler.OnTabChanged += TabsHandler_OnTabChanged;

        Env.Instance.Inventory.OnBucksCountUpdated += Inventory_OnBucksCounterUpdated;

        GadsmeService.Instance.ChangeVideoBannersInteractivity(false);
    }

    void OnDisable()
    {
        GadsmeService.Instance.ChangeVideoBannersInteractivity(true);
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
        openNowButton.onClick.RemoveListener(OpenNowButton_OnClick);

        tabsHandler.OnTabChanged -= TabsHandler_OnTabChanged;

        Env.Instance.Inventory.OnBucksCountUpdated -= Inventory_OnBucksCounterUpdated;
    }

    #endregion


    
    #region Initialization

    public void Init(ContentItemInfo itemToHighlight = null)
    {
        bool isRandomItem = BalanceDataProvider.Instance.IsRandomShopItemPurchase;

        isOpenButtonEnabled = isRandomItem;
        isVideoButtonEnabled = isRandomItem;

        valveNameAtStart = Env.Instance.Inventory.ValveName;
        creamNameAtStart = Env.Instance.Rules.CreamSkin.Value;
        creamSprinklingNameAtStart = Env.Instance.Inventory.CurrentConfiture;

        openNowButton.gameObject.SetActive(isOpenButtonEnabled);
        watchAdButton.gameObject.SetActive(isVideoButtonEnabled);

        if (isVideoButtonEnabled)
        {
            watchAdButton.Init(AdModule.RewardedVideo, "shop", WatchAdButton_OnClick);
        }

        tabsHandler.Init(itemToHighlight);

        UpdateButtons();
    }

    #endregion



    #region Buttons update logic

    void UpdateButtons()
    {
        BalanceDataProvider provider = BalanceDataProvider.Instance;
        bool isRandomItem = provider.IsRandomShopItemPurchase;
        int openPrice = provider.ItemCoinsPrice;
        int videoReward = provider.CoinsVideoRewardShop;

        openNowPriceText.text = openPrice.ToString();
        watchAdPrizeText.text = "+" + videoReward;

        watchAdButton.gameObject.SetActive(isVideoButtonEnabled);

        bool isShapeProgressRequired = (tabsHandler.CurrentTab.assetsType == ContentAsset.AssetType.Shape);
        bool isOpenAvailable = (tabsHandler.CanOpenItem() && 
                               Env.Instance.Inventory.Bucks >= openPrice && isRandomItem);

        shapeProgressRoot.gameObject.SetActive(isShapeProgressRequired);
        openNowButton.gameObject.SetActive(isOpenButtonEnabled && !isShapeProgressRequired);         
        openNowButton.interactable = isOpenAvailable;
        openNowButtonImage.sprite = (isOpenAvailable) ? openNowButtonSpriteOn : openNowButtonSpriteOff;
    }

    #endregion



    #region Events handling

    void CloseButton_OnClick()
    {
        if (OptionsPanel.IsVibroEnabled)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);

        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        Env.Instance.Inventory.SetItemsKnown(new List<ContentAsset.AssetType>(tabsHandler.ShowedAssetTypes).ToArray());

        var parameters = new Dictionary<string, string>();
        if (Env.Instance.Inventory.ValveName != valveNameAtStart)
        {
            parameters["item_pipe"] = Env.Instance.Inventory.ValveName;
        }

        if (Env.Instance.Rules.CreamSkin.Value != creamNameAtStart)
        {
            parameters["item_cream"] = Env.Instance.Rules.CreamSkin.Value;
        }

        if (Env.Instance.Inventory.CurrentConfiture != creamSprinklingNameAtStart)
        {
            parameters["item_cream_sprinkling"] = Env.Instance.Inventory.CurrentConfiture;
        }
        
        Close();
    }


    void OpenNowButton_OnClick()
    {
        if (OptionsPanel.IsVibroEnabled)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);

        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        int openPrice = BalanceDataProvider.Instance.ItemCoinsPrice;

        if (tabsHandler.CanOpenItem() && Env.Instance.Inventory.TrySpendBucks(openPrice, category: "buy_item", itemId: "random_item"))
        {
            openNowButton.interactable = false;

            tabsHandler.TryOpenRandomItem(ShopItem_OnOpened);
        }
        else
        {
            UpdateButtons();
        }
    }


    void WatchAdButton_OnClick()
    {
        if (OptionsPanel.IsVibroEnabled)
            MMVibrationManager.Haptic(HapticTypes.LightImpact);

        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        watchAdButton.Reset();

        Env.Instance.Sound.StopMusic();
        eventSystem.enabled = false;
        enableES = StartCoroutine(EnableES());
        AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, "shop",
            (result) =>
        {
            Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);

            switch (result)
            {
                case AdActionResultType.Success:
                    int videoReward = BalanceDataProvider.Instance.CoinsVideoRewardShop;
                    Env.Instance.Inventory.AddBucks(videoReward, watchAdButton.transform, category: "reward", itemId: "ad_watch");
                    break;

                case AdActionResultType.NoInternet:
                    Env.Instance.UI.Messages.ShowInfoBox("label_no_video".Translate(), () =>
                    {
                    });
                    break;
            }

            UpdateButtons();

            eventSystem.enabled = true;
            if (enableES != null)
            {
                StopCoroutine(enableES);
            }
        });
    }

    private Coroutine enableES; 
    private IEnumerator EnableES()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        eventSystem.enabled = true;
        enableES = null;
    }

    void Inventory_OnBucksCounterUpdated(int amount, Transform animationRoot, Action callback)
    {
        UpdateButtons();
    }


    void ShopItem_OnOpened()
    {
        UpdateButtons();
    }


    void TabsHandler_OnTabChanged(ShopTabsHandler.ShopTabSettings tabSettings)
    {
        UpdateButtons();

        Env.Instance.SendWindow("skins/" + tabSettings.title.ToLower());
    }

    #endregion
}
