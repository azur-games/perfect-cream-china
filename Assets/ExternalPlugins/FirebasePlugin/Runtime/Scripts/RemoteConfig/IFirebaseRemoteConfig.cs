using System;


namespace Modules.Firebase.RemoteConfig
{
    internal interface IFirebaseRemoteConfig : IFirebaseModule
    {
        event Action<bool> OnRemoteConfigFetch;
        bool IsDataFetched { get; }
        
        float GetRemoteSettingsFloat(string key);
        int GetRemoteSettingsInt(string key);
        bool GetRemoteSettingsBool(string key);
        string GetRemoteSettingsString(string key);
    }
}
