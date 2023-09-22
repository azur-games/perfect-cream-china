using HuaweiMobileServices.Ads;
using Modules.Advertising;
using Modules.General.Abstraction;
using System;


namespace Modules.HmsPlugin.Advertising
{
    internal class HuaweiInterstitialModuleImplementor : InterstitialModuleImplementor, IAdListener
    {
        #region Fields

        private InterstitialAd interstitialView;

        private string latestLoadedAdId;

        private bool isShowing;

        #endregion



        #region Properties
        
        public override bool IsInterstitialAvailable
        {
            get => interstitialView != null && interstitialView.Loaded;
            protected set => throw new NotImplementedException();
        }
        
        #endregion



        #region Methods

        public HuaweiInterstitialModuleImplementor(IAdvertisingService service) : base(service) {}
        

        public override void ShowInterstitial(string placementName)
        {
            if (interstitialView?.Loaded == true)
            {
                ApplicationFocusObserver.Instance.StartObserve(OnAdClosed);
                interstitialView.Show();
            }
            else
            {
                CustomDebug.LogError("[HuaweiInterstitialModuleImplementor - ShowInterstitial] Interstitial Ad Still Not Loaded Yet!");
            }
        }
        
        
        public void LoadInterstitial(string id)
        {
            CustomDebug.Log($"[HuaweiInterstitialModuleImplementor - LoadInterstitial] {id}");
            latestLoadedAdId = id;
            interstitialView = new InterstitialAd
            {
                AdId = id,
                AdListener = this
            };
            Invoke_OnAdRequested(string.Empty);
            interstitialView.LoadAd(new AdParam.Builder().Build());
        }

        #endregion

        
        
        #region Events handlers

        public void OnAdLoaded()
        {
            Invoke_OnAdRespond(0, AdActionResultType.Success, string.Empty, string.Empty);
        }
        
        
        public void OnAdFailed(int reason)
        {
            string errorDescription = reason.ToAdsKitResultCode();
            Invoke_OnAdRespond(0, AdActionResultType.Error,  errorDescription, string.Empty);
        }
        
        
        public void OnAdClosed()
        {
            if (isShowing)
            {
                isShowing = false;
                ApplicationFocusObserver.Instance.StopObserve();
                Invoke_OnAdHide(AdActionResultType.Success, string.Empty, string.Empty, string.Empty, string.Empty);
                LoadInterstitial(latestLoadedAdId);
            }
            else
            {
                CustomDebug.Log("[HuaweiInterstitialModuleImplementor - OnAdClosed] OnAdClosed already called for this show"); 
            }
        }
        

        public void OnAdLeave()
        {
            CustomDebug.Log("[HuaweiInterstitialModuleImplementor - OnAdLeave]");
        }

        
        public void OnAdOpened()
        {
            isShowing = true;
            Invoke_OnAdShow(AdActionResultType.Success, 0, string.Empty, string.Empty);
        }


        public void OnAdClicked()
        {
            Invoke_OnAdClick(string.Empty);
        }

        
        public void OnAdImpression()
        {
            // Never called - https://github.com/EvilMindDevs/hms-unity-plugin/issues/254
            Invoke_OnImpressionDataReceive(string.Empty, string.Empty, 0);
        }
        
        #endregion
    }
}