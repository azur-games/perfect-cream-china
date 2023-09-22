using Modules.General.Abstraction;
using Modules.General.ServicesInitialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppsFlyerConnector;
using AppsFlyerSDK;
using UnityEngine;


namespace Modules.AppsFlyer
{
    public class AppsFlyerAnalyticsServiceImplementor : IAnalyticsService
    {
        #region Fields
        
        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;

        private string deviceId;
        private LLAppsFlyerSettings appsFlyerSettings;

        #endregion

#if DEVELOPMENT_BUILD
        public static bool IsDebug => true;
#else
        public static bool IsDebug => false;
#endif

        #region Properties
        
        public bool IsAsyncInitializationEnabled =>
            #if UNITY_IOS
                true;  
            #elif UNITY_ANDROID
                false;
            #else
                true;
            #endif

        public bool IsAsyncWorkAvailable => true;

        #endregion



        #region Class lifecycle

        public AppsFlyerAnalyticsServiceImplementor(LLAppsFlyerSettings settings)
        {
            appsFlyerSettings = settings;
        }

        #endregion



        #region Methods

        public void PreInitialize() { }


        public void SetDeviceId(string deviceId)
        {
            this.deviceId = deviceId;
        }


        public void SetUserConsent(bool isConsentAvailable)
        {
            //AppsFlyerSDK.AppsFlyer.SetUserConsent(isConsentAvailable);
        }


        public void Initialize()
        {
            try
            {
                CustomDebug.Log("[Appsflyer] Init start");
                AppsFlyerSDK.AppsFlyer.initSDK(appsFlyerSettings.devKey, appsFlyerSettings.appID,
                    AppsFlyerMarker.Instance);

                AppsFlyerPurchaseConnector.init(AppsFlyerMarker.Instance, AppsFlyerConnector.Store.GOOGLE);
                AppsFlyerPurchaseConnector.setIsSandbox(false);
                AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(
                    AppsFlyerAutoLogPurchaseRevenueOptions
                        .AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions,
                    AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases); 
                AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);
                AppsFlyerPurchaseConnector.build();
                AppsFlyerPurchaseConnector.startObservingTransactions();
                AppsFlyerSDK.AppsFlyer.startSDK();
                InitializationStatus analyticsInitializationStatus = InitializationStatus.None;

                OnServiceInitialized?.Invoke(this, InitializationStatus.Success);
                
                AppsFlyerAdRevenue.start();
                AppsFlyerAdRevenue.setIsDebug(IsDebug);

                CustomDebug.Log("[Appsflyer] Init complete");
            }
            catch (Exception e)
            {
                CustomDebug.LogError(e);
                OnServiceInitialized?.Invoke(this, InitializationStatus.Failed);
            }
        }


        public void SendEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            if (IsAsyncWorkAvailable)
            {
                Task.Run(() =>
                {
                    AppsFlyerSDK.AppsFlyer.sendEvent(eventName, parameters);
                });
            }
            else
            {
                AppsFlyerSDK.AppsFlyer.sendEvent(eventName, parameters);
            }
        }


        public void SetUserProperty(string name, string value) {}


        public static void LogPurchase(
            string productName,
            string currencyCode,
            string price,
            string transactionId,
            string androidPurchaseDataJson = "",
            string androidPurchaseSignature = "",
            string androidPublicKey = "")
        {
            CustomDebug.Log($"[AppsFlyer] Sending purchase event for {productName}");
#if UNITY_IOS
             AppsFlyerSDK.AppsFlyer.validateAndSendInAppPurchase(productName,
                price,
                currencyCode,
                transactionId,
                null,
                AppsFlyerMarker.Instance
            );
#elif UNITY_ANDROID
            AppsFlyerSDK.AppsFlyer.validateAndSendInAppPurchase(
                androidPublicKey,
                androidPurchaseSignature,
                androidPurchaseDataJson,
                price,
                currencyCode,
                null, 
                AppsFlyerMarker.Instance
            );
#endif
        }


        public string LLAppsFlyerGetAppsFlyerUID()
        {
            return AppsFlyerSDK.AppsFlyer.getAppsFlyerId();
        }


        public void TrackCrossPromoImpression(
            string appId,
            IReadOnlyDictionary<string, string> parameters,
            Action<bool> callback = null)
        {
            // LLAppsFlyerManager.TrackCrossPromoImpression(appId, parameters, callback);
        }


        public void TrackCrossPromoClick(
            string appId,
            IReadOnlyDictionary<string, string> parameters)
        {
            //LLAppsFlyerManager.TrackCrossPromoClick(appId, parameters);
        }

        #endregion
        
    }
}
