#if FIREBASE_REMOTE_CONFIG
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.Firebase.RemoteConfig
{
    internal class FirebaseRemoteConfigImplementation : IFirebaseRemoteConfig
    {
        public event Action<bool> OnRemoteConfigFetch;
        private long cacheExpiration = 3600;
        
        
        public bool IsDataFetched { get; private set; }
        public int InitializationPriority => 0;
        
        
        public Task Initialize(LLFirebaseSettings settings = null)
        {
            if (settings != null)
            {
                Dictionary<string, object> defaults = new Dictionary<string, object>();
                foreach (var defaultValue in settings.DefaultValues)
                {
                    defaults.Add(defaultValue.key, defaultValue.value);
                }
                FirebaseRemoteConfig.SetDefaults(defaults);
                
                cacheExpiration = settings.CacheExpiration;
            }
            
            // Fetch data
            FirebaseRemoteConfig
                .FetchAsync(TimeSpan.FromSeconds(cacheExpiration))
                .ContinueWithOnMainThread(task =>
                {
                    FirebaseRemoteConfig.ActivateFetched();
                    
                    bool result = FirebaseRemoteConfig.Info.LastFetchStatus == LastFetchStatus.Success;
                    IsDataFetched = result;
                    
                    OnRemoteConfigFetch?.Invoke(result);
                });

            return Task.CompletedTask;
        }


        public float GetRemoteSettingsFloat(string key)
        {
            return (float)FirebaseRemoteConfig.GetValue(key).DoubleValue;
        }
        
        
        public int GetRemoteSettingsInt(string key)
        {
            return (int)FirebaseRemoteConfig.GetValue(key).LongValue;
        }


        public bool GetRemoteSettingsBool(string key)
        {
            return FirebaseRemoteConfig.GetValue(key).BooleanValue;
        }


        public string GetRemoteSettingsString(string key)
        {
            return FirebaseRemoteConfig.GetValue(key).StringValue;
        }
    }
}
#endif
