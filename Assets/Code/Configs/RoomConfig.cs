using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomConfig", menuName = "Configs/RoomConfig")]
public class RoomConfig : ScriptableObject
{
    #region Fields

    [SerializeField] string someinfo = null;
    [SerializeField] List<string> scenes = null;
    [SerializeField] List<GameObject> prefabs = null;

    [SerializeField] SrDebuggerHelper.OptionType srDebugerOptions = 0;

    #endregion



    #region Properties

    public string SomeInfo
    {
        get
        {
            return someinfo;
        }
    }

    public List<GameObject> Prefabs
    {
        get
        {
            return prefabs;
        }
    }

    public List<string> Scenes
    {
        get
        {
            return scenes;
        }
    }

    public SrDebuggerHelper.OptionType SrDebugerOptions
    {
        get
        {
            return srDebugerOptions;
        }
    }

    #endregion
}
