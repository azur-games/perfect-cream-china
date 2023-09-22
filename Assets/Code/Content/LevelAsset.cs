using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelAsset : ContentAsset
{
    public LevelBalanceData levelBalanceData;
    
    public enum DeliveredObjectType
    {
        None,
        CommonBox,
        PlasticBox,
        MetallicBox,
        SoapBox,
        FastFoodBox,
    }

    public override AssetType GetAssetType() { return AssetType.Level; }

    public ShapeGenerator Generator;

    public FinalAnimatedBox FinishAnimationObject;
    public DeliveredObjectType TypeOfDeliveredObject;

    public FinalJar FinalJar;
    public string ValveAssetOverrideName;

    public List<ColorScheme> ColorSchemes;

    public string CreamSkinName;

    public int OverrideCoinsReward = -1;

    public int MinStarsCount = 0;

    // progress filling
    public Color BaseUIColor;

    private void Awake()
    {
        Generator.SetBalance(levelBalanceData);
    }
}
