using System;
using Modules.General.Abstraction;
using UnityEngine;

namespace Modules.HmsPlugin.Advertising
{
    [Serializable]
    internal class HuaweiAdModuleInfo
    {
        private enum HuaweiAdModule
        {
            Banner, 
            Interstitial, 
            RewardedVideo
        }

        #region Fields

        [SerializeField]
        private HuaweiAdModule module;
        [SerializeField]
        private string moduleId;

        #endregion


        
        #region Properties

        internal string Id => moduleId;

        
        internal AdModule AdModule
        {
            get
            {
                switch (module)
                {
                    case HuaweiAdModule.Banner: return AdModule.Banner;
                    case HuaweiAdModule.Interstitial: return AdModule.Interstitial;
                    case HuaweiAdModule.RewardedVideo: return AdModule.RewardedVideo;
                    default: return AdModule.None;
                }
            }
        }

        #endregion

    }
}