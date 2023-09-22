using HuaweiMobileServices.Ads;
using Modules.Advertising;
using Modules.General.Abstraction;
using System;


namespace Modules.HmsPlugin.Advertising
{
    internal class HuaweiBannerModuleImplementor : BannerModuleImplementor
    {
        #region Fields

        private BannerAd bannerView;

        #endregion
        

        
        #region Properties

        public override bool IsBannerAvailable
        {
            get;
            protected set;
        }
        
        #endregion

        

        #region Methods

        public HuaweiBannerModuleImplementor(IAdvertisingService service) : base(service) { }
        

        public void LoadBanner(string id, HuaweiBannerSettings settings)
        {
            var bannerAdStatusListener = new AdStatusListener();
            bannerAdStatusListener.mOnAdLoaded += AdStatusListener_mOnAdLoaded;
            bannerAdStatusListener.mOnAdFailed += AdStatusListener_mOnAdFailed;
            bannerAdStatusListener.mOnAdClosed += AdStatusListener_mOnAdClosed;
            bannerAdStatusListener.mOnAdImpression += AdStatusListener_mOnAdImpression;
            bannerAdStatusListener.mOnAdClicked += AdStatusListener_mOnAdClicked;
            bannerAdStatusListener.mOnAdOpened += AdStatusListener_mOnAdOpened;

            bannerView?.DestroyBanner();
            
            bannerView = new BannerAd(bannerAdStatusListener);
            bannerView.AdId = id;
            bannerView.PositionType = (int)settings.BannerPosition;
            bannerView.SizeType = settings.BannerSize;
            bannerView.AdStatusListener = bannerAdStatusListener;
            IsBannerAvailable = false;
            
            Invoke_OnAdRequested(string.Empty);
            
            bannerView.LoadBanner(new AdParam.Builder().Build());
            bannerView.HideBanner();
        }
        
        
        public override void ShowBanner(string placementName)
        {
            if (bannerView == null)
            {
                CustomDebug.LogError("[HuaweiBannerController - Show] Banner Ad is Null.");
                return;
            }
            bannerView.ShowBanner();
        }
        

        public override void HideBanner()
        {
            if (bannerView == null)
            {
                CustomDebug.LogError("[HuaweiBannerController - Hide] Banner Ad is Null.");
                return;
            }
            bannerView.HideBanner();
        }

        #endregion
        
        
        
        #region Event handlers

        private void AdStatusListener_mOnAdLoaded(object sender, EventArgs e)
        {
            IsBannerAvailable = true;
            Invoke_OnAdRespond(0, AdActionResultType.Success, string.Empty, string.Empty);
        }
        
        
        private void AdStatusListener_mOnAdFailed(object sender, AdLoadErrorCodeEventArgs e)
        {
            string errorDescription = e.ErrorCode.ToAdsKitResultCode();
            Invoke_OnAdRespond(0, AdActionResultType.Error,  errorDescription, string.Empty);
        }

        
        private void AdStatusListener_mOnAdOpened(object sender, EventArgs e)
        {
            Invoke_OnAdShow(AdActionResultType.Success, 0, string.Empty, string.Empty);
        }
        
        
        private void AdStatusListener_mOnAdClosed(object sender, EventArgs e)
        {
            Invoke_OnAdHide(AdActionResultType.Success, string.Empty, string.Empty, string.Empty, string.Empty);
        }
        

        private void AdStatusListener_mOnAdClicked(object sender, EventArgs e)
        {
            Invoke_OnAdClick(string.Empty);
        }

        private void AdStatusListener_mOnAdImpression(object sender, EventArgs e)
        {
            Invoke_OnImpressionDataReceive(string.Empty, string.Empty, 0);
        }

        #endregion
    }
}