using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager
{
    public enum Restriction
    {
        Clamp,
        Mod,
    }

    private ContentItemsLibrary library;

    public ContentManager()
    {
        string libPath = Env.Instance.Config.GetContentLibraryPath();
        library = Resources.Load<ContentItemsLibrary>(libPath);
        library.SegregateTypes();
    }

    public T LoadContentAsset<T>(ContentAsset.AssetType assetType, string name) where T : ContentAsset
    {
        return LoadContentAsset<T>(
            assetType,
            (asset) =>
            {
                return asset.Name == name;
            });
    }

    public ContentItemIconRef LoadContentItemIconRef(ContentAsset.AssetType assetType, string name)
    {
        return LoadContentItemIconRef(
            assetType,
            (asset) =>
            {
                return asset.Name == name;
            });
    }

    public T LoadContentAsset<T>(ContentAsset.AssetType assetType, System.Predicate<ContentItemInfo> checker) where T : ContentAsset
    {
        ContentItem contentItem = LoadContentItem(assetType, checker);
        if (null == contentItem) return null;

        ContentAsset contentAsset = contentItem.Asset;
        if (null == contentAsset) return null;

        return contentAsset as T;
    }

    public ContentItem LoadContentItem(ContentAsset.AssetType assetType, int index, Restriction restriction)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType);
        if (0 == elements.Count) return null;

        return LoadItem(elements, index, restriction);
    }

    public ContentItem LoadContentItem(ContentAsset.AssetType assetType, int index, Restriction restriction, params string[] tags)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType, tags);
        if (0 == elements.Count) return null;

        return LoadItem(elements, index, restriction);
    }

    public ContentItem LoadContentItem(ContentAsset.AssetType assetType, System.Predicate<ContentItemInfo> checker)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType);
        elements.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        for (int index = 0; index < elements.Count; index++)
        {
            ContentItemsLibrary.ContentItemsCollectionElement itemDescription = elements[index % elements.Count];

            bool itIs = checker(itemDescription.Info);
            if (!itIs) continue;

            string libPath = Env.Instance.Config.GetContentLibraryPath();
            string itemPath = System.IO.Path.GetDirectoryName(libPath) + itemDescription.RelativePath;

            if (System.IO.Path.HasExtension(itemPath))
            {
                string ext = System.IO.Path.GetExtension(itemPath);
                itemPath = itemPath.Substring(0, itemPath.Length - ext.Length);
            }

            ContentItem item = Resources.Load<ContentItem>(itemPath);
            return item;
        }

        return null;
    }

    public ContentItemIconRef LoadContentItemIconRef(ContentAsset.AssetType assetType, System.Predicate<ContentItemInfo> checker)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType);
        elements.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        for (int index = 0; index < elements.Count; index++)
        {
            ContentItemsLibrary.ContentItemsCollectionElement itemDescription = elements[index % elements.Count];
            
            bool itIs = checker(itemDescription.Info);
            if (!itIs) continue;

            string libPath = Env.Instance.Config.GetContentLibraryPath();
            string itemPath = System.IO.Path.GetDirectoryName(libPath) + itemDescription.RelativePath + ContentItemIconRef.FILE_NAME_POSTFIX;

            if (System.IO.Path.HasExtension(itemPath))
            {
                string ext = System.IO.Path.GetExtension(itemPath);
                itemPath = itemPath.Substring(0, itemPath.Length - ext.Length);
            }

            ContentItemIconRef item = Resources.Load<ContentItemIconRef>(itemPath);
            return item;
        }

        return null;
    }

    public ContentItem LoadRandomContentItem(ContentAsset.AssetType assetType, bool excludeCanNotBeReceived, params string[] excludeNames)
    {
        HashSet<string> excludedNamesSet = ((null == excludeNames) || (0 == excludeNames.Length)) ? new HashSet<string>() : new HashSet<string>(excludeNames);

        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType);
        List<ContentItemsLibrary.ContentItemsCollectionElement> valideElements = new List<ContentItemsLibrary.ContentItemsCollectionElement>(elements.Count);

        foreach (ContentItemsLibrary.ContentItemsCollectionElement el in elements)
        {
            if (Env.Instance.Inventory.IsPremiumItem(el.Info)) continue;
            if (excludeCanNotBeReceived && el.Info.CannotBeReceived) continue;
            if (excludedNamesSet.Contains(el.Info.Name)) continue;
            valideElements.Add(el);
        }

        return LoadItem(valideElements, UnityEngine.Random.Range(0, valideElements.Count), Restriction.Clamp);
    }

    public ContentItem LoadNextContentItem(ContentAsset.AssetType assetType, System.Predicate<ContentItemInfo> checker)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType);
        elements.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        for (int index = 0; index < elements.Count; index++)
        {
            ContentItemsLibrary.ContentItemsCollectionElement itemDescription = elements[index % elements.Count];

            bool itIs = checker(itemDescription.Info);
            if (!itIs) continue;

            itemDescription = elements[(1 + index) % elements.Count];

            string libPath = Env.Instance.Config.GetContentLibraryPath();
            string itemPath = System.IO.Path.GetDirectoryName(libPath) + itemDescription.RelativePath;

            if (System.IO.Path.HasExtension(itemPath))
            {
                string ext = System.IO.Path.GetExtension(itemPath);
                itemPath = itemPath.Substring(0, itemPath.Length - ext.Length);
            }

            ContentItem item = Resources.Load<ContentItem>(itemPath);
            return item;
        }

        return null;
    }

    public List<ContentItemInfo> GetAvailableInfos(ContentAsset.AssetType assetType, System.Predicate<ContentItemInfo> filter = null)
    {
        List<ContentItemInfo> resultInfos = new List<ContentItemInfo>();
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(assetType);
        elements.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        foreach (var element in elements)
        {
            if (filter == null || filter(element.Info))
            {
                resultInfos.Add(element.Info);
            }
        }

        return resultInfos;
    }

    public int GetItemIndexByAssetType(ContentItem item)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(item.Info.AssetType);
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Info.Name == item.Info.Name) return i;
        }

        return -1;
    }

    public int GetItemIndexByAssetTypeAndTags(ContentItem item)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> elements = library.GetItems(item.Info.AssetType, item.Info.Tags.ToArray());
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Info.Name == item.Info.Name) return i;
        }

        return -1;
    }

    public ContentItem LoadItem(List<ContentItemsLibrary.ContentItemsCollectionElement> elements, int index, Restriction restriction)
    {
        switch (restriction)
        {
            case Restriction.Clamp:
                index = Mathf.Clamp(index, 0, elements.Count - 1);
                break;

            case Restriction.Mod:
                index = index % elements.Count;
                break;
        }

        elements.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));
        ContentItemsLibrary.ContentItemsCollectionElement itemDescription = elements[index];

        return LoadItem(itemDescription);
    }

    public ContentItem LoadItem(ContentItemsLibrary.ContentItemsCollectionElement itemDescription)
    {
        string libPath = Env.Instance.Config.GetContentLibraryPath();
        string itemPath = System.IO.Path.GetDirectoryName(libPath) + itemDescription.RelativePath;

        if (System.IO.Path.HasExtension(itemPath))
        {
            string ext = System.IO.Path.GetExtension(itemPath);
            itemPath = itemPath.Substring(0, itemPath.Length - ext.Length);
        }

        ContentItem item = Resources.Load<ContentItem>(itemPath);
        return item;
    }

    public int GetItemsCount()
    {
        return library.GetCount();
    }

    public List<ContentItemsLibrary.ContentItemsCollectionElement> GetItems(ContentAsset.AssetType assetType)
    {
        return library.GetItems(assetType);
    }

    public List<ContentItemsLibrary.ContentItemsCollectionElement> GetItemsOfSubcategory(ContentAsset.AssetType assetType, string subcategory)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> resList = new List<ContentItemsLibrary.ContentItemsCollectionElement>();
        foreach (ContentItemsLibrary.ContentItemsCollectionElement element in GetItems(assetType))
        {
            if (element.Info.SubCategory != subcategory) continue;
            resList.Add(element);
        }

        return resList;
    }

    public ContentItemsLibrary.ContentItemsCollectionElement GetItem(ContentAsset.AssetType assetType, string name)
    {
        foreach (ContentItemsLibrary.ContentItemsCollectionElement element in GetItems(assetType))
        {
            if (element.Info.Name == name) return element;
        }

        return null;
    }

    public int GetItemsCount(ContentAsset.AssetType assetType)
    {
        return library.GetCount(assetType);
    }

    public ContentAsset.AssetType GetItemsType(string itemName)
    {
        return library.GetItemsType(itemName);
    }
}
