using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public static class ItemsCollectionEditorTools
{
    private static HashSet<string> availableTags = null;
    public static HashSet<string> AvailableTags
    {
        get
        {
            if (null == availableTags)
            {
                RefreshTags();
            }

            return availableTags;
        }
    }

    public static string LastAvailableTag
    {
        get
        {
            return AvailableTags.LastOrDefault();
        }
    }

    public static int AvailableTagsCount
    {
        get
        {
            return (null == AvailableTags) ? 0 : AvailableTags.Count;
        }
    }

    public static void RefreshTags(bool forced = false)
    {
        if (forced) availableTags = null;
        
        if (null != availableTags) return;

        availableTags = new HashSet<string>();

        foreach (string guid in AssetDatabase.FindAssets("t:contentitem", null))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ContentItem item = AssetDatabase.LoadAssetAtPath<ContentItem>(path);

            foreach (string tag in item.Info.Tags)
            {
                availableTags.Add(tag);
            }
        }
    }

    public static ContentItemsLibrary FindParentLibrary(ContentItem item)
    {
        string fullAssetPath = AssetDatabase.GetAssetPath(item);
        string directoriesPath = fullAssetPath;
        if (string.IsNullOrEmpty(directoriesPath)) return null;

        while (true)
        {
            directoriesPath = System.IO.Path.GetDirectoryName(directoriesPath);
            if (string.IsNullOrEmpty(directoriesPath)) break;

            ContentItemsLibrary library = AssetDatabase.LoadAssetAtPath<ContentItemsLibrary>(directoriesPath);
            if (null != library) return library;
        }

        return null;
    }

    public static ContentItemsLibrary FindParentLibrary(ContentItem item, List<ContentItemsLibrary> allLibraries, out string itemRelativePath)
    {
        string itemPath = AssetDatabase.GetAssetPath(item);
        string itemsDirectoryPath = System.IO.Path.GetDirectoryName(itemPath);
        itemRelativePath = null;

        foreach (ContentItemsLibrary library in allLibraries)
        {
            string libraryDirectoryPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(library));
            if (itemsDirectoryPath.Length < libraryDirectoryPath.Length) continue;

            string itemPathSubstring = itemsDirectoryPath.Substring(0, libraryDirectoryPath.Length);
            if (itemPathSubstring == libraryDirectoryPath)
            {
                itemRelativePath = itemPath.Substring(libraryDirectoryPath.Length + 1);
                return library;
            }
        }

        return null;
    }

    public static void DrawUseSelectedFolderField(string caption, ref string path)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(caption + path);
        if (GUILayout.Button("<use selected>", GUILayout.Width(150.0f)))
        {
            path = ItemsCollectionEditorTools.GetFirstSelectedFolder() ?? "";
        }
        GUILayout.EndHorizontal();
    }

    public static string GetFirstSelectedFolder()
    {
        Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        
        foreach (Object obj in selectedAsset)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && System.IO.Directory.Exists(path))
            {
                //path = Path.GetDirectoryName(path);
                return path;
            }
        }

        return null;
    }

    public static List<string> GetAssetsPaths<T>(List<T> assets)
    {
        List<string> resList = new List<string>();
        if (null != assets)
        {
            foreach (T asset in assets)
            {
                string path = AssetDatabase.GetAssetPath(asset as Object);
                if (string.IsNullOrEmpty(path)) continue;

                resList.Add(path);
            }
        }

        return resList;
    }

    public static void DeleteAssets(List<string> paths)
    {
        foreach (string assetPath in paths)
        {
            AssetDatabase.DeleteAsset(assetPath);
        }
    }

    public static List<T> GetAssets<T>(string path, bool onPrefab, out List<string> paths) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        paths = new List<string>();

        string[] prefabPath = string.IsNullOrEmpty(path) ? null : new[] { path };

        List<string> guids = onPrefab ?
            (new List<string>(AssetDatabase.FindAssets("t:prefab", prefabPath))) :
            FindAssetGUIDs<T>(path);

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) continue;

            T assetObj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (null == assetObj) continue;

            paths.Add(assetPath);
            assets.Add(assetObj);
        }

        return assets;
    }

    public static List<string> FindAssetGUIDs<T>(string path)
    {
        string typeName = typeof(T).Name.ToLower();
        string[] assetPath = string.IsNullOrEmpty(path) ? null : new[] { path };
        string[] guids = AssetDatabase.FindAssets("t:" + typeName, assetPath);
        return new List<string>(guids);
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    public static void DrawUILine(Color color, float xoffset, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.x += xoffset;
        r.width += 6;
        r.width -= xoffset;
        EditorGUI.DrawRect(r, color);
    }

    /*public static void DeleteTag(string tag)
    {
        List<ContentItemsLibrary> allLibraries = GetAssets<ContentItemsLibrary>(null, false, out _);
        List<ContentItem> allItems = GetAssets<ContentItem>(null, false, out _);

        foreach (ContentItem item in allItems)
        {
            bool removed = item.Info.Tags.Remove(tag);

            if (removed)
            {
                EditorUtility.SetDirty(item);
            }
        }

        foreach (ContentItemsLibrary library in allLibraries)
        {
            bool removed = library.RemoveTag(tag);

            if (removed)
            {
                EditorUtility.SetDirty(library);
            }
        }
    }*/

    public static void RenameTag(string tag, string newTag)
    {
        List<ContentItemsLibrary> allLibraries = GetAssets<ContentItemsLibrary>(null, false, out _);
        List<ContentItem> allItems = GetAssets<ContentItem>(null, false, out _);

        foreach (ContentItem item in allItems)
        {
            // remove old tag
            bool oldTagRemoved = item.Info.Tags.Remove(tag);
            if (!oldTagRemoved) continue;

            // add new tag
            item.Info.TryToAddTag(newTag);
            EditorUtility.SetDirty(item);

            // update library
            ContentItemsLibrary library = FindParentLibrary(item, allLibraries, out string itemRelativePath);
            if (null == library)
            {
                Debug.LogError("Content item " + AssetDatabase.GetAssetPath(item) + " has no parent library!");
                continue;
            }

            //library.UpdateElementInfo(itemRelativePath, item);
            EditorUtility.SetDirty(library);
        }
    }

    public static bool SetTag(IEnumerable<ContentItem> toItems, string tag)
    {
        bool tagWasSetted = false;

        List<ContentItemsLibrary> allLibraries = GetAssets<ContentItemsLibrary>(null, false, out _);

        foreach (ContentItem item in toItems)
        {
            bool tagAdded = item.Info.TryToAddTag(tag);
            if (!tagAdded) continue;
            EditorUtility.SetDirty(item);

            // update library
            ContentItemsLibrary library = FindParentLibrary(item, allLibraries, out string itemRelativePath);
            if (null == library)
            {
                Debug.LogError("Content item " + AssetDatabase.GetAssetPath(item) + " has no parent library!");
                continue;
            }

            //library.UpdateElementInfo(itemRelativePath, item);
            EditorUtility.SetDirty(library);

            tagWasSetted = true;
        }

        return tagWasSetted;
    }

    public static void RemoveTag(IEnumerable<ContentItem> toItems, string tag)
    {
        List<ContentItemsLibrary> allLibraries = GetAssets<ContentItemsLibrary>(null, false, out _);

        foreach (ContentItem item in toItems)
        {
            bool removed = item.Info.Tags.Remove(tag);
            if (!removed) continue;
            EditorUtility.SetDirty(item);

            // update library
            ContentItemsLibrary library = FindParentLibrary(item, allLibraries, out string itemRelativePath);
            if (null == library)
            {
                Debug.LogError("Content item " + AssetDatabase.GetAssetPath(item) + " has no parent library!");
                continue;
            }

            //library.UpdateElementInfo(itemRelativePath, item);
            EditorUtility.SetDirty(library);
        }
    }

    public static void DrawTagsPopup(string currentTag, System.Action<string> newSelectedTagAction)
    {
        int currentTagIndex = AvailableTags.ToList().IndexOf(currentTag);

        int index = EditorGUILayout.Popup(currentTagIndex, AvailableTags.ToArray(), GUILayout.Width(100.0f));
        if (index != currentTagIndex)
        {
            newSelectedTagAction(AvailableTags.ToArray()[index]);
        }
    }

    public static void RefreshContentItemsLibrary(ContentItemsLibrary contentLibrary)
    {
        string libraryFilePath = GetAssetsPaths(new List<ContentItemsLibrary>() { contentLibrary })[0];

        string libraryFolder = System.IO.Path.GetDirectoryName(libraryFilePath);

        List<ContentItem> items = GetAssets<ContentItem>(libraryFolder, false, out List<string> paths);

        contentLibrary.Elements = new List<ContentItemsLibrary.ContentItemsCollectionElement>(items.Count);
        for (int index = 0; index < items.Count; index++)
        {
            ContentItem item = items[index];
            string path = paths[index];
            string relativePath = path.Substring(libraryFolder.Length + 1);

            contentLibrary.Elements.Add(new ContentItemsLibrary.ContentItemsCollectionElement(relativePath, item.Info));
        }

    }
}
