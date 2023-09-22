using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentItemInfo
{
    #region Fields

    [SerializeField] private string name = null;
    [SerializeField] private bool cannotBeReceived;
    [SerializeField] private bool isPremiumItem;
    [SerializeField] private int priceOverride;
    [SerializeField] private ContentAsset.AssetType assetType = ContentAsset.AssetType.None;
    [SerializeField] private List<string> tags = new List<string>();
    [SerializeField] private int order = 0;
    [SerializeField] private string subCategory;

    #endregion


    #region Properties

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public string SubCategory
    {
        get
        {
            return subCategory;
        }

        set
        {
            subCategory = value;
        }
    }

    public bool IsPremiumItem
    {
        get
        {
            return isPremiumItem;
        }

        set
        {
            isPremiumItem = value;
        }
    }

    public bool CannotBeReceived
    {
        get
        {
            return cannotBeReceived;
        }

        set
        {
            cannotBeReceived = value;
        }
    }

    public int PriceOverride
    {
        get
        {
            return priceOverride;
        }

        set
        {
            priceOverride = value;
        }
    }

    public ContentAsset.AssetType AssetType
    {
        get
        {
            return assetType;
        }

        set
        {
            assetType = value;
        }
    }

    public List<string> Tags
    {
        get
        {
            return tags;
        }

        set
        {
            tags = value;
        }
    }

    public int Order
    {
        get
        {
            return order;
        }

        set
        {
            order = value;
        }
    }

    #endregion

    public ContentItemInfo()
    {

    }

    public ContentItemInfo(ContentItemInfo copyFrom)
    {
        if (null == copyFrom) return;

        Name = copyFrom.Name;

        if (null != copyFrom.Tags)
            Tags = new List<string>(copyFrom.Tags);

        Order = copyFrom.Order;
        AssetType = copyFrom.AssetType;
        CannotBeReceived = copyFrom.CannotBeReceived;
        isPremiumItem = copyFrom.isPremiumItem;
        PriceOverride = copyFrom.PriceOverride;
        SubCategory = copyFrom.SubCategory;
    }

    public bool TryToAddTag(string tag)
    {
        if (!tags.Contains(tag))
        {
            tags.Add(tag);
            return true;
        }

        return false;
    }

    public bool TagEquals(params string[] equalsTo)
    {
        if (equalsTo.Length != tags.Count) return false;

        HashSet<string> tagsSet = new HashSet<string>(tags);

        foreach (string tag in equalsTo)
        {
            if (!tagsSet.Contains(tag)) return false;
        }

        return true;
    }
}
