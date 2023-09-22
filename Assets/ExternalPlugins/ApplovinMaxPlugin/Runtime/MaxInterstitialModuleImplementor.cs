using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using System;
using System.Collections.Generic;
using Amazon.Scripts;
using AmazonAds;
using UnityEngine;


namespace Modules.Max
{
    public class MaxInterstitialModuleImplementor : InterstitialModuleImplementor
    {
        #region Fields

        private int         retryAttempt;
        private DateTime    requestDate = DateTime.Now;
        private DateTime    responseDate = DateTime.Now;
        private bool        clicked = false;
        private bool isFirstLoad = true;
        private Dictionary<string, object> _adDataDictionary = new Dictionary<string, object>();

        #endregion



        #region Properties

        public int ResponseDelay => (int) Math.Round((DateTime.Now - requestDate).TotalMilliseconds);


        public int ShowDelay => (int) Math.Round((DateTime.Now - responseDate).TotalMilliseconds);


        public string InterstitialId => LLMaxSettings.DoesInstanceExist ? LLMaxSettings.Instance.InterstitialId : string.Empty;
        
        
        public override bool IsInterstitialAvailable { get; protected set; }

        #endregion



        #region Methods

        public MaxInterstitialModuleImplementor(IAdvertisingService service) : base(service)
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        }


        public void LoadInterstitial()
        {
            IsInterstitialAvailable = false;
            requestDate = DateTime.Now;
            
            Debug.Log($"[MaxInterstitialModuleImplementor - LoadInterstitial] {InterstitialId}");
            
#if !UNITY_EDITOR
            if (isFirstLoad)
            {
                isFirstLoad = false;
                var interstitialAdRequest = new APSInterstitialAdRequest(AmazonSettings.Instance.InterstitialSlotId);
                
                interstitialAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetInterstitialLocalExtraParameter(InterstitialId, "amazon_ad_response", adResponse.GetResponse());
                    LoadInterstitialWithId();
                }; 
                
                interstitialAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetInterstitialLocalExtraParameter(InterstitialId, "amazon_ad_error", adError.GetAdError());
                    LoadInterstitialWithId();
                }; 
                
                interstitialAdRequest.LoadAd();
            }
            else
                LoadInterstitialWithId();
#else
            LoadInterstitialWithId();
#endif

            Invoke_OnAdRequested(InterstitialId);
        }
        
        private void LoadInterstitialWithId()
        {
            MaxSdk.LoadInterstitial(InterstitialId);
        }


        public override void ShowInterstitial(string placementName)
        {
            clicked = false;
            MaxSdk.ShowInterstitial(InterstitialId, placementName);
            Invoke_OnAdStarted(AdActionResultType.Success, ShowDelay, string.Empty, placementName, _adDataDictionary);
        }

        #endregion
        
        
        
        #region Event handlers
        
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            IsInterstitialAvailable = true;
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
            responseDate = DateTime.Now;

            // Reset retry attempt
            retryAttempt = 0;
            
            EventDataContainer.Data = ExtractInfo(adInfo);
            _adDataDictionary = EventDataContainer.Data;
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Success, string.Empty, adUnitId);
        }


        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            responseDate = DateTime.Now;

            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

            Scheduler.Instance.CallMethodWithDelay(this, LoadInterstitial, (float)retryDelay);
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Error, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }


        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdShow(AdActionResultType.Success, ShowDelay, string.Empty, adUnitId);
        }


        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();

            Invoke_OnAdShow(AdActionResultType.Error, ShowDelay, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }


        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var data = ExtractInfo(adInfo);
            Invoke_OnAdClick(adUnitId);
            clicked = true;
        }


        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();

            var data = ExtractInfo(adInfo);
            Invoke_OnAdHide(AdActionResultType.Skip, string.Empty, adUnitId, adInfo.Placement, clicked ? "clicked" : "watched", data);
        }

        
        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"---> Inter1 revenue paid {adInfo.Revenue}");
            
            Invoke_OnImpressionDataReceive(MaxUtils.GetRevenueInfo(AdModule, adInfo), adUnitId, (float)adInfo.Revenue);
        }
        
        private static Dictionary<string, object> ExtractInfo(MaxSdkBase.AdInfo adInfo)
        {
            var data = new Dictionary<string, object>
            {
                { "network", adInfo.NetworkName },
                { "creo_id", adInfo.CreativeIdentifier },
                { "revenue", adInfo.Revenue }
            };
            return data;
        }
        
        #endregion
    }
}