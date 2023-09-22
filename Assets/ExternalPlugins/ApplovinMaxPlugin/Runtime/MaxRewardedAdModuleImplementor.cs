using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using System;
using UnityEngine;
using System.Collections.Generic;
using Amazon.Scripts;
using AmazonAds;

namespace Modules.Max
{
    public class MaxRewardedAdModuleImplementor : RewardedVideoModuleImplementor
    {
        #region Fields

        private int     retryAttempt = 0;
        private DateTime    requestDate = DateTime.Now;
        private DateTime    responseDate = DateTime.Now;
        private bool        isWatched;
        private bool        clicked = false;
        private bool isFirstLoad = true;
        private Dictionary<string, object> _adDataDictionary = new Dictionary<string, object>();
        #endregion



        #region Properties

        public int ResponseDelay => (int) Math.Round((DateTime.Now - requestDate).TotalMilliseconds);


        public int ShowDelay => (int) Math.Round((DateTime.Now - responseDate).TotalMilliseconds);
        

        public string RewardedId => LLMaxSettings.DoesInstanceExist ? LLMaxSettings.Instance.RewardedId : string.Empty;
        
        
        public override bool IsVideoAvailable { get; protected set; }

        #endregion



        #region Methods

        public MaxRewardedAdModuleImplementor(IAdvertisingService service) : base(service)
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }


        public override void ShowVideo(string placementName)
        {
            Debug.Log($"[MaxRewardedAdModuleImplementor - ShowVideo] {RewardedId}");
            isWatched = false;
            clicked = false;
            MaxSdk.ShowRewardedAd(RewardedId, placementName);
            Invoke_OnAdStarted(AdActionResultType.Success, ShowDelay, string.Empty, placementName, _adDataDictionary);
        }


        public void LoadRewardedAd()
        {
            IsVideoAvailable = false;
            requestDate = DateTime.Now;

#if !UNITY_EDITOR
            if (isFirstLoad)
            { 
                isFirstLoad = false;
                var rewardedVideoAdRequest = new APSVideoAdRequest(AmazonSettings.Instance.RewardedVideoWidth, AmazonSettings.Instance.RewardedVideoHeight, AmazonSettings.Instance.RewardedVideoSlotId);
                rewardedVideoAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetRewardedAdLocalExtraParameter(RewardedId, "amazon_ad_response", adResponse.GetResponse());
                    LoadRewardedAdWithId();
                }; 
                
                rewardedVideoAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetRewardedAdLocalExtraParameter(RewardedId, "amazon_ad_error", adError.GetAdError());
                    LoadRewardedAdWithId();
                }; 

                rewardedVideoAdRequest.LoadAd();
            }
            else
            {
                LoadRewardedAdWithId();
            }
#else
            LoadRewardedAdWithId();
#endif
            
            Invoke_OnAdRequested(RewardedId);
        }

        #endregion

        private void LoadRewardedAdWithId()
        {
            MaxSdk.LoadRewardedAd(RewardedId);
        }

        #region Event handlers

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            IsVideoAvailable = true;
            responseDate = DateTime.Now;
            // Reset retry attempt
            retryAttempt = 0;
            
            EventDataContainer.Data = ExtractInfo(adInfo);
            _adDataDictionary = EventDataContainer.Data;
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Success, string.Empty, adUnitId);
        }
        
        
        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            responseDate = DateTime.Now;
            // Rewarded ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        
            retryAttempt++;
            float retryDelay = (float)Math.Pow(2, Math.Min(6, retryAttempt));

            Scheduler.Instance.CallMethodWithDelay(this, LoadRewardedAd, retryDelay);
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Error, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
            Debug.Log($"EXAMPLE 2 {errorInfo.Message} {errorInfo.AdLoadFailureInfo}");
        }

        
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdShow(AdActionResultType.Success, ShowDelay, string.Empty, adUnitId);
        }
        
        
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();

            Invoke_OnAdShow(AdActionResultType.Error, ShowDelay, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }

        
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            clicked = true;
            Invoke_OnAdClick(adUnitId);
        }
        
        
        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();

            if (!isWatched)
            {
                var data = ExtractInfo(adInfo);
                Invoke_OnAdHide(AdActionResultType.Skip, string.Empty, adUnitId, adInfo.Placement, "canceled", data);
            }
        }

        

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            isWatched = true;
            
            var data = ExtractInfo(adInfo);
            Invoke_OnAdHide(AdActionResultType.Success, string.Empty, adUnitId, adInfo.Placement, clicked? "clicked": "watched", data);
        }
        
        
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
            Debug.Log($"---> Rewarded1 revenue {adInfo.Revenue}");
            
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