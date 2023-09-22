using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Level Balance Data")]
public class LevelBalanceData : ScriptableObject
{
    [SerializeField] private LevelBalance mainLevelsBalance;
    [SerializeField] private List<LevelBalanceOverride> overrides;

    public bool IsNeedObstacles(int levelIndex)
    {
        var balance = overrides.Find(o => o.levelIndex == levelIndex)?.levelBalance;
        balance ??= mainLevelsBalance;
        return balance.isNeedObstacles;
    }
    
    public int GetNonFatalHitsNumber(int levelIndex)
    {
        var balance = overrides.Find(o => o.levelIndex == levelIndex)?.levelBalance;
        balance ??= mainLevelsBalance;
        return balance.nonFatalHitsNumber;
    }
    
    public float GetLengthStart(int levelIndex)
    {
        var balance = overrides.Find(o => o.levelIndex == levelIndex)?.levelBalance;
        balance ??= mainLevelsBalance;
        return balance.lengthStart;
    }
    
    public float GetLengthDelta(int levelIndex)
    {
        var balance = overrides.Find(o => o.levelIndex == levelIndex)?.levelBalance;
        balance ??= mainLevelsBalance;
        return balance.levelLengthDelta;
    }
    
    [Serializable]
    public class LevelBalanceOverride
    {
        public int levelIndex;
        public LevelBalance levelBalance;
    }
    
    [Serializable]
    public class LevelBalance
    {
        public bool isNeedObstacles;
        public int nonFatalHitsNumber;
        [Tooltip("Score, that should be reached to finish the level")] public float lengthStart;
        [Tooltip("This delta will be multiplied to levels passed count.")] public float levelLengthDelta;
    }
}
