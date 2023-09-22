using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayRoomConfig", menuName = "Configs/GameplayRoomConfig")]
public class GameplayRoomConfig : RoomConfig
{
    [System.Serializable]
    public struct ShapeLengthByLevel
    {
        [SerializeField] public int Level;
        [SerializeField] public float MaxShapeLength;
    }

    #region Fields

    [Space]
    [Header("Ingame")]
    [SerializeField] GameObject ingameKeyPrefab = null;
    [SerializeField] GameObject ingameCoinPrefab = null;
    [SerializeField] float minFillingForGoodResult = 0.75f;

    [SerializeField] List<ShapeLengthByLevel> shapesLengthByLevel;

    [Space]
    [Header("Finishing")]
    [SerializeField] FinishingFlowConfig finishingFlowConfig = null;

    [Space]
    [Header("Shapes generation settings")]
    [SerializeField] AnimationCurve shapesInverseCDF;

    [Space]
    [Header("Score settings")]
    [SerializeField] float scoreToFinishFirstLevel = 400.0f;
    [SerializeField] float scoreToFinishPerLevel = 200.0f;
    [SerializeField] float scoreToFinishMax = 1000.0f;

    [Space]
    [Header("Extra levels settings")]
    [SerializeField] List<string> extraLevels = new List<string>();

    [Space]
    [Header("Rewards settings")]
    [SerializeField] int baseLevelCoinsReward = 35;
    [SerializeField] int extraCoinsMultiplier = 5;

    [Space] 
    [Header("Visual effects")] 
    [SerializeField] GameObject shapeTopTextPrefab;

    #endregion



    #region Properties

    public float MaxShapeLengthOnLevel(int level)
    {
        foreach (ShapeLengthByLevel slbl in shapesLengthByLevel)
        {
            if (slbl.Level == level) return slbl.MaxShapeLength;
        }

        return float.MaxValue;
    }

    public FinishingFlowConfig FinishingFlowConfig
    {
        get
        {
            return finishingFlowConfig;
        }
    }

    public List<string> ExtraLevels
    {
        get
        {
            return extraLevels;
        }
    }

    public float MinFillingForGoodResult => minFillingForGoodResult;

    public int BaseLevelCoinsReward => baseLevelCoinsReward;

    public int ExtraCoinsMultiplier => extraCoinsMultiplier;

    public GameObject IngameKeyPrefab => ingameKeyPrefab;

    public GameObject IngameCoinPrefab => ingameCoinPrefab;

    public GameObject ShapeTopTextPrefab => shapeTopTextPrefab;

    public AnimationCurve ShapesInverseCDF => shapesInverseCDF;

    #endregion
}
