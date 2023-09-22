using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentItemsLibrary : ScriptableObject
{
    [System.Serializable]
    public class ContentItemsCollectionElement
    {
        [SerializeField] public string RelativePath = null;
        [SerializeField] public ContentItemInfo Info;

        public ContentItemsCollectionElement(string relativePath, ContentItemInfo info)
        {
            RelativePath = relativePath;
            Info = new ContentItemInfo(info);
        }
    }

    #region Fields

    [SerializeField] List<ContentItemsCollectionElement> elements = null;

    #endregion


    #region Properties

    public List<ContentItemsCollectionElement> Elements
    {
        get
        {
            return elements;
        }

        set
        {
            elements = value;
        }
    }

    public Dictionary<ContentAsset.AssetType, List<ContentItemsCollectionElement>> ItemsByType { get; private set; } = 
        new Dictionary<ContentAsset.AssetType, List<ContentItemsCollectionElement>>();

    #endregion

    public void SetElements(Dictionary<ContentItem, string> items)
    {
        elements = new List<ContentItemsCollectionElement>();

        foreach (KeyValuePair<ContentItem, string> item in items)
        {
            ContentItemsCollectionElement element = new ContentItemsCollectionElement(item.Value, new ContentItemInfo(item.Key.Info));
            elements.Add(element);
        }
    }

    public void SegregateTypes()
    {
        foreach (ContentItemsCollectionElement element in elements)
        {
            if (!ItemsByType.TryGetValue(element.Info.AssetType, out List<ContentItemsCollectionElement> listByType))
            {
                listByType = new List<ContentItemsCollectionElement>();
                ItemsByType.Add(element.Info.AssetType, listByType);
            }

            listByType.Add(element);
        }

        // check names
        foreach (ContentAsset.AssetType assetType in ItemsByType.Keys)
        {
            HashSet<string> namesSet = new HashSet<string>();
            foreach (ContentItemsCollectionElement cice in ItemsByType[assetType])
            {
                if (namesSet.Contains(cice.Info.Name))
                {
                    Debug.LogError("Duplicated item Name found: " + cice.Info.Name);
                    continue;
                }

                namesSet.Add(cice.Info.Name);
            }
        }
    }

    public int GetCount()
    {
        return elements.Count;
    }

    public int GetCount(ContentAsset.AssetType assetType)
    {
        return GetItems(assetType).Count;
    }

    public ContentAsset.AssetType GetItemsType(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return ContentAsset.AssetType.None;

        foreach (ContentItemsLibrary.ContentItemsCollectionElement element in elements)
        {
            if (element.Info.Name == itemName) return element.Info.AssetType;
        }

        return ContentAsset.AssetType.None;
    }

    public List<ContentItemsCollectionElement> GetItems(ContentAsset.AssetType assetType)
    {
        if (assetType == ContentAsset.AssetType.None)
        {
            return new List<ContentItemsCollectionElement>(elements);
        }

        if (ItemsByType.TryGetValue(assetType, out List<ContentItemsCollectionElement> listByType))
        {
            return new List<ContentItemsCollectionElement>(listByType);
        }

        return new List<ContentItemsCollectionElement>();
    }

    public List<ContentItemsCollectionElement> GetItems(ContentAsset.AssetType assetType, params string[] tags)
    {
        List<ContentItemsCollectionElement> resList = new List<ContentItemsCollectionElement>();

        foreach (ContentItemsCollectionElement element in GetItems(assetType))
        {
            if (!element.Info.TagEquals(tags)) continue;

            resList.Add(element);
        }

        return resList;
    }
}
