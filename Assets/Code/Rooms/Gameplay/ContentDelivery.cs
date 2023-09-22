using System.Collections.Generic;

public class ContentDelivery
{
    private static readonly List<string> predefinedItems = new List<string>() { "choco", "Hubabuba" };

    private static readonly Dictionary<string, int> predefinedStarPrices = new Dictionary<string, int>()
    {
        { predefinedItems[0], 3 },
        { predefinedItems[1], 4 },
    };

    private string ponyPrizeName = "pony";

    public ContentAsset GetPrize(bool nineCheshsScreen)
    {
        if (nineCheshsScreen &&
            Env.Instance.Inventory.Contains(predefinedItems[0]) &&
            !Env.Instance.Inventory.Contains(ponyPrizeName))
        {
            return Env.Instance.Content.LoadContentAsset<ValveAsset>(ContentAsset.AssetType.Valve, ponyPrizeName);
        }

        var predefined = TryGetPredefined();
        if (predefined)
            return predefined;

        var prizes = GetPrizes(1);
        return prizes.Count > 0 ? prizes[0] : null;
    }

    
    public List<ContentAsset> GetPrizes(int count)
    {
        ContentAsset.AssetType itemTypeToReceive =
            Env.Instance.Content.GetItemsType(Env.Instance.Inventory.LastReceiveItem);
        if (itemTypeToReceive == ContentAsset.AssetType.None)
        {
            itemTypeToReceive = ContentAsset.AssetType.Shape;
        }

        List<string> ownedValves = new List<string>(Env.Instance.Inventory.AvailableValves);
        List<string> ownedCreams = new List<string>(Env.Instance.Inventory.AvailableCreams);
        List<string> ownedConfitures = new List<string>(Env.Instance.Inventory.AvailableConfiture);

        int totalValves = Env.Instance.Content.GetItemsCount(ContentAsset.AssetType.Valve);
        int totalCreams = Env.Instance.Content.GetItemsCount(ContentAsset.AssetType.CreamSkin);
        int totalConfitures = Env.Instance.Content.GetItemsCount(ContentAsset.AssetType.Confiture);

        bool hasFailedToObtainValve = false;
        bool hasFailedToObtainCream = false;
        bool hasFailedToObtainConfiture = false;
        
        List<ContentAsset> prizes = new List<ContentAsset>(count);

        while ((count > 0) && 
            ((ownedValves.Count < totalValves && !hasFailedToObtainValve) ||
             (ownedCreams.Count < totalCreams && !hasFailedToObtainCream) ||
             (ownedConfitures.Count < totalConfitures && !hasFailedToObtainConfiture)))
        {
            ContentAsset asset = null;
            
            switch (itemTypeToReceive)
            {
                case ContentAsset.AssetType.CreamSkin:
                case ContentAsset.AssetType.Shape:
                    itemTypeToReceive = ContentAsset.AssetType.Valve;
                    asset = TryGetNew(itemTypeToReceive, ownedValves);
                    
                    if (asset != null)
                    {
                        ownedValves.Add(asset.Name);
                    }
                    
                    hasFailedToObtainValve = asset == null;
                    break;
                
                case ContentAsset.AssetType.Valve:
                    itemTypeToReceive = ContentAsset.AssetType.Confiture;
                    asset = TryGetNew(itemTypeToReceive, ownedConfitures);
                    
                    if (asset != null)
                    {
                        ownedConfitures.Add(asset.Name);
                    }

                    hasFailedToObtainConfiture = asset == null;
                    break;
                
                case ContentAsset.AssetType.Confiture:
                    itemTypeToReceive = ContentAsset.AssetType.CreamSkin;
                    asset = TryGetNew(itemTypeToReceive, ownedCreams);
                    
                    if (asset != null)
                    {
                        ownedCreams.Add(asset.Name);
                    }

                    hasFailedToObtainCream = asset == null;
                    break;
            }

            if (asset == null)
            {
                continue;
            }
            prizes.Add(asset);
            count--;
        }
        
        return prizes;
    }

    
    public void ApplyPrize(ContentAsset prize)
    {
        if (prize is CreamSkinAsset)
        {
            if (null != Env.Instance.Rooms.GameplayRoom)
            {
                Env.Instance.Rooms.GameplayRoom.Controller.GetComponentInChildren<CreamCreator>(true).CreamSkinName = prize.Name;
            }
            else
            {
                Env.Instance.Rules.CreamSkin.Value = prize.Name;
            }

            Env.Instance.Inventory.ReceiveCream(prize.Name);
        }
        else if (prize is ValveAsset)
        {
            Env.Instance.Inventory.ApplyValve(prize.Name);
        }
        else if (prize is Shape)
        {
            ApplyShape(prize.Name);
        }
        else if (prize is ConfitureAsset)
        {
            Env.Instance.Inventory.ReceiveConfiture(prize.Name);
        }
    }


    public void ApplyPrize(ContentItemInfo prizeInfo)
    {
        if (prizeInfo.AssetType == ContentAsset.AssetType.CreamSkin)
        {
            if (null != Env.Instance.Rooms.GameplayRoom)
            {
                Env.Instance.Rooms.GameplayRoom.Controller.GetComponentInChildren<CreamCreator>(true).CreamSkinName = prizeInfo.Name;
            }
            else
            {
                Env.Instance.Rules.CreamSkin.Value = prizeInfo.Name;
            }

            Env.Instance.Inventory.ReceiveCream(prizeInfo.Name);
        }
        else if (prizeInfo.AssetType == ContentAsset.AssetType.Valve)
        {
            Env.Instance.Inventory.ApplyValve(prizeInfo.Name);
        }
        else if (prizeInfo.AssetType == ContentAsset.AssetType.Shape)
        {
            ApplyShape(prizeInfo.Name);
        }
        else if (prizeInfo.AssetType == ContentAsset.AssetType.Confiture)
        {
            Env.Instance.Inventory.ReceiveConfiture(prizeInfo.Name);
        }
    }


    public void ApplyShape(string shape)
    {
        Env.Instance.Inventory.ReceiveShape(shape);
    }

    private ContentAsset TryGetNew(ContentAsset.AssetType assetType, List<string> alreadyOwned)
    {
        return Env.Instance.Content.LoadContentItem(assetType,
            (asset) =>
            {
                if (asset.CannotBeReceived) return false;
                if (Env.Instance.Inventory.IsPremiumItem(asset)) return false;
                if (alreadyOwned.Contains(asset.Name)) return false;
                return true;
            })?.Asset;
    }


    private ContentAsset TryGetPredefined()
    {
        foreach (string predefinedItem in predefinedItems)
        {
            if (!Env.Instance.Inventory.Contains(predefinedItem))
            {
                return Env.Instance.Content.LoadContentAsset<ContentAsset>(ContentAsset.AssetType.None, predefinedItem);
            }
        }

        return null;
    }
}
