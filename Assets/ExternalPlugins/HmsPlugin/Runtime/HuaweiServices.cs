using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using Modules.Hive;
using Modules.HmsPlugin.InAppPurchase;
using Modules.InAppPurchase;
using System;
using UnityEngine;


namespace Modules.HmsPlugin
{
    [InitQueueService(-6000, typeof(IHuaweiServices))]
    public class HuaweiServices : IHuaweiServices
    {
        #region Fields
        
        private const string DefaultAdvertisingId = "00000000-0000-0000-0000-000000000000";

        #endregion
        
        
        
        #region Methods

        public string GetAdvertisingIdentifier()
        {
            #if HIVE_HUAWEI && !UNITY_EDITOR
                return HuaweiMobileServices.Ads.AdvertisingIdClient.AdVertisingIdInfo.Id;
            #else
                return DefaultAdvertisingId;
            #endif
        }
        
        
        public void InitializeService(IServiceContainer container, 
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            if (!Application.isEditor && PlatformInfo.AndroidTarget == AndroidTarget.Huawei)
            {
                InitializationQueue.Instance.OnServicePreInit += InitializationQueue_OnServicePreInit;
            }

            onCompleteCallback?.Invoke();
        }

        #endregion



        #region Events handlers

        private void InitializationQueue_OnServicePreInit(object service)
        {
            if (service is IPurchasingModuleProvider purchasingModuleImplementor)
            {
                InitializationQueue.Instance.OnServicePreInit -= InitializationQueue_OnServicePreInit;
                purchasingModuleImplementor.CustomPurchasingModule = new HuaweiPurchasingModule();
            }
        }

        #endregion
    }
}
