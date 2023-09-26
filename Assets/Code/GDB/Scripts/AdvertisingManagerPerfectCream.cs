using System;
using Modules.General.Abstraction;
using BoGD.UI.PERFECTCREAM;
using Modules.General;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

namespace Modules.Advertising
{
    public class AdvertisingManagerPerfectCream : AdvertisingManager
    {
        private bool reachabilitySent = false;

        protected override void Init()
        {
            base.Init();
        }
        private void OnSendRevenue(string serviceName,
            string impressionJsonData,
            string adIdentifier,
            float revenue)
		{

            if (revenue < 0.0f)
            {
                return;
            }

            Dictionary<string, string> data = Json.Deserialize<Dictionary<string, string>>(impressionJsonData);

            String networkName = "";
            data.TryGetValue("network_name", out networkName);
            String location = "";
            data.TryGetValue("country_code", out location);

            var analyticData = new Dictionary<string, object>
            {
                { "adUnitIdentifier", adIdentifier },
                { "currency", "USD" },
                { "networkName", networkName },
                { "value", revenue },
                { "location", location }
            };
            
            //FirebaseManager.LogEvent("purchase", analyticData);
            //FB.LogAppEvent("ad_revenue_max", revenue, analyticData);

            CustomDebug.LogWarning("FIREBASE AND FACEBOOK SENT REVENUE");
        }

        public bool CheckActiveNoAdsAndSubscription
		{
			get
			{
                return false;
            }
		}
        

        private WindowWarningInterstitial warningInterstitial = null;

        private List<string> accesPlacements = new List<string>(){ "BeforeResult"};

		public override void TryShowAdByModule(AdModule module, string adShowingPlacement, Action<AdActionResultType> callback = null)
        {
            if (BoGD.CheatController.Instance.CheatBuid)
            {
                if (!BoGD.CheatController.Instance.HasCheat(BoGD.CONSOLE.Cheat.EnableAds))
                {
                    callback?.Invoke(AdActionResultType.Success);
                    return;
                }
            }

           base.TryShowAdByModule(module, adShowingPlacement, callback);
        }


		protected override void ActivityUpdater_OnUpdateInactivityTimer()
        {
            if (Application.platform != RuntimePlatform.Android && !Application.isEditor)
            {
                base.ActivityUpdater_OnUpdateInactivityTimer();
                return;
            }

            string placement = AdPlacementType.Inactivity;

            if (CheckActiveNoAdsAndSubscription)
			{
                return;
			}

            if(!IsAdModuleByPlacementAvailable(AdModule.Interstitial, placement)) //check necessary
            {
                return;
            }

            if (!IsVideoAvailable(AdModule.Interstitial))
            {
                SenAnalytics("video_ads_available", placement, "not_available", "interstitial");
                ResetTimer();
                return;
            }

            if (warningInterstitial != null)
            {
                return;
            }

            ForceShowInactivityInterstitial = true;
            warningInterstitial = Env.Instance.UI.Messages.ShowWarningInterstitial("inactivity", OnCloseWarningTimer);
            reachabilitySent = false;
        }

        private void ResetTimer()
		{
            InactivityTimer.Instance.ResetInactivityTimer();
        }

        private void OnCloseWarningTimer(bool value)
        {
            CustomDebug.Log("OnCloseWarningTimer");
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                TryShowAdByModule(AdModule.Interstitial, AdPlacementType.Inactivity, actionResultType =>
                {
                    ForceShowInactivityInterstitial = false;
                    if (InactivityTimer.HasFoundInstance)
                    {
                        ResetTimer();
                    }
                    warningInterstitial = null;
                });
            }, 0.0f);
            //warningInterstitial = null;
        }

        private void OnCloseWarningBack(bool value)
        {
            CustomDebug.Log("OnCloseWarningBack");
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                TryShowAdByModule(AdModule.Interstitial, AdPlacementType.Background);
            }, 0.0f);
            warningInterstitial = null;
        }

        protected override void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEntered)
        {
            var placement = AdPlacementType.Background;
            
            if (CheckActiveNoAdsAndSubscription)
			{
                return;
			}

            if (!IsAdModuleByPlacementAvailable(AdModule.Interstitial, placement))// �������������
            {
                return;
            }

            if (Application.platform != RuntimePlatform.Android && !Application.isEditor)
            {
                base.LLApplicationStateRegister_OnApplicationEnteredBackground(isEntered);
                return;
            }

            if (!IsVideoAvailable(AdModule.Interstitial))
            {
                //SendAdAvailabilityCheckByModule(AdModule.Interstitial, false, AdPlacementType.Background, AdAvailabilityErrorType.NoAdAvailable);
                if (!InactivityTimer.Instance.InterstitialBackgroundTimerAvailable)
                {
                    SenAnalytics("video_ads_available", placement, "not_available", "interstitial");
                    InactivityTimer.Instance.InterstitialBackgroundTimerAvailable = true;
                }

                return;
            }

            if (warningInterstitial != null)
            {
                return;
            }

            if (!isEntered)
            {
                warningInterstitial = Env.Instance.UI.Messages.ShowWarningInterstitial("background", OnCloseWarningBack);
            }
        }

        protected override void Controller_OnAdStarted(AdModule adModule, AdActionResultType responseResultType, int delay, string errorDescription, string adIdentifier, string advertisingPlacement)
        {
            base.Controller_OnAdStarted(adModule, responseResultType, delay, errorDescription, adIdentifier, advertisingPlacement);
            if (adModule != AdModule.Banner)
            {
                SenAnalytics("video_ads_started", advertisingPlacement, "start", GetTypeFromModule(adModule));
            }
        }

        protected override void Controller_OnAdShow(AdModule adModule, AdActionResultType responseResultType, int delay, string errorDescription, string adIdentifier, string advertisingPlacement)
        {
            //if (adModule != AdModule.Banner)
            //{
            //    SenAnalytics("video_ads_started", advertisingPlacement, "start", GetTypeFromModule(adModule));
            //}
            base.Controller_OnAdShow(adModule, responseResultType, delay, errorDescription, adIdentifier, advertisingPlacement);
        }

        protected override void Controller_OnAdHide(AdModule adModule, AdActionResultType responseResultType, string errorDescription, string adIdentifier, string advertisingPlacement, string result)
        {
            base.Controller_OnAdHide(adModule, responseResultType, errorDescription, adIdentifier, advertisingPlacement, result);
            if (adModule != AdModule.Banner)
            {
                SenAnalytics("video_ads_watch", advertisingPlacement, result , GetTypeFromModule(adModule));
            }
        }

        private void OnAvailabilityCheck(AdModule adModule, string placement, bool result)
        {
            if (adModule == AdModule.Banner)
            {
                return;
            }

            SenAnalytics("video_ads_available", placement, result ? "success" : "not_available", GetTypeFromModule(adModule));
        }

        private string GetTypeFromModule(AdModule adModule)
        {
            return adModule == AdModule.Banner ? "banner" : adModule == AdModule.Interstitial ? "interstitial" : "rewarded";
        }



        public void SenAnalytics(string eventName, string placementName, string result, string adModule)
        {
            //if (CheckSubscription(adModule))
			//{
            //    return;
			//}
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["placement"] = placementName;
            dictionary["result"] = result;
            dictionary["ad_type"] = adModule;
            dictionary["connection"] = Application.internetReachability == NetworkReachability.NotReachable ? 0 : 1;
        }

		private bool CheckSubscription(string typeAds)
		{
            if (GetModuleFromType(typeAds) == AdModule.Interstitial)
            {
                return CheckActiveNoAdsAndSubscription;
            }
            return false;
        }

        private AdModule GetModuleFromType(string type)
        {
            switch(type)
			{
                case "banner":
                    return AdModule.Banner;
                case "interstitial":
                    return AdModule.Interstitial;
                case "rewarded":
                    return AdModule.RewardedVideo;

                default:
                    return AdModule.None;
            }
        }
    }    
}