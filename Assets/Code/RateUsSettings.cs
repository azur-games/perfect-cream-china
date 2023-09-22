using Modules.General.HelperClasses;
using Modules.Hive;
using UnityEngine;


[CreateAssetMenu(fileName = "RateUsSettings")]
public class RateUsSettings : ScriptableSingleton<RateUsSettings>
{
    #region Fields

    [SerializeField] private string urlIOS = default;
    [SerializeField] private string urlAndroid = default;
    [SerializeField] private string urlHuawei;
    
    #endregion



    #region Properties
    
    public string UrlIOS => urlIOS;


    public string UrlAndroid
    {
        get
        {
            switch (PlatformInfo.AndroidTarget)
            {
                case AndroidTarget.Huawei:
                    return urlHuawei;
                default:
                    return urlAndroid;
            }
        }
    }

    #endregion
}