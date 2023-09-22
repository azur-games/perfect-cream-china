using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using System;
using Amazon.Scripts;
using AmazonAds;
using UnityEngine;


namespace Modules.Max
{
    public class MaxBannerModuleImplementor : BannerModuleImplementor
    {
        #region Fields

        private int retryAttempt = 0;
        private DateTime requestDate = DateTime.Now;
        private DateTime responseDate = DateTime.Now;
        
        private APSBannerAdRequest bannerAdRequest;
        
        #endregion



        #region Properties

        public int ResponseDelay => (int)Math.Round((DateTime.Now - requestDate).TotalMilliseconds);


        public int ShowDelay => (int)Math.Round((DateTime.Now - responseDate).TotalMilliseconds);
        

        public string BannerId => LLMaxSettings.DoesInstanceExist ? LLMaxSettings.Instance.BannerId : string.Empty;
        
        
        public MaxSdkBase.BannerPosition BannerPosition => LLMaxSettings.DoesInstanceExist ? LLMaxSettings.Instance.BannerPosition : MaxSdkBase.BannerPosition.BottomCenter;

        
        public override bool IsBannerAvailable { get; protected set; }

        protected int BannerWidth => MaxSdkUtils.IsTablet() ? 
            LLMaxSettings.Instance.TabletBannerWidth : 
            LLMaxSettings.Instance.PhoneBannerWidth;
        
        protected int BannerHeight => MaxSdkUtils.IsTablet() ? 
            LLMaxSettings.Instance.TabletBannerHeight : 
            LLMaxSettings.Instance.PhoneBannerHeight;
        
        #endregion



        #region Class lifecycle

        public MaxBannerModuleImplementor(IAdvertisingService service) : base(service)
        {
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += BannerOnAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += BannerOnAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += BannerOnAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        }

        #endregion



        #region Methods

        public void LoadBanner()
        {
            IsBannerAvailable = false;
            Debug.Log($"[MaxBannerModuleImplementor - LoadBanner] {BannerId}");
            
            requestDate = DateTime.Now;
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            // if (BannerWidth > 0)
            // {
            //     MaxSdk.SetBannerWidth(BannerId, BannerWidth);
            // }
            
            MaxSdk.SetBannerExtraParameter(BannerId, 
                "adaptive_banner", 
                LLMaxSettings.Instance.EnableAdaptiveBanners ? "true" : "false");
            
            MaxSdk.SetBannerExtraParameter(BannerId, 
                "force_banner", 
                LLMaxSettings.Instance.ForceBanners ? "true" : "false");
            
            MaxSdk.SetBannerExtraParameter(BannerId, 
                "should_stop_auto_refresh_on_ad_expand", 
                LLMaxSettings.Instance.ShouldStopAutoRefreshOnAdExpand ? "true" : "false");

#if !UNITY_EDITOR
            if (bannerAdRequest != null)
                bannerAdRequest.DestroyFetchManager();
            
            bannerAdRequest = new APSBannerAdRequest(BannerWidth, BannerHeight, AmazonSettings.Instance.BannerSlotId);
            
            bannerAdRequest.onFailedWithError += (adError) =>
            {
                MaxSdk.SetBannerLocalExtraParameter(BannerId, "amazon_ad_error", adError.GetAdError());
                CreateBannerWithIdAndPosition();
            }; 
            bannerAdRequest.onSuccess += (adResponse) =>
            {
                MaxSdk.SetBannerLocalExtraParameter(BannerId, "amazon_ad_response", adResponse.GetResponse());
                CreateBannerWithIdAndPosition();
            };
            
            bannerAdRequest.LoadAd();
#else
            CreateBannerWithIdAndPosition();
#endif
            // Set background or background color for banners to be fully functional
            // MaxSdk.SetBannerBackgroundColor(BannerId, Color.white);
        }
        
        private void CreateBannerWithIdAndPosition()
        {
            MaxSdk.CreateBanner(BannerId, BannerPosition);
            MaxSdk.SetBannerBackgroundColor(BannerId, Color.white);
        }


        public override void ShowBanner(string placementName)
        {
            MaxSdk.ShowBanner(BannerId);

            Invoke_OnAdShow(AdActionResultType.Success, ShowDelay, string.Empty, BannerId);
            Invoke_OnAdStarted(AdActionResultType.Success, ShowDelay, string.Empty, BannerId);
        }


        public override void HideBanner()
        {
            MaxSdk.HideBanner(BannerId);

            Invoke_OnAdHide(AdActionResultType.Success, string.Empty, BannerId, "banner", "watched");
        }

        #endregion

        

        #region Event handlers

        private void BannerOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            responseDate = DateTime.Now;

            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

            Scheduler.Instance.CallMethodWithDelay(this, LoadBanner, (float)retryDelay);
            
            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Error, $"{errorInfo.Message} Info: {errorInfo.AdLoadFailureInfo}", adUnitId);
        }

        
        private void BannerOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            IsBannerAvailable = true;
            retryAttempt = 0;
            responseDate = DateTime.Now;

            Invoke_OnAdRespond(ResponseDelay, AdActionResultType.Success, string.Empty, adUnitId);
        }

        
        private void BannerOnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnAdClick(adUnitId);
        }
        
        
        private void BannerOnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Invoke_OnImpressionDataReceive(MaxUtils.GetRevenueInfo(AdModule, adInfo), adUnitId, (float)adInfo.Revenue);
        }

        #endregion
    }
}