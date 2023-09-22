using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentAsset : MonoBehaviour
{
    public enum AssetType
    {
        None = 0,
        Level = 1,
        Valve = 2,
        CreamSkin = 3,
        Shape = 4,
        Confiture = 5,
        InteriorObject = 6
    }

    public Sprite Icon;
    public Sprite AlternativeIcon;
    public Sprite AlternativeIcon2;
    public string Name;

    public string SubCategory;

    public bool CannotBeReceived;
    public bool IsPremiumItem;

    public int PriceOverride;

    public virtual AssetType GetAssetType()
    {
        return AssetType.None;
    }
        
}
