using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using BoGD;
using TMPro;

public class ShopTabsHandler : MonoBehaviour
{
    #region Nested types

    [Serializable]
    public class ShopTabSettings
    {
        public ContentAsset.AssetType assetsType;
        public ShopTabButtonHandler buttonHandler;
        public string title;
    }

    #endregion



    #region Fields

    [SerializeField] TextMeshProUGUI tabTitle;
    [SerializeField] ShopItemsScroll itemsScroll;

    [Space]
    [Header("Tabs settings")]
    [SerializeField] ContentAsset.AssetType initialTabType;
    [SerializeField] List<ShopTabSettings> tabsSettings = new List<ShopTabSettings>();

    [Header("Items settings")]
    [SerializeField] List<ContentAsset.AssetType> bannedOpenningTypes = new List<ContentAsset.AssetType>();

    public event Action<ShopTabSettings> OnTabChanged;

    ShopTabSettings currentTabSettings = null;

    public HashSet<ContentAsset.AssetType> ShowedAssetTypes { get; } = new HashSet<ContentAsset.AssetType>();

    #endregion



    #region Properties

    public ShopTabSettings CurrentTab
    {
        get
        {
            return currentTabSettings;
        }
    }


    public ShopItemsScroll Scroll => itemsScroll;

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        ShopItemNotificationInfo.OnHighlightNewChanged += ShopItemNotificationInfo_OnHighlightNewChanged;
    }


    void OnDisable()
    {
        ShopItemNotificationInfo.OnHighlightNewChanged -= ShopItemNotificationInfo_OnHighlightNewChanged;
    }

    #endregion



    #region Initialization

    public void Init(ContentItemInfo itemToHighlight = null)
    {
        foreach (var tabSettings in tabsSettings)
        {
            tabSettings.buttonHandler.Button.onClick.AddListener(() => { TabButton_OnClick(tabSettings); });
        }

        if (currentTabSettings == null)
        {
            if (itemToHighlight != null)
            {
                ShowItem(itemToHighlight);
            }
            else
            {
                ChangeTab(tabsSettings.Find((tabSettings) => 
                {
                    return tabSettings.assetsType == initialTabType;
                }));
            }
        }

        UpdateTabsButtons();
    }

    #endregion



    #region Content update handling
    private Guid? cellsSpawnSound = null;
    void ChangeTab(ShopTabSettings tabSettings)
    {
        if (currentTabSettings != tabSettings)
        {
            cellsSpawnSound = Env.Instance.Sound.PlaySound(AudioKeys.UI.ShopRespanwObjects);
        }

        ShowedAssetTypes.Add(tabSettings.assetsType);

        tabTitle.text = tabSettings.title.Translate();

        if (currentTabSettings != null)
        {
            currentTabSettings.buttonHandler.State = ShopTabButtonState.Inactive;
        }

        currentTabSettings = tabSettings;
        currentTabSettings.buttonHandler.State = ShopTabButtonState.Active;
        OnTabChanged?.Invoke(currentTabSettings);

        List<ContentItemInfo> availableItems = GetItemsInfosForTab(CurrentTab, (itemInfo) => 
        {
            return !itemInfo.CannotBeReceived;
        });

        // premium items at first
        List<ContentItemInfo> premiumItems = availableItems.FindAll(item => Env.Instance.Inventory.IsPremiumItem(item));
        if (null != premiumItems)
        {
            foreach (ContentItemInfo premiumItem in premiumItems)
            {
                availableItems.Remove(premiumItem);
            }
            foreach (ContentItemInfo premiumItem in premiumItems)
            {
                availableItems.Insert(0, premiumItem);
            }
        }

        itemsScroll.Init(availableItems);

        UpdateTabsButtons();
    }


    void UpdateTabsButtons()
    {
        foreach (var tabSettings in tabsSettings)
        {
            bool hasNewItems = Env.Instance.Inventory.IsHasNewItems(tabSettings.assetsType);
            tabSettings.buttonHandler.UpdateContent(hasNewItems);
        }
    }


    public ShopItem ShowItem(ContentItemInfo itemInfo, Action onComplete = null)
    {
        ShopTabSettings itemTab = FindTabForItem(itemInfo);

        bool isTabChangeRequired = (itemTab != currentTabSettings);
        if (isTabChangeRequired)
        {
            ChangeTab(itemTab);
        }
        else
        {
            UpdateTabsButtons();
        }

        return itemsScroll.ShowItem(itemInfo, isTabChangeRequired, onComplete);
    }

    #endregion


    private void OnDestroy()
    {
        if (Env.Instance.Sound.InstanceIfExist && 
            cellsSpawnSound.HasValue &&
            Env.Instance.Sound.IsPlaybackActive(cellsSpawnSound.Value))
        {
            Env.Instance.Sound.StopSound(cellsSpawnSound.Value);
        }
    }


    #region Items opening

    public bool TryOpenRandomItem(Action onComplete = null)
    {
        ContentItemInfo itemToOpen = GetRandomItemToOpen();

        if (itemToOpen != null && Env.Instance.Inventory.TryReceiveItem(itemToOpen.AssetType, itemToOpen.Name, false))
        {
            ShowItem(itemToOpen, onComplete).UpdateContent();
            Env.Instance.Inventory.SendCustomizeEvent(itemToOpen.Name, itemToOpen.AssetType.ToString().ToLower(), "soft_random", BalanceDataProvider.Instance.ItemCoinsPrice);
            return true;
        }

        onComplete?.Invoke();
        return false;
    }


    public bool CanOpenItem()
    {
        return GetAllAvailableToOpenItems().Count > 0;
    }

    #endregion



    #region Content delivery

    List<ContentItemInfo> GetItemsInfosForTab(ShopTabSettings tabSettings, Predicate<ContentItemInfo> filter = null)
    {
        HashSet<string> inventoryItems = new HashSet<string>(Env.Instance.Inventory.GetAvailableItems(tabSettings.assetsType));

        List<ContentItemInfo> allItemsOfType = Env.Instance.Content.GetAvailableInfos(tabSettings.assetsType, filter);

        Dictionary<string, ContentItemInfo> allItemsOfTypeMap = new Dictionary<string, ContentItemInfo>();
        foreach (ContentItemInfo cii in allItemsOfType)
        {
            allItemsOfTypeMap.Add(cii.Name, cii);
        }

        List<ContentItemInfo> resultItemsList = new List<ContentItemInfo>();

        // add known items
        foreach (string itemName in inventoryItems)
        {
            if (!allItemsOfTypeMap.ContainsKey(itemName)) continue;
            resultItemsList.Add(allItemsOfTypeMap[itemName]);
        }

        // add other items
        foreach (ContentItemInfo cii in allItemsOfType)
        {
            if (inventoryItems.Contains(cii.Name)) continue;
            resultItemsList.Add(cii);
        }

        return resultItemsList;
    }


    List<ContentItemInfo> GetAllAvailableToOpenItems()
    {
        List<ContentItemInfo> result = new List<ContentItemInfo>();

        foreach (var tabSettings in tabsSettings)
        {
            List<ContentItemInfo> availableItemsForTab = GetItemsInfosForTab(tabSettings, (itemInfo) => 
            {
                return !Env.Instance.Inventory.IsPremiumItem(itemInfo) &&
                       !itemInfo.CannotBeReceived && 
                       !Env.Instance.Inventory.IsItemAvailable(itemInfo.AssetType, itemInfo.Name) && 
                       !bannedOpenningTypes.Contains(itemInfo.AssetType);
            });

            // no need random! use first item only
            if ((null != availableItemsForTab) && (availableItemsForTab.Count > 0))
            {
                result.Add(availableItemsForTab.First());
            }
        }

        return result;
    }


    ContentItemInfo GetRandomItemToOpen()
    {
        List<ContentItemInfo> availableToOpenItems = GetAllAvailableToOpenItems();
        return (availableToOpenItems.Count > 0) ? availableToOpenItems.RandomObject() : null;
    }


    ShopTabSettings FindTabForItem(ContentItemInfo itemInfo)
    {
        return tabsSettings.Find((tabSettings) => 
        {
            return tabSettings.assetsType == itemInfo.AssetType;
        });
    }

    #endregion



    #region Events handling

    void TabButton_OnClick(ShopTabSettings tabSettings)
    {
        if (tabSettings != null && CurrentTab != tabSettings)
        {
            if (OptionsPanel.IsVibroEnabled)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            ChangeTab(tabSettings);
        }
    }


    void ShopItemNotificationInfo_OnHighlightNewChanged(bool shouldHighlight)
    {
        UpdateTabsButtons();
    }

    #endregion
}
