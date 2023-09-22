using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BalanceData))]
public class BalanceDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Tools");
        if(GUILayout.Button("Show Level Queue"))
        {
            ShowLevelsQueue();
        }
        GUILayout.Space(20);
        GUILayout.Label("Data");
        base.OnInspectorGUI();
    }

    private void ShowLevelsQueue()
    {
        var cachedRoomConfig = GetGameplayRoomConfig();
        string result = "";
        var builder = new StringBuilder(result);
      
        for (int i = 0; i < 100; i++)
        {
            builder.Append($"Level {i + 1}");
            builder.Append(i == 0 ? " - Tutorial\n" : GetLevelType(i, cachedRoomConfig));
        }
        
        Debug.Log(builder.ToString());
    }
    
    private string GetLevelType(int levelIndex, GameplayRoomConfig roomConfig)
    {
        BalanceData data  = (BalanceData)target;
        bool nextExtra =  levelIndex > 0 &&
                          0 == (levelIndex+ 1) % data.extraLevelsPeriod;
        
        int extraLevelVariant = (levelIndex + 1) / data.extraLevelsPeriod % roomConfig.ExtraLevels.Count;
        var levelName = roomConfig.ExtraLevels[extraLevelVariant];
        
        return nextExtra ? " - " + levelName + "\n" : " - common\n";
    }
    
    private GameplayRoomConfig GetGameplayRoomConfig()
    {
        string[] guids = AssetDatabase.FindAssets("t:"+ nameof(GameplayRoomConfig)); 
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<GameplayRoomConfig>(path);
    }
}
