using System;
using AmazonAds;
using Modules.General.ServicesInitialization;
using UnityEngine;

namespace Amazon.Scripts
{
    public class AmazonServiceImplementor : IInitializable
    {
        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;

        
        public AmazonServiceImplementor()
        {
            if (!AmazonSettings.DoesInstanceExist)
                Debug.LogError("[AmazonServiceImplementor] Need AmazonSettings to init Amazon");
        }

        public void Initialize()
        {
#if !UNITY_EDITOR
            AmazonAds.Amazon.Initialize(AmazonSettings.Instance.AppId);
            var isDebugBuild = Debug.isDebugBuild;
            AmazonAds.Amazon.EnableTesting(isDebugBuild);
            AmazonAds.Amazon.EnableLogging(isDebugBuild);
            AmazonAds.Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.MAX));
#endif
            OnServiceInitialized?.Invoke(this, InitializationStatus.Success);
        }

        public bool IsAsyncInitializationEnabled { get; }

        public void PreInitialize()
        {
        }

        public void SetUserConsent(bool isConsentAvailable)
        {
            AmazonAds.Amazon.SetConsentStatus(AmazonAds.Amazon.ConsentStatus.EXPLICIT_YES);
        }
    }
}