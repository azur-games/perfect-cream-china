using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive;
using Modules.Hive.Ioc;
using Modules.InAppPurchase;
using System;
using UnityEngine;
using UnityEngine.Purchasing.Extension;


namespace Modules.HmsPlugin.InAppPurchase
{
    /// <summary>
    /// Переработанный ExternalDependencies/hms-unity-plugin/Huawei/Scripts/IAP/UnityPurchase/HuaweiPurchasingModule.cs
    /// </summary>
    internal class HuaweiPurchasingModule : AbstractPurchasingModule
    {
        #region Fields

        // Должен быть такой же как HuaweiAppGallery.Name 
        private const string STORE_NAME = "AppGallery";

        #endregion

        
        
        #region AbstractPurchasingModule

        public override void Configure()
        {
            if (!Application.isEditor && PlatformInfo.AndroidTarget == AndroidTarget.Huawei)
            {
                RegisterStore(STORE_NAME, HuaweiStore.Instance);
            }
            else
            {
                CustomDebug.LogError($"[HuaweiPurchasingModule - Configure] Trying to configure with {PlatformInfo.AndroidTarget}");
            }
        }

        #endregion
    }
}