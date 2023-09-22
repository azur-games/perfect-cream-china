using System;
using System.Collections.Generic;
using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.HmsPlugin.Advertising;
using UnityEngine;
using static HuaweiConstants.UnityBannerAdPositionCode;

namespace Modules.HmsPlugin.Settings
{
    [CreateAssetMenu(fileName = "LLHuaweiKitsSettings")]
    public class LLHuaweiKitsSettings : ScriptableSingleton<LLHuaweiKitsSettings>
    {
        #region Inner structures

        private enum HuaweiBannerSize
        {
            BANNER_SIZE_320_50,
            USER_DEFINED,
            BANNER_SIZE_320_100,
            BANNER_SIZE_468_60,
            BANNER_SIZE_DYNAMIC,
            BANNER_SIZE_728_90,
            BANNER_SIZE_300_250,
            BANNER_SIZE_SMART,
            BANNER_SIZE_160_600,
            BANNER_SIZE_360_57
        }

        #endregion
        
        
        
        #region Fields
        
        private const string HuaweiAdIdPrefsFormat = "Huawei{0}Id";

        public bool IsIAPKitEnabled;
        
        public bool IsAdsKitEnabled;

        [SerializeField] [HideInInspector]
        private List<HuaweiAdModuleInfo> adModulesInfos;

        [SerializeField] [HideInInspector]
        private UnityBannerAdPositionCodeType bannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM;

        [SerializeField] [HideInInspector] 
        private HuaweiBannerSize bannerSize = HuaweiBannerSize.BANNER_SIZE_320_50;
        
        private readonly Dictionary<AdModule, string> abTestsModulesIds = new Dictionary<AdModule, string> ();

        #endregion



        #region Properties

        public HuaweiBannerSettings BannerSettings => new HuaweiBannerSettings()
        {
            BannerPosition = bannerPosition,
            BannerSize = bannerSize.ToString()
        };
        
        
        public static Dictionary<AdModule, string> TestAdsIds => new Dictionary<AdModule, string>()
        {
            { AdModule.Banner, "testw6vs28auh3" },
            { AdModule.Interstitial, "testb4znbuh3n2" },
            { AdModule.RewardedVideo, "testx9dtjwj8hp"}
        };

        #endregion
        
        

        #region Methods

        public bool IsModuleEnabled(AdModule module, out string id)
        {       
            id = GetModuleId(module);
            return !string.IsNullOrEmpty(id);
        }
        
        
        public string GetModuleId(AdModule module) 
        {
            string prefsKey = string.Format(HuaweiAdIdPrefsFormat, module.ToString());
            if (CustomPlayerPrefs.HasKey(prefsKey))
            {
                return CustomPlayerPrefs.GetString(prefsKey);
            }
            if (abTestsModulesIds.ContainsKey(module) && !string.IsNullOrEmpty(abTestsModulesIds[module]))
            {
                return abTestsModulesIds[module];
            }
            return adModulesInfos.Find(i => i.AdModule.Equals(module))?.Id;
        }
        
        
        public void SetAdsAbTestKeysData(HuaweiAdsKitAbTestData abTestData)
        {
            CustomDebug.Log($"[LLHuaweiAdsKitSettings - SetAbTestKeysData] {abTestData.huaweiAdsKitRewardedVideoModuleId}," +
                            $" {abTestData.huaweiAdsKitInterstitialModuleId}, {abTestData.huaweiAdsKitBannerModuleId}");
            
            if (!string.IsNullOrEmpty(abTestData.huaweiAdsKitBannerModuleId))
            {
                abTestsModulesIds[AdModule.Banner] = abTestData.huaweiAdsKitBannerModuleId;
            }
            if (!string.IsNullOrEmpty(abTestData.huaweiAdsKitInterstitialModuleId))
            {
                abTestsModulesIds[AdModule.Interstitial] = abTestData.huaweiAdsKitInterstitialModuleId;
            }
            if (!string.IsNullOrEmpty(abTestData.huaweiAdsKitRewardedVideoModuleId))
            {
                abTestsModulesIds[AdModule.RewardedVideo] = abTestData.huaweiAdsKitRewardedVideoModuleId;
            }
        }


        public void SaveModuleIdToPrefs(AdModule module, string id)
        {
            CustomPlayerPrefs.SetString(string.Format(HuaweiAdIdPrefsFormat, module.ToString()), id);
        }

        #endregion
    }
}