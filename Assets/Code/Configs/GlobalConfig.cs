using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalConfig", menuName = "Configs/GlobalConfig")]
public class GlobalConfig : ScriptableObject
{
    #region Fields

    [SerializeField] string initialSceneName = null;
    [SerializeField] string configsResourcePathPrefix = null;
    [SerializeField] string configsResourcePathPostfix = null;
    [SerializeField] string roomResourcePathPrefix = null;
    [SerializeField] string initialRoomName = null;
    [SerializeField] string uiConfigPath = null;
    [SerializeField] string baseContentLibraryPath = null;
    [SerializeField] bool srDebuggerEnabled = true;
    [SerializeField] GameObject soundManager = null;

    #endregion


    #region Properties

    public string InitialSceneName
    {
        get
        {
            return initialSceneName;
        }
    }

    public string RoomResourcePathPrefix
    {
        get
        {
            return roomResourcePathPrefix;
        }
    }

    public string ConfigsResourcePathPrefix
    {
        get
        {
            return configsResourcePathPrefix;
        }
    }

    public string ConfigsResourcePathPostfix
    {
        get
        {
            return configsResourcePathPostfix;
        }
    }

    public string InitialRoomName
    {
        get
        {
            return initialRoomName;
        }
    }

    public string UiConfigPath
    {
        get
        {
            return uiConfigPath;
        }
    }

    public string BaseContentLibraryPath
    {
        get
        {
            return baseContentLibraryPath;
        }
    }

    public bool SrDebuggerEnabled
    {
        get
        {
            return srDebuggerEnabled;
        }
    }

    public GameObject SoundManager
    {
        get
        {
            return soundManager;
        }
    }

    #endregion

    public static GlobalConfig LoadSelf()
    {
        System.Type thisClassType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

        string thisClassTypeName = thisClassType.ToString();

        GlobalConfig globalConfig = Resources.Load<GlobalConfig>(thisClassTypeName);

        return globalConfig;
    }

    public string GetRoomConfigResourcePath<T>() where T : Room
    {
        return GetRoomConfigResourcePath(typeof(T).ToString());
    }

    public string GetRoomConfigResourcePath(string roomName)
    {
        return ConfigsResourcePathPrefix + RoomResourcePathPrefix + roomName + ConfigsResourcePathPostfix;
    }

    public string GetUIConfigResourcePath()
    {
        return ConfigsResourcePathPrefix + UiConfigPath + ConfigsResourcePathPostfix;
    }

    public string GetContentLibraryPath()
    {
        return BaseContentLibraryPath;
    }
}
