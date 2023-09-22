using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace Modules.Firebase
{
    [CreateAssetMenu(fileName = "LLFirebaseSettings")]
    public class LLFirebaseSettings : ScriptableSingleton<LLFirebaseSettings>
    {
        [Serializable]
        public struct DefaultValueStruct
        {
            public string key;
            [TextArea(1, 30)] public string value;
            public string description;
        }


        [SerializeField] [Header("Analytics")] private bool shouldSendGameAnalytics = false;
        [SerializeField] [Header("Cache Expiration")] private long cacheExpiration = 3600L;
        [Space(10)]
        [SerializeField] [Header("Remote Config Default Values")] private DefaultValueStruct[] defaultValues = null;
        [Space(10)]
        [SerializeField] [Header("Firebase Id Fetch Delay")] private int firebaseIdFetchRetryDelay = 250;

        
        public DefaultValueStruct[] DefaultValues => defaultValues;
        
        
        public long CacheExpiration => cacheExpiration;
        
        
        public bool ShouldSendGameAnalytics => shouldSendGameAnalytics;
        
        
        public int FirebaseIdFetchRetryDelay => firebaseIdFetchRetryDelay;
    }
}