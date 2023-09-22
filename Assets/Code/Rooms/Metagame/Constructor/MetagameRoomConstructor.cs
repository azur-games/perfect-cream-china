using System.Collections.Generic;
using UnityEngine;

public class MetagameRoomConstructor : MonoBehaviour
{
    private Dictionary<string, PlaceholderInfo> pivots;
    private List<ContentItemsLibrary.ContentItemsCollectionElement> contentElements;

    public void Construct()
    {
        pivots = CollectPivots();

        contentElements = Env.Instance.Content.GetItems(ContentAsset.AssetType.InteriorObject);
        contentElements.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        int barGradeLevelFromConfig = BalanceDataProvider.Instance.BarGradeLevel;
        if (barGradeLevelFromConfig > 0)
        {
            Env.Instance.Inventory.SetInteriorGrade(barGradeLevelFromConfig);
        }

        UpToGrade(null, false);
    }

    public int GetLastGradeLevel()
    {
        int lastLevel = 0;
        foreach (ContentItemsLibrary.ContentItemsCollectionElement element in contentElements)
        {
            lastLevel = Mathf.Max(element.Info.Order, lastLevel);
        }

        return lastLevel;
    }

    public int GetMaxGradeLevel()
    {
        return contentElements.Last().Info.Order;
    }

    public void UpToGrade(System.Action onFinish, bool withAnimations)
    {
        // find elements for current grade level
        Dictionary<string, ContentItemsLibrary.ContentItemsCollectionElement> lastGradeElements = new Dictionary<string, ContentItemsLibrary.ContentItemsCollectionElement>();
        foreach (ContentItemsLibrary.ContentItemsCollectionElement element in contentElements)
        {
            if (element.Info.Order > Env.Instance.Inventory.InteriorLevel) break;
            if (!pivots.ContainsKey(element.Info.SubCategory))
            {
                Debug.LogError("Can not find placeholder for subcategory: " + element.Info.SubCategory);
                continue;
            }

            if (!lastGradeElements.ContainsKey(element.Info.SubCategory))
            {
                lastGradeElements.Add(element.Info.SubCategory, element);
            }
            else
            {
                lastGradeElements[element.Info.SubCategory] = element;
            }
        }

        if (withAnimations)
        {
            int waitingAnimations = lastGradeElements.Values.Count;

            // instantiate items instead placeholders
            foreach (ContentItemsLibrary.ContentItemsCollectionElement element in lastGradeElements.Values)
            {
                PlaceholderInfo pinfo = pivots[element.Info.SubCategory];
                pinfo.UpgradeTo(
                    element, 
                    () =>
                    {
                        waitingAnimations--;
                        if (0 == waitingAnimations)
                        {
                            onFinish?.Invoke();
                        }
                    }, 
                    true);
            }
        }
        else
        {
            // instantiate items instead placeholders
            foreach (ContentItemsLibrary.ContentItemsCollectionElement element in lastGradeElements.Values)
            {
                PlaceholderInfo pinfo = pivots[element.Info.SubCategory];
                pinfo.UpgradeTo(element, null, false);
            }

            onFinish?.Invoke();
        }
    }

    private Dictionary<string, PlaceholderInfo> CollectPivots()
    {
        Dictionary<string, PlaceholderInfo> points = new Dictionary<string, PlaceholderInfo>();
        ConstructionPlaceholder[] placeholders = gameObject.GetComponentsInChildren<ConstructionPlaceholder>();

        foreach (ConstructionPlaceholder placeholder in placeholders)
        {
            PlaceholderInfo pinfo = new PlaceholderInfo(placeholder);
            points.Add(pinfo.SubCategory, pinfo);
            Destroy(placeholder.gameObject);
        }

        return points;
    }
}
