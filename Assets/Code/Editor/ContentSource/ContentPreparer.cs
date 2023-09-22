using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ContentPreparer
{
    private const string SCRIPTABLE_OBJECT_EXTENSION = ".asset";
    private const string SCRIPTABLE_OBJECT_LIBRARY_NAME = "ContentItemsLibrary";

    public static bool PrepareContentItems(ContentSource contentSource, out string errorString)
    {
        errorString = null;

        if (string.IsNullOrEmpty(contentSource.TargetDirectory))
        {
            errorString = "No destination directory specified";
            return false;
        }

        string sourceDirectory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(contentSource));

        ClearDirectory(contentSource.TargetDirectory);

        List<ContentAsset> sourceAssets = ItemsCollectionEditorTools.GetAssets<ContentAsset>(sourceDirectory, true, out _);
        Dictionary<ContentItem, string> resultItems = new Dictionary<ContentItem, string>();

        foreach (ContentAsset sourceAsset in sourceAssets)
        {
            PrepareItem(sourceAsset, sourceDirectory, contentSource.TargetDirectory, out ContentItem contentItem, out string resultAssetPath);
            resultItems.Add(contentItem, resultAssetPath);
        }

        AssetDatabase.SaveAssets();

        string contentLibraryPath = System.IO.Path.Combine(contentSource.TargetDirectory, SCRIPTABLE_OBJECT_LIBRARY_NAME + SCRIPTABLE_OBJECT_EXTENSION);
        ContentItemsLibrary contentLibrary = ScriptableObject.CreateInstance<ContentItemsLibrary>();
        AssetDatabase.CreateAsset(contentLibrary, contentLibraryPath);
        contentLibrary.SetElements(resultItems);
        EditorUtility.SetDirty(contentLibrary);
        AssetDatabase.SaveAssets();

        return true;
    }

    private static void PrepareItem(ContentAsset sourceAsset, string sourceDirectory, string targetDirectory, out ContentItem contentItem, out string resultAssetPath)
    {
        resultAssetPath = AssetDatabase.GetAssetPath(sourceAsset);
        resultAssetPath = resultAssetPath.Substring(sourceDirectory.Length, resultAssetPath.Length - sourceDirectory.Length);

        string fileExtension = System.IO.Path.GetExtension(resultAssetPath);
        resultAssetPath = string.IsNullOrEmpty(fileExtension) ? resultAssetPath : resultAssetPath.Substring(0, resultAssetPath.Length - fileExtension.Length);

        string resultFullPath = targetDirectory + resultAssetPath;
        string dirName = System.IO.Path.GetDirectoryName(resultFullPath);
        if (!System.IO.Directory.Exists(dirName))
        {
            System.IO.Directory.CreateDirectory(dirName);
        }

        // Content Item
        contentItem = ScriptableObject.CreateInstance<ContentItem>();
        AssetDatabase.CreateAsset(contentItem, resultFullPath + SCRIPTABLE_OBJECT_EXTENSION);
        contentItem.Asset = sourceAsset;
        contentItem.Info = new ContentItemInfo()
        {
            Name = sourceAsset.Name,
            CannotBeReceived = sourceAsset.CannotBeReceived,
            IsPremiumItem = sourceAsset.IsPremiumItem,
            PriceOverride = sourceAsset.PriceOverride,
            SubCategory = sourceAsset.SubCategory,
            AssetType = sourceAsset.GetAssetType(),
            Order = GetAssetItemOrder(resultAssetPath),
        };

        EditorUtility.SetDirty(contentItem);

        // Content Item Icon Ref
        string iconRefPath = resultFullPath + ContentItemIconRef.FILE_NAME_POSTFIX;
        ContentItemIconRef iconRef = ScriptableObject.CreateInstance<ContentItemIconRef>();
        AssetDatabase.CreateAsset(iconRef, iconRefPath + SCRIPTABLE_OBJECT_EXTENSION);
        iconRef.Icon = sourceAsset.Icon;
        iconRef.AlternativeIcon = sourceAsset.AlternativeIcon;
        iconRef.AlternativeIcon2 = sourceAsset.AlternativeIcon2;
        EditorUtility.SetDirty(iconRef);
    }

    private static int GetAssetItemOrder(string assetPath)
    {
        string parentDirectoryName = System.IO.Path.GetDirectoryName(assetPath);
        string prevParentDirectoryName = System.IO.Path.GetDirectoryName(parentDirectoryName);
        parentDirectoryName = parentDirectoryName.Substring(prevParentDirectoryName.Length, parentDirectoryName.Length - prevParentDirectoryName.Length);
        return ExtractNum(parentDirectoryName);
    }

    private static int ExtractNum(string str)
    {
        HashSet<char> numChars = new HashSet<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        string numStr = "";
        for (int i = 0; i < str.Length; i++)
        {
            char chr = str[i];
            if (!numChars.Contains(chr)) continue;
            numStr += chr;
        }

        if (string.IsNullOrEmpty(numStr)) return 0;
        if (!int.TryParse(numStr, out int resultInt)) return 0;

        return resultInt;
    }

    private static void ClearDirectory(string path)
    {
        if (System.IO.Directory.Exists(path))
        {
            System.IO.Directory.Delete(path, true);
        }
        
        System.IO.Directory.CreateDirectory(path);
    }
}
