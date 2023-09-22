using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentItem : ScriptableObject
{
    #region Fields

    [SerializeField] ContentItemInfo info = new ContentItemInfo();

    [SerializeField] ContentAsset asset = null;

    #endregion


    #region Properties

    public ContentItemInfo Info
    {
        get
        {
            return info;
        }

        set
        {
            info = value;
        }
    }

    public ContentAsset Asset
    {
        get
        {
            return asset;
        }

        set
        {
            asset = value;
        }
    }

    #endregion
}
