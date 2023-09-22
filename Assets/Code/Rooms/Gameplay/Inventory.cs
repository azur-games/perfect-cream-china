using BoGD;
using Modules.Advertising;
using Modules.General;
using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public static string INVENTORY_PREFS_KEY = "inventory";

    [System.Serializable]
    private class Data
    {
        public int CurrentLevelIndex;
        public int Coins;
        public int Cakes;
        public int Stars;
        public int InteriorLevel;
        public int CoinsBox;

        public int CurrentLevelPickedUpCoins;
        public int CurrentLevelPickedUpKeys;
        public int CurrentLevelFeverCompletions;

        public string CurrentValve;
        public List<string> AvailableValves;

        public List<string> AvailableShapes;
        public List<string> AvailableCreams;

        public string CurrentConfiture;
        public List<string> AvailableConfiture;

        public bool PremiumWorksForThisUser;

        public string LastReceiveItem;
    }

    private Data _data;
    public int Bucks { get { return _data.Coins; } private set { _data.Coins = value; } }
    public int BucksBox { get { return _data.CoinsBox; } private set { _data.CoinsBox = value; } }
    public int Keys { get { return _data.Cakes; } private set { _data.Cakes = value; } }
    public int Stars { get { return _data.Stars; } private set { _data.Stars = value; } }
    public int InteriorLevel { get { return _data.InteriorLevel; } private set { _data.InteriorLevel = value; } }
    public bool PremiumWorksForThisUser { get { return _data.PremiumWorksForThisUser; } private set { _data.PremiumWorksForThisUser = value; } }

    public int CurrentLevelIndex 
    { 
        get { return _data.CurrentLevelIndex; } 
        set 
        {
            if (value != _data.CurrentLevelIndex)
            {
                _data.CurrentLevelPickedUpCoins = 0;
                _data.CurrentLevelPickedUpKeys = 0;
                _data.CurrentLevelFeverCompletions = 0;
                
                OnLevelChanged?.Invoke();
            }

            _data.CurrentLevelIndex = value; 
        } 
    }

    public int CurrentLevelPickedUpCoins { get { return _data.CurrentLevelPickedUpCoins; } set { _data.CurrentLevelPickedUpCoins = value; } }
    public int CurrentLevelPickedUpKeys { get { return _data.CurrentLevelPickedUpKeys; } set { _data.CurrentLevelPickedUpKeys = value; } }
    public int CurrentLevelFeverCompletions { get { return _data.CurrentLevelFeverCompletions; } set { _data.CurrentLevelFeverCompletions = value; } }
    public List<string> AvailableShapes { get { return _data.AvailableShapes; } private set { _data.AvailableShapes = value; } }
    public List<string> AvailableCreams { get { return _data.AvailableCreams; } private set { _data.AvailableCreams = value; } }
    public List<string> AvailableValves { get { return _data.AvailableValves; } private set { _data.AvailableValves = value; } }
    public List<string> AvailableConfiture { get { return _data.AvailableConfiture; } private set { _data.AvailableConfiture = value; } }
    public string LastReceiveItem { get { return _data.LastReceiveItem; } private set { _data.LastReceiveItem = value; } }
    public string ValveName { get { return _data.CurrentValve; } private set { _data.CurrentValve = value; } }
    public string CurrentConfiture { get { return _data.CurrentConfiture; } private set { _data.CurrentConfiture = value; } }

    public ContentDelivery Delivery;

    public event Action OnLevelChanged;
    public event Action<int, Transform, Action> OnBucksCountUpdated;
    public event Action<int, Transform, Action> OnStarsCountUpdated;
    public event Action<int, Transform, Action> OnKeysCountUpdated;
    public event Action<int, Transform, Action> OnBucksBoxValueUpdated;
    public event Action<string> OnShapeReceived;

    public NewItemsDetectionHelper NewItemsDetector { get; private set; } = new NewItemsDetectionHelper();

    private bool isFirstLounch = false;

    public static Inventory Create()
    {
        Data data = CustomPlayerPrefs.HasKey(INVENTORY_PREFS_KEY) ? JsonUtility.FromJson<Data>(CustomPlayerPrefs.GetString(INVENTORY_PREFS_KEY)) : null;
        Inventory inventory = new Inventory(data);
        return inventory;
    }

    public bool Contains(string itemName)
    {
        if (AvailableShapes.Contains(itemName)) return true;
        if (AvailableCreams.Contains(itemName)) return true;
        if (AvailableValves.Contains(itemName)) return true;
        if (AvailableConfiture.Contains(itemName)) return true;
        return false;
    }

    private void FillStartData()
    {
        if (_data.AvailableShapes == null)
            _data.AvailableShapes = new List<string>();

        if (_data.AvailableCreams == null)
        {
            _data.AvailableCreams = new List<string>() { "WhiteStar" }; //white star cream by default on start
        }

        if (_data.AvailableValves == null)
        {
            _data.AvailableValves = new List<string>() { "v0" }; //first valve by default on start
            _data.CurrentValve = "v0";
        }

        if (_data.AvailableConfiture == null)
        {
            AvailableConfiture = new List<string>();
            _data.CurrentConfiture = "";
        }

        if (isFirstLounch)
        {
            _data.PremiumWorksForThisUser = true;
        }

        //var startShapes = new List<string>() { "waffle_3", "waffle_5", "waffle_7", "waffle_9", "waffle_11", "cake_long", "cake_short", "pancake_long", "pancake_short" };
        var startShapes = new List<string>() { "waffle_3", "waffle_5", "waffle_9", "waffle_11", "cake_9", "cake_long", "cake_short", "cake_7", "waffle_7" };

        foreach (var shape in startShapes)
        {
            if (_data.AvailableShapes.Contains(shape))
                continue;

            _data.AvailableShapes.Add(shape);
        }

        bool findNewItems = false;
        findNewItems |= NewItemsDetector.AddKnown(AvailableShapes);
        findNewItems |= NewItemsDetector.AddKnown(AvailableCreams);
        findNewItems |= NewItemsDetector.AddKnown(AvailableValves);
        findNewItems |= NewItemsDetector.AddKnown(AvailableConfiture);

        if (findNewItems)
        {
            NewItemsDetector.SaveKnownKeys();
        }
    }

    public void UnlockAll(ContentAsset.AssetType assetType)
    {
        foreach (ContentItemsLibrary.ContentItemsCollectionElement el in Env.Instance.Content.GetItems(assetType))
        {
            string itemName = el.Info.Name;
            if (Contains(itemName)) continue;

            switch (assetType)
            {
                case ContentAsset.AssetType.Confiture:
                    AvailableConfiture.Add(itemName);
                    break;

                case ContentAsset.AssetType.Valve:
                    AvailableValves.Add(itemName);
                    break;

                case ContentAsset.AssetType.Shape:
                    ReceiveShape(itemName, false, false);
                    break;

                case ContentAsset.AssetType.CreamSkin:
                    AvailableCreams.Add(itemName);
                    break;
            }
        }

        Save();
    }

    private Inventory(Data data)
    {
        isFirstLounch = (null == data);

        _data = isFirstLounch ? (new Data()) : data;

        FillStartData();

        Delivery = new ContentDelivery();
    }

    public void AddBucks(int amount, Transform addAnimationRoot = null, Action callback = null, string category = "", string itemId = "")
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Coins);

        Bucks += amount;
        Save();

        OnBucksCountUpdated?.Invoke(amount, addAnimationRoot, callback);

        var data = new Dictionary<string, object>();
        itemId = itemId.ToLower().Replace(' ', '_');
        data["category"] = category;
        data["item_id"] = itemId;
        data["value"] = amount;
        BoGD.MonoBehaviourBase.Analytics.SendEvent("soft_currency", data);
    }

    public bool TrySpendBucks(int amount, Transform animationRoot = null, Action callback = null, string category = "", string itemId = "")
    {
        if (Bucks < amount)
            return false;

        Bucks -= amount;
        Save();

        OnBucksCountUpdated?.Invoke(-amount, animationRoot, callback);
        itemId = itemId.ToLower().Replace(' ', '_');
        var data = new Dictionary<string, object>();
        data["category"] = category;
        data["item_id"] = itemId;
        data["value"] = -amount;
        BoGD.MonoBehaviourBase.Analytics.SendEvent("soft_currency", data);

        return true;
    }

    public void AddStars(int amount, Transform animationRoot = null, Action callback = null)
    {
        Stars += amount;
        Save();

        OnStarsCountUpdated?.Invoke(amount, animationRoot, callback);
    }

    public bool TrySpendStars(int amount, Transform animationRoot = null, Action callback = null)
    {
        if (Stars < amount)
            return false;

        Stars -= amount;
        Save();

        OnStarsCountUpdated?.Invoke(-amount, animationRoot, callback);

        return true;
    }

    public void AddKeys(int amount, Transform animationRoot = null, Action callback = null)
    {
        Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.KeyCollect);

        Keys = Mathf.Clamp(Keys + amount, 0, 3);
        Save();

        OnKeysCountUpdated?.Invoke(amount, animationRoot, callback);
    }

    public bool TrySpendKeys(int amount, Transform animationRoot = null, Action callback = null)
    {
        if (Keys < amount)
            return false;

        Keys -= amount;
        Save();

        OnKeysCountUpdated?.Invoke(-amount, animationRoot, callback);

        return true;
    }

    public void IncreaseBucksBoxValue(int value, Transform animationRoot = null, Action callback = null)
    {
        bool notFull = BucksBox < BalanceDataProvider.Instance.CoinsBoxMaxAmount;
        BucksBox = Mathf.Clamp(BucksBox + value, 0, BalanceDataProvider.Instance.CoinsBoxMaxAmount);
        Save();

        OnBucksBoxValueUpdated?.Invoke(value, animationRoot, callback);

        bool isCoinsBoxFull = notFull && BucksBox >= BalanceDataProvider.Instance.CoinsBoxMaxAmount;
        if (isCoinsBoxFull)
        {
            var data = new Dictionary<string, object>();
            data["action"] = "full";
            BoGD.MonoBehaviourBase.Analytics.SendEvent("piggy_bank", data);
        }
    }

    public bool TryRecieveBucksBoxValue(int multiplier, Transform animationRoot = null, Action callback = null)
    {
        if (BucksBox < BalanceDataProvider.Instance.CoinsBoxMaxAmount)
            return false;

        int value = BucksBox;

        BucksBox = 0;
        AddBucks(multiplier * value, animationRoot, category: "reward", itemId: "piggy_bank");

        OnBucksBoxValueUpdated?.Invoke(-value, animationRoot, callback);

        return true;
    }

    public int GetNextInteriorUpgradePrice()
    {
        return BalanceDataProvider.Instance.CafeUpgradeStartPrice + BalanceDataProvider.Instance.CafeUpgradeDeltaPrice * InteriorLevel;
    }

    public void SetInteriorGrade(int grade)
    {
        InteriorLevel = grade;
        Save();
    }

    public void UpgradeInterior(int gradesCount)
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.UpgradeDevice);

        InteriorLevel += gradesCount;
        Save();
    }
    
    public void SetCurrentCream(string creamName, bool highlight = false)
    {
        if (!highlight)
        {
            SendCustomizeEvent(creamName, ContentAsset.AssetType.CreamSkin.ToString().ToLower(), "owned", creamName == "WhiteStar"? 0 : BalanceDataProvider.Instance.ItemCoinsPrice);
        }
    }

    public void SetCurrentValve(string valveName, bool highlight = false)
    {
        ValveName = valveName;
        if (!highlight)
        {
            SendCustomizeEvent(valveName, ContentAsset.AssetType.Valve.ToString().ToLower(), "owned", valveName == "v0"? 0 : BalanceDataProvider.Instance.ItemCoinsPrice);
        }
    }

    public void ApplyValve(string valveName)
    {
        bool available = true;
        if (!AvailableValves.Contains(valveName))
        {
            PlayUnlockedSound(false);

            AvailableValves.Add(valveName);
            available = false;
        }

        LastReceiveItem = valveName;
        ValveName = valveName;
        Save();

        var category = ContentAsset.AssetType.Valve.ToString().ToLower();
        var reason =  available? "owned" : "lootbox";
        var price = valveName == "v0" ? 0 : BalanceDataProvider.Instance.ItemCoinsPrice;
        SendCustomizeEvent(valveName, category, reason, price);
    }

    public string GetLastAvailableShape()
    {
        return AvailableShapes.Last();
    }

    public void SetItemsKnown(params ContentAsset.AssetType[] assetTypes)
    {
        bool findNewItems = false;
        foreach (ContentAsset.AssetType assetType in assetTypes)
        {
            switch (assetType)
            {
                case ContentAsset.AssetType.Confiture:
                    findNewItems = NewItemsDetector.AddKnown(AvailableConfiture);
                    break;

                case ContentAsset.AssetType.CreamSkin:
                    findNewItems = NewItemsDetector.AddKnown(AvailableCreams);
                    break;

                case ContentAsset.AssetType.Shape:
                    findNewItems = NewItemsDetector.AddKnown(AvailableShapes);
                    break;

                case ContentAsset.AssetType.Valve:
                    findNewItems = NewItemsDetector.AddKnown(AvailableValves);
                    break;
            }
        }

        if (findNewItems)
        {
            NewItemsDetector.SaveKnownKeys();
        }
    }

    public bool IsHasNewItems(params ContentAsset.AssetType[] assetTypes)
    {
        foreach (ContentAsset.AssetType assetType in assetTypes)
        {
            switch (assetType)
            {
                case ContentAsset.AssetType.Confiture:
                    if (NewItemsDetector.IsHasUnknowns(AvailableConfiture)) return true;
                    break;

                case ContentAsset.AssetType.CreamSkin:
                    if (NewItemsDetector.IsHasUnknowns(AvailableCreams)) return true;
                    break;

                case ContentAsset.AssetType.Shape:
                    if (NewItemsDetector.IsHasUnknowns(AvailableShapes)) return true;
                    break;

                case ContentAsset.AssetType.Valve:
                    if (NewItemsDetector.IsHasUnknowns(AvailableValves)) return true;
                    break;
            }
        }

        return false;
    }

    public void ReceiveShape(string shapeName, bool needCallback = true, bool needSave = true, bool isAds = false)
    {
        // insert shapes same subcategory
        string shapeItemSubcategory = Env.Instance.Content.GetItem(ContentAsset.AssetType.Shape, shapeName).Info.SubCategory;
        if (!string.IsNullOrEmpty(shapeItemSubcategory))
        {
            List<ContentItemsLibrary.ContentItemsCollectionElement> subcategoryItems = Env.Instance.Content.GetItemsOfSubcategory(ContentAsset.AssetType.Shape, shapeItemSubcategory);
            foreach (ContentItemsLibrary.ContentItemsCollectionElement subcategoryItem in subcategoryItems)
            {
                string subcategoryShapeName = subcategoryItem.Info.Name;
                if (subcategoryShapeName == shapeName) continue;

                if (!AvailableShapes.Contains(subcategoryShapeName))
                {
                    AvailableShapes.Add(subcategoryShapeName);
                }
            }
        }

        // and add item itself
        if (!AvailableShapes.Contains(shapeName))
        {
            PlayUnlockedSound(true);

            AvailableShapes.Add(shapeName);
            if (needCallback)
            {
                OnShapeReceived?.Invoke(shapeName);
            }

            SendCustomizeEvent(shapeName, "food", isAds ? "ad_watched" : "unlock", 0);
        }

        //LastReceiveItem = shapeName;

        if (needSave) Save();
    }

    public void SetCurrentConfiture(string confitureItemName, bool highlight = false)
    {
        CurrentConfiture = confitureItemName;
        Save();

        if (!highlight)
        {
            SendCustomizeEvent(confitureItemName, ContentAsset.AssetType.Confiture.ToString().ToLower(), "owned", BalanceDataProvider.Instance.ItemCoinsPrice);
        }
    }

    public void ReceiveConfiture(string confitureItemName)
    {
        bool available = true;
        if (!AvailableConfiture.Contains(confitureItemName))
        {
            PlayUnlockedSound(false);

            AvailableConfiture.Add(confitureItemName);
            available = false;
        }

        LastReceiveItem = confitureItemName;

        CurrentConfiture = confitureItemName;
        Save();

        var category = ContentAsset.AssetType.Confiture.ToString().ToLower();
        var reason =  available? "owned" : "lootbox";
        var price = BalanceDataProvider.Instance.ItemCoinsPrice;
        SendCustomizeEvent(confitureItemName, category, reason, price);
    }

    public void ReceiveCream(string creamName)
    {
        bool available = true;
        if (!AvailableCreams.Contains(creamName))
        {
            PlayUnlockedSound(false);

            AvailableCreams.Add(creamName);
            available = false;
        }

        LastReceiveItem = creamName;

        Save();

        var category = ContentAsset.AssetType.CreamSkin.ToString().ToLower();
        var reason =  available? "owned" : "lootbox";
        var price = creamName == "WhiteStar" ? 0 : BalanceDataProvider.Instance.ItemCoinsPrice;
        SendCustomizeEvent(creamName, category,reason, price);
    }


    public bool IsItemAvailable(ContentAsset.AssetType assetType, string name)
    {
        List<string> availableItemsOfType = GetAvailableItems(assetType);
        return availableItemsOfType != null && availableItemsOfType.Contains(name);
    }


    public bool TryReceiveItem(ContentAsset.AssetType assetType, string name, bool isSoundRequired = true)
    {
        List<string> availableItemsOfType = GetAvailableItems(assetType);

        if (availableItemsOfType != null && !availableItemsOfType.Contains(name))
        {
            if (isSoundRequired)
            {
                PlayUnlockedSound(assetType == ContentAsset.AssetType.Shape);
            }

            availableItemsOfType.Add(name);

            LastReceiveItem = name;

            Save();

            return true;
        }

        return false;
    }


    public List<string> GetAvailableItems(ContentAsset.AssetType assetType)
    {
        switch (assetType)
        {
            case ContentAsset.AssetType.Shape:
                return AvailableShapes;

            case ContentAsset.AssetType.Valve:
                return AvailableValves;

            case ContentAsset.AssetType.CreamSkin:
                return AvailableCreams;

            case ContentAsset.AssetType.Confiture:
                return AvailableConfiture;

            default:
                return null;
        }
    }

    public void GetAllContent()
    {
        foreach (ContentAsset.AssetType assetType in Enum.GetValues(typeof(ContentAsset.AssetType)))
        {
            if (assetType == ContentAsset.AssetType.None) continue;
            if (assetType == ContentAsset.AssetType.Level) continue;
            if (assetType == ContentAsset.AssetType.InteriorObject) continue;

            List<string> availableItemsOfThisType = GetAvailableItems(assetType);

            foreach (ContentItemInfo contentItemInfo in Env.Instance.Content.GetAvailableInfos(assetType))
            {
                if (!availableItemsOfThisType.Contains(contentItemInfo.Name))
                {
                    availableItemsOfThisType.Add(contentItemInfo.Name);
                }
            }
        }

        Save();
    }

    // need to restart application after this function
    public void ResetToDefault()
    {
        _data.AvailableConfiture = null;
        _data.AvailableCreams = null;
        _data.AvailableShapes = null;
        _data.AvailableValves = null;

        FillStartData();
        Env.Instance.Rules.CreamSkin.Value = _data.AvailableCreams[0];

        Save();
    }

    public void Save()
    {
        CustomPlayerPrefs.SetString(INVENTORY_PREFS_KEY, JsonUtility.ToJson(_data));
    }

    public ContentItemsLibrary.ContentItemsCollectionElement GetNext(ContentAsset.AssetType assetType)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> items = Env.Instance.Content.GetItems(assetType)
                                                                                            .FindAll((element) => !Contains(element.Info.Name) &&
                                                                                                                  !element.Info.CannotBeReceived &&
                                                                                                                  !IsPremiumItem(element.Info));
        if (0 == items.Count) return null;

        items.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        return items.First();
    }

    public int GetNextShapeStarsPrice()
    {
        ContentItemsLibrary.ContentItemsCollectionElement nextShape = GetNext(ContentAsset.AssetType.Shape);
        if (null == nextShape) return -1;

        if (nextShape.Info.PriceOverride > 0)
        {
            return nextShape.Info.PriceOverride;
        }

        return BalanceDataProvider.Instance.StarsForDesertUnlock;
    }

    public bool CanPurchaseSomething(params ContentAsset.AssetType[] assetTypes)
    {
        int needMoneyForPurchase = BalanceDataProvider.Instance.ItemCoinsPrice;
        if (Env.Instance.Inventory.Bucks < needMoneyForPurchase)
        {
            // not enough
            return false;
        }

        foreach (ContentAsset.AssetType assetType in assetTypes)
        {
            if (CanPurchaseSomething(assetType)) return true;
        }

        return false;
    }

    private bool CanPurchaseSomething(ContentAsset.AssetType assetType) // without checking money
    {
        HashSet<string> available = new HashSet<string>(GetAvailableItems(assetType));

        foreach (ContentItemsLibrary.ContentItemsCollectionElement element in Env.Instance.Content.GetItems(assetType))
        {
            if (element.Info.CannotBeReceived) continue;
            if (IsPremiumItem(element.Info)) continue;
            if (available.Contains(element.Info.Name)) continue;
            return true;
        }

        return false;
    }

    public bool IsPremiumItem(ContentItemInfo itemInfo)
    {
        if (!PremiumWorksForThisUser)
        {
            return false;
        }
        return BalanceDataProvider.Instance.IsSubscriptionAvailable && itemInfo.IsPremiumItem;
    }

    public bool IsNextExtraLevel()
    {
        int extraLevelPeriod = BalanceDataProvider.Instance.ExtraLevelsPeriod;

        return ((Env.Instance.Inventory.CurrentLevelIndex > 0) &&
            (0 == ((Env.Instance.Inventory.CurrentLevelIndex + 1) % extraLevelPeriod)));
    }

    private void PlayUnlockedSound(bool isUnlockedDessert)
    {
        string audioKey = isUnlockedDessert ? AudioKeys.UI.DessertUnlock : AudioKeys.UI.Unlocked2;
        Env.Instance.Sound.PlaySound(audioKey);
    }

    public void SendCustomizeEvent(string id, string category, string reason, int value)
    {
        var data = new Dictionary<string, object>();
        data["item_id"] = id.ToLower().Replace(' ', '_');
        data["category"] = category;

        data["reason"] = reason;
        data["value"] = value;
        MonoBehaviourBase.Analytics.SendEvent("customize", data);
    }
}
