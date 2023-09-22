using HuaweiMobileServices.Ads;
using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using Modules.Hive;
using Modules.HmsPlugin.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.HmsPlugin.Advertising
{
    public class HuaweiAdvertisingServiceImplementor : IAdvertisingService
    {
        #region Fields
        
        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;

        private HuaweiAdsKitAbTestData huaweiAdsKitAbTestData;

        private HuaweiBannerModuleImplementor bannerImplementor;
        
        private HuaweiInterstitialModuleImplementor interstitialImplementor;
        
        private HuaweiRewardedVideoModuleImplementor rewardedVideoImplementor;
        
        #endregion



        #region Properties

        public bool IsAsyncInitializationEnabled => false;


        public bool IsSupportedByCurrentPlatform => PlatformInfo.AndroidTarget == AndroidTarget.Huawei;
        
        
        public string ServiceName => "huawei";


        public List<IEventAds> SupportedAdsModules { get; } = new List<IEventAds>();
        
        #endregion



        #region Class lifecycle;

        public HuaweiAdvertisingServiceImplementor(HuaweiAdsKitAbTestData abTestData = null)
        {
            #if UNITY_EDITOR || !HIVE_HUAWEI
                return;
            #endif
            
            huaweiAdsKitAbTestData = abTestData;
            
            if (LLHuaweiKitsSettings.Instance.IsModuleEnabled(AdModule.Banner, out string bannerId))
            {
                SupportedAdsModules.Add(bannerImplementor = new HuaweiBannerModuleImplementor(this));
            }
            if (LLHuaweiKitsSettings.Instance.IsModuleEnabled(AdModule.Interstitial, out string interId))
            {
                SupportedAdsModules.Add(interstitialImplementor = new HuaweiInterstitialModuleImplementor(this));
            }
            if (LLHuaweiKitsSettings.Instance.IsModuleEnabled(AdModule.RewardedVideo, out string rewardedId))
            {
                SupportedAdsModules.Add(rewardedVideoImplementor = new HuaweiRewardedVideoModuleImplementor(this));
            }
        }

        #endregion

        

        #region Methods

        public void PreInitialize()
        {
            CustomDebug.Log($"[HuaweiAdvertisingServiceImplementor - PreInitialize]");
        }
        

        public void SetUserConsent(bool isConsentAvailable)
        {
            if (isConsentAvailable)
            {
                var huaweiService = Services.GetService<IHuaweiServices>();
                string idfa = huaweiService?.GetAdvertisingIdentifier() ?? string.Empty;
                if (string.IsNullOrEmpty(idfa))
                {
                    CustomDebug.Log($"[HuaweiAdvertisingServiceImplementor - SetUserConsent] no IDFA for user consent");
                    return;
                }
                HwAds.SetConsent(idfa);
            }
        }
        

        public void Initialize()
        {
            #if UNITY_EDITOR || !HIVE_HUAWEI
                OnServiceInitialized?.Invoke(this, InitializationStatus.Success);
                return;
            #endif

            if (!LLHuaweiKitsSettings.DoesInstanceExist)
            {
                Debug.LogError("[HuaweiAdvertisingServiceImplementor - Initialize] Need LLHuaweiKitsSettings asset to init Huawei");
                return;
            }

            if (huaweiAdsKitAbTestData != null)
            {
                LLHuaweiKitsSettings.Instance.SetAdsAbTestKeysData(huaweiAdsKitAbTestData);
            }

            if (LLHuaweiKitsSettings.Instance.IsModuleEnabled(AdModule.Banner, out string bannerId))
            {
                bannerImplementor?.LoadBanner(bannerId, LLHuaweiKitsSettings.Instance.BannerSettings);
            }
            if (LLHuaweiKitsSettings.Instance.IsModuleEnabled(AdModule.Interstitial, out string interId))
            {
                interstitialImplementor?.LoadInterstitial(interId);
            }
            if (LLHuaweiKitsSettings.Instance.IsModuleEnabled(AdModule.RewardedVideo, out string rewardedId))
            {
                rewardedVideoImplementor?.LoadVideo(rewardedId);
            }
            
            OnServiceInitialized?.Invoke(this, InitializationStatus.Success);
        }

        
        public void ReloadWithTestAd(bool saveToPrefs = true)
        {
            Dictionary<AdModule, string> testIds = LLHuaweiKitsSettings.TestAdsIds;

            void SaveToPrefsIfNeeded(AdModule module)
            {
                if (saveToPrefs)
                {
                    LLHuaweiKitsSettings.Instance.SaveModuleIdToPrefs(module, testIds[module]);
                }
            }
            
            if (bannerImplementor != null && testIds.ContainsKey(AdModule.Banner))
            {
                bannerImplementor.LoadBanner(testIds[AdModule.Banner], LLHuaweiKitsSettings.Instance.BannerSettings);
                SaveToPrefsIfNeeded(AdModule.Banner);
            }
            if (interstitialImplementor != null && testIds.ContainsKey(AdModule.Interstitial))
            {
                interstitialImplementor.LoadInterstitial(testIds[AdModule.Interstitial]);
                SaveToPrefsIfNeeded(AdModule.Interstitial);
            }
            if (rewardedVideoImplementor != null && testIds.ContainsKey(AdModule.RewardedVideo))
            {
                rewardedVideoImplementor.LoadVideo(testIds[AdModule.RewardedVideo]);
                SaveToPrefsIfNeeded(AdModule.RewardedVideo);
            }
        }

        #endregion
    }
}