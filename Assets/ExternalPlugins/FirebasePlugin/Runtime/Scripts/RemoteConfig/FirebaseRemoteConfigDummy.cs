using System;
using System.Threading.Tasks;


namespace Modules.Firebase.RemoteConfig
{
    internal class FirebaseRemoteConfigDummy : IFirebaseRemoteConfig
    {
        public event Action<bool> OnRemoteConfigFetch;
        
        
        public bool IsDataFetched => false;
        public int InitializationPriority => 0;

        public Task Initialize(LLFirebaseSettings settings = null)
        {
            OnRemoteConfigFetch?.Invoke(false);
            return Task.CompletedTask;
        }
        
        
        public float GetRemoteSettingsFloat(string key) => 0.0f;
        public int GetRemoteSettingsInt(string key) => 0;
        public bool GetRemoteSettingsBool(string key) => false;
        public string GetRemoteSettingsString(string key) => string.Empty;
    }
}
