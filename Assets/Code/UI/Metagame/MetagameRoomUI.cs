using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BoGD;
using Code.Subscriptions;
using Modules.General.HelperClasses;


public class MetagameRoomUI : MonoBehaviourBase
{
    
    #region Fields

    [SerializeField] private StartGameplayButton startButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button noAdsButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private AnimationBool upgradeButtonAnimationBool;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button coinsBoxButton;
    [SerializeField] private Button changeLanguageButton;
    [SerializeField] private ButtonScaleBehaviour coinsBoxButtonScaleBehaviour;

    [SerializeField] private Spinner noAdsSpinner;

    [SerializeField] private Text upgradePrice;
    [SerializeField] private OptionsPanel optionsPanel;

    [SerializeField] private MetagamePersBubble metagamePersBubble;
    [SerializeField] private MetagamePiggyBubble metagamePiggyBubble;

    [SerializeField] private NewIconHelper newShopItemsIconHelper;
    [SerializeField] private GameObject CanPurchaseIcon;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private CustomSubscriptionPopUp customSubscriptionPopUp;

    public Camera UICamera;
    private IAdvertisingNecessaryInfo adInfo = null;

    #endregion



    #region Unity lifecycle

    private void Start()
    {
        metagamePersBubble.OnNewShape += (itemInfo) =>
        {
            Env.Instance.UI.Messages.ShowContentReceive(itemInfo, () => {});
        };

        newShopItemsIconHelper.Refresh();
        UpdateCanPurchaseIcon();

        if (((Env.Instance.Rooms.MetagameRoom.Context.GameplayResult == MetagameRoomContext.GameplaySessionResult.Completed) ||
            (Env.Instance.Rooms.MetagameRoom.Context.GameplayResult == MetagameRoomContext.GameplaySessionResult.CompletedExtraLevel)) 
            && (Env.Instance.Inventory.CurrentLevelIndex == 2))
        {
            // RateUsFeedbackPopupScreen.ShowRatePopupIfCan();
        }
        
                    
        if (CustomPlayerPrefs.GetBool("FirstLaunch", true))
        {
            customSubscriptionPopUp.Show();
            CustomPlayerPrefs.SetBool("FirstLaunch", false);
        }

        if (UserActivityChecker.Instance.IsCoinsBoxPeriodicRewardAvailable)
        {
            Env.Instance.UI.Messages.ShowCoinsPiggyRewardBox(() => 
            {
                UpdateCoinsBoxButton();
                UpdateBubbles();
            });
        }

        UpdateCoinsBoxButton();
        UpdateBubbles();

        if (BalanceDataProvider.Instance.IsReceivedShapeHighlightingEnabled && 
            Env.Instance.Rooms.MetagameRoom.Context.LastItemReceived != null)
        {
            UIMessageBox mbox = Env.Instance.UI.Messages.ShowShopBox(Env.Instance.Rooms.MetagameRoom.Context.LastItemReceived);
            mbox.AddOnCloseDelegate(() =>
            {
                newShopItemsIconHelper.Refresh();
                UpdateCanPurchaseIcon();
            });
        }

        bool useCommonStartButton = !Env.Instance.Inventory.IsNextExtraLevel();
        startButton.Init(useCommonStartButton, StartButton_OnClick);

        LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;
    }

    void OnDestroy()
    {
        LLApplicationStateRegister.OnApplicationEnteredBackground -= LLApplicationStateRegister_OnApplicationEnteredBackground;
    }

    private void UpdateCanPurchaseIcon()
    {
        if (newShopItemsIconHelper.gameObject.activeSelf)
        {
            CanPurchaseIcon.SetActive(false);
            return;
        }

        CanPurchaseIcon.gameObject.SetActive(
            Env.Instance.Inventory.CanPurchaseSomething(
                ContentAsset.AssetType.Confiture,
                ContentAsset.AssetType.Valve,
                ContentAsset.AssetType.CreamSkin));
    }

    private void Update()
    {
        if (null == Env.Instance.Rooms.MetagameRoom.Controller)
        {
            return;
        }

        UpdateUpgradeButton();

        if(Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                coinsBoxButton.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                upgradeButton.onClick.Invoke();
            }
        }
    }

    void OnEnable()
    {
        adInfo = adInfo ?? Services.AdvertisingManagerSettings.AdvertisingInfo;

        int barGradeLevelFromConfig = BalanceDataProvider.Instance.BarGradeLevel;
        upgradeButton.gameObject.SetActive(barGradeLevelFromConfig == 0);

        shopButton.onClick.AddListener(ShopButton_OnClick);
        upgradeButton.onClick.AddListener(UpgradeButton_OnClick);

        settingsButton.onClick.AddListener(SettingsButton_OnClick);
        changeLanguageButton.gameObject.SetActive(BoGD.CheatController.Instance.CheatBuid);
        changeLanguageButton.onClick.AddListener(ChangeLanguage_OnClick);

        noAdsButton.onClick.AddListener(NoAdsButton_OnClick);
        coinsBoxButton.onClick.AddListener(CoinsBoxButton_OnClick);

        Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);

        UpdateUpgradePrice();
    }



	void OnDisable()
    {
        shopButton.onClick.RemoveListener(ShopButton_OnClick);
        upgradeButton.onClick.RemoveListener(UpgradeButton_OnClick);
        settingsButton.onClick.RemoveListener(SettingsButton_OnClick);
        changeLanguageButton.onClick.RemoveListener(ChangeLanguage_OnClick);
        noAdsButton.onClick.RemoveListener(NoAdsButton_OnClick);
        coinsBoxButton.onClick.RemoveListener(CoinsBoxButton_OnClick);
    }

    #endregion



    #region Content update

    private void UpdateUpgradePrice()
    {
        int nextInteriorUpgradePrice = Env.Instance.Inventory.GetNextInteriorUpgradePrice();
        upgradePrice.text = nextInteriorUpgradePrice.ToString();

        bool inAppsEnabled = BalanceDataProvider.Instance.InAppsEnabled;

        noAdsButton.gameObject.SetActive(!adInfo.IsNoAdsActive && !adInfo.IsSubscriptionActive && inAppsEnabled);
    }


    private void ChangeLanguage_OnClick()
    {
        var localization = (Localization)Localizator;
        var languages = localization.AvailableLanguages;

        var currentLanguage = localization.CurrentLanguage;
        CustomDebug.Log("currentLanguage = ".Color(Color.red) + currentLanguage);

        var index = GetIndexLanguage(currentLanguage, languages);
        CustomDebug.Log("index = ".Color(Color.red) + index);

        index = index + 1 == languages.Count ? 0 : index+1;
        CustomDebug.Log("index = ".Color(Color.red) + index);

        CustomDebug.Log("SetSystemLanguage = ".Color(Color.red) + languages[index]);
        localization.SetSystemLanguage(languages[index]);
    }

	private int GetIndexLanguage(SystemLanguage currentLanguage, List<SystemLanguage> languages)
	{
        int i = 0;
		foreach(var lang in languages)
		{
            if(lang == currentLanguage)
			{
                return i;
			}
            i++;
		}
        return i;
	}

	private void UpdateCoinsBoxButton()
    {
        if (BalanceDataProvider.Instance.IsCoinsBoxEnabled)
        {
            bool isCoinsBoxFull = Env.Instance.Inventory.BucksBox >= BalanceDataProvider.Instance.CoinsBoxMaxAmount;
            coinsBoxButtonScaleBehaviour.IsIdleBlocked = !isCoinsBoxFull;
        }
        else
        {
            coinsBoxButton.gameObject.SetActive(false);
        }
    }


    private void UpdateBubbles()
    {
        if(this == null)
        {
            return;
        }

        bool isCoinsBoxFull = BalanceDataProvider.Instance.IsCoinsBoxEnabled && 
                              BalanceDataProvider.Instance.IsCoinsBoxBubbleEnabled &&
                              Env.Instance.Inventory.BucksBox >= BalanceDataProvider.Instance.CoinsBoxMaxAmount && 
                              UserActivityChecker.Instance.LastCoinsBoxOpenDate < UserActivityChecker.Instance.LastCoinsBoxFullDate;
        
        if (isCoinsBoxFull)
        {
            metagamePiggyBubble.Show(CoinsBoxButton_OnClick);
        }
        else
        {
            metagamePiggyBubble.Hide();
        }
    }

    #endregion



    #region Events handling

    private void StartButton_OnClick()
    {
        eventSystem.enabled = false;
        onPurchasedCallbackEvent = null;

        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        Env.Instance.Sound.FadeOutCurrentlyPlayingMusic();

        CheckPremiumItems();

        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            eventSystem.enabled = true;
            GameplayRoomContext context = new GameplayRoomContext();
            
            Env.Instance.Rooms.SwitchToRoom<GameplayRoom>(true, context, () =>
            {
                overlay.Close();
            });
        });
    }

    void ShopButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        CheckPremiumItems();

        UIMessageBox mbox = Env.Instance.UI.Messages.ShowShopBox();
        mbox.AddOnCloseDelegate(() =>
        {
            newShopItemsIconHelper.Refresh();
            UpdateCanPurchaseIcon();
        });
    }


    void UpgradeButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
        eventSystem.enabled = false;
        upgradeButton.interactable = false;
        
        var price = Env.Instance.Inventory.GetNextInteriorUpgradePrice();
        string itemId  = "none";

        var interiorObjects = Env.Instance.Content.GetItems(ContentAsset.AssetType.InteriorObject).FindAll((element) =>
        {
            return element.Info.Order <= Env.Instance.Inventory.InteriorLevel + 1;
        });
        interiorObjects.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));
        var interiorItem = interiorObjects.Last();
        itemId = interiorItem.Info.Name;

        if (Env.Instance.Inventory.TrySpendBucks(price, category: "sweetshop_upgrade", itemId: itemId))
        {
            eventSystem.enabled = true;
            Env.Instance.Inventory.UpgradeInterior(1);
            int itemNumber = 0;
            if (interiorObjects.Count > 0)
            {
                itemNumber = interiorObjects.IndexOf(interiorItem);
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
                {
                    Env.Instance.UI.Messages.ShowContentReceive(interiorItem.Info, () => MakeBarUpgrade(1));
                    overlay.Close();
                });
            }
			UpdateCanPurchaseIcon();
            int serialNumber = itemNumber - 1;
            var data = new Dictionary<string, object>();
            data["upgrade_number"] = serialNumber;
            data["item_id"] = itemId.Replace(" ", "_");
            data["reason"] = "soft";
            data["value"] = price;
        }
    }

    public void MakeBarUpgrade(int gradesCount)
    {
        if(this == null)
        {
            return;
        }

        Env.Instance.Sound.PlaySound(AudioKeys.UI.BarUpgrade);

        Env.Instance.Rooms.MetagameRoom.Controller.UpInteriorToCurrentGrade(
            () =>
            {
                UpdateUpgradeButton();
            });

        UpdateUpgradePrice();
    }

    void SettingsButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        optionsPanel.Show();
    }



    void NoAdsButton_OnClick()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            PopupManager.Instance.ShowMessagePopup(messageHandler: transform);
            return;
        }
        if (!Services.StoreManager.IsInitialized)
        {
            PopupManager.Instance.ShowMessagePopup(message : "label_store_is_not_initialized".Translate(), messageHandler: transform);
            return;
        }
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        noAdsButton.interactable = false;
        noAdsSpinner.Show();

        onPurchasedCallbackEvent = OnRestorePurchaseCallback;
    }

    private System.Action<IPurchaseItemResult> onPurchasedCallbackEvent = null;

    private void OnRestorePurchaseCallback(IPurchaseItemResult purchaseItemResult)
    {
        onPurchasedCallbackEvent = null;
        noAdsSpinner.Hide();

        if (purchaseItemResult.IsSucceeded)
        {
            Env.Instance.UI.Messages.ShowInfoBox("label_purchase_complete".Translate(), () => { });
        }
        else
        {
            noAdsButton.interactable = true;
        }
    }

    bool NoAds_OnPurchased(IPurchaseItemResult result)
    {
        UpdateUpgradePrice();
        noAdsButton.interactable = true;
        return true;
    }


    void NoAds_OnRestored(IPurchaseItemResult result)
    {
        UpdateUpgradePrice();
    }


    bool WeeklySubscription_OnPurchased(IPurchaseItemResult result)
    {
        UpdateUpgradePrice();
        return true;
    }


    void WeeklySubscription_OnRestored(IPurchaseItemResult result)
    {
        UpdateUpgradePrice();
    }


    void CoinsBoxButton_OnClick()
    {
        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            Env.Instance.UI.Messages.ShowCoinsPiggyBox(() => 
            {
                UpdateCoinsBoxButton();
                UpdateBubbles();
            });

            overlay.Close();
        });
    }


    private void CheckPremiumItems()
    {
        if (adInfo.IsSubscriptionActive)
            return;

        List<ContentItemInfo> premiumCreams = Env.Instance.Content.GetAvailableInfos(ContentAsset.AssetType.CreamSkin, PremiumFilter);
        List<ContentItemInfo> premiumValves = Env.Instance.Content.GetAvailableInfos(ContentAsset.AssetType.Valve, PremiumFilter);

        if (!premiumCreams.IsNullOrEmpty())
        {
            ContentItemInfo premiumCream = premiumCreams.Find((itemInfo) => itemInfo.Name.Equals(Env.Instance.Rules.CreamSkin.Value));
            if (premiumCream != null)
            {
                string lastCream = Env.Instance.Inventory.AvailableCreams.Last();

                if (lastCream != null)
                {
                    if (Env.Instance.Rooms.GameplayRoom != null)
                    {
                        Env.Instance.Rooms.GameplayRoom.Controller.GetComponentInChildren<CreamCreator>(true).CreamSkinName = lastCream;
                    }
                    else
                    {
                        Env.Instance.Rules.CreamSkin.Value = lastCream;
                    }
                }
            }
        }

        if (!premiumValves.IsNullOrEmpty())
        {
            ContentItemInfo premiumValve = premiumValves.Find((itemInfo) => itemInfo.Name.Equals(Env.Instance.Inventory.ValveName));
            if (premiumValve != null)
            {
                string lastValve = Env.Instance.Inventory.AvailableValves.Last();

                if (lastValve != null)
                {
                    if (Env.Instance.Rooms.GameplayRoom != null)
                    {
                        ValveAsset valve = Env.Instance.Content.LoadContentAsset<ValveAsset>(ContentAsset.AssetType.Valve, lastValve);
                        Env.Instance.Rooms.GameplayRoom.Controller.Valve.UpdateSkin(valve);
                    }

                    Env.Instance.Inventory.SetCurrentValve(lastValve);
                }
            }
        }

        Env.Instance.Inventory.Save();
    }


    private bool PremiumFilter(ContentItemInfo itemInfo)
    {
        return Env.Instance.Inventory.IsPremiumItem(itemInfo);
    }


    private int buckCountCached = -1;
    private int nextInteriorUpgradePriceCached = -1;
    private void UpdateUpgradeButton()
    {
        int nextInteriorUpgradePrice = Env.Instance.Inventory.GetNextInteriorUpgradePrice();
        int buckCount = Env.Instance.Inventory.Bucks;

        if (Env.Instance.Inventory.InteriorLevel >= Env.Instance.Rooms.MetagameRoom.Controller.GetMaxBarUpgradeLevel())
        {
            if (upgradeButton.gameObject.activeSelf)
                upgradeButton.gameObject.SetActive(false);

            startButton.JumpsEnable = true;
            return;
        }

        if ((buckCountCached == buckCount) &&
            (nextInteriorUpgradePriceCached == nextInteriorUpgradePrice)) return;

        upgradeButtonAnimationBool.Set(buckCount >= nextInteriorUpgradePrice);

        buckCountCached = buckCount;
        nextInteriorUpgradePriceCached = nextInteriorUpgradePrice;

        upgradeButton.interactable = (nextInteriorUpgradePrice <= buckCount);

        startButton.JumpsEnable = !upgradeButton.interactable;
    }

    private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEnteredBackground)
    {
        if (!isEnteredBackground && UserActivityChecker.Instance.IsCoinsBoxPeriodicRewardAvailable)
        {
            Env.Instance.UI.Messages.ShowCoinsPiggyRewardBox(() => 
            {
                UpdateCoinsBoxButton();
                UpdateBubbles();
            });
        }
    }

    #endregion
}
