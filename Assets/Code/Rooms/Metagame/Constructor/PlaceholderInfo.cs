using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderInfo
{
    public string SubCategory { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Transform Parent { get; set; }

    public InteriorObjectAsset InteriorObject { get; set; }
    public ContentItemsLibrary.ContentItemsCollectionElement CurrentGradeElement { get; set; }

    public PlaceholderInfo(ConstructionPlaceholder placeholder)
    {
        SubCategory = placeholder.Subcategory;
        Position = placeholder.transform.position;
        Rotation = placeholder.transform.rotation;
        Parent = placeholder.transform.parent;

        InteriorObject = null;
        CurrentGradeElement = null;
    }

    public void DestroyInteriorObject()
    {
        if (null == InteriorObject) return;
        GameObject.Destroy(InteriorObject.gameObject);
        InteriorObject = null;
    }

    public void DestroyInteriorObjectWithAnimation(System.Action onFinish)
    {
        if (null == InteriorObject)
        {
            onFinish?.Invoke();
            return;
        }

        InteriorObject.PlayDestroying(() =>
        {
            GameObject.Destroy(InteriorObject.gameObject);
            InteriorObject = null;
            onFinish?.Invoke();
        });
    }

    public void UpgradeTo(ContentItemsLibrary.ContentItemsCollectionElement gradeElement, System.Action onFinish, bool withAnimations)
    {
        if ((null != CurrentGradeElement) && (CurrentGradeElement.Info.Order >= gradeElement.Info.Order))
        {
            //  this is not an upgrade!
            onFinish?.Invoke();
            return;
        }

        if (withAnimations)
        {
            DestroyInteriorObjectWithAnimation(() =>
            {
                CurrentGradeElement = gradeElement;
                ContentItem contentItem = Env.Instance.Content.LoadItem(gradeElement);
                InteriorObject = GameObject.Instantiate(contentItem.Asset, Position, Rotation, Parent) as InteriorObjectAsset;
                InteriorObject.PlayCreating(onFinish);
            });
        }
        else
        {
            DestroyInteriorObject();

            CurrentGradeElement = gradeElement;
            ContentItem contentItem = Env.Instance.Content.LoadItem(gradeElement);
            InteriorObject = GameObject.Instantiate(contentItem.Asset, Position, Rotation, Parent) as InteriorObjectAsset;
            onFinish?.Invoke();
        }
    }
}
