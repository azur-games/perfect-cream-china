using HuaweiMobileServices.Ads;
using Modules.Advertising;
using Modules.General.Abstraction;


namespace Modules.HmsPlugin.Advertising
{
    internal class HuaweiRewardedVideoModuleImplementor : RewardedVideoModuleImplementor, IRewardAdStatusListener
    {
        #region Fields

        private RewardAd rewardedView;

        private string latestLoadedAdId;

        private bool wasRewardGiven;

        private bool isShowing;
        
        #endregion



        #region Properties

        public override bool IsVideoAvailable
        {
            get => rewardedView != null && rewardedView.Loaded;
            protected set => throw new System.NotImplementedException();
        }

        #endregion
        


        #region Methods

        public HuaweiRewardedVideoModuleImplementor(IAdvertisingService service) : base(service) {}
        
        
        public override void ShowVideo(string placementName)
        {
            if (rewardedView?.Loaded == true)
            {
                ApplicationFocusObserver.Instance.StartObserve(OnRewardAdClosed);
                rewardedView.Show(this);
            }
            else
            {
                CustomDebug.LogError("[HuaweiRewardedVideoModuleImplementor - Show] Rewarded Ad still not loaded");
            }
        }
        
        
        public void LoadVideo(string id)
        {
            CustomDebug.Log($"[HuaweiRewardedVideoModuleImplementor - LoadVideo] {id}");
            latestLoadedAdId = id;
            if (rewardedView != null)
            {
                rewardedView.Destroy();
            }
            rewardedView = new RewardAd(id);
            Invoke_OnAdRequested(string.Empty);
            rewardedView.LoadAd(new AdParam.Builder().Build(), OnRewardAdLoaded, OnRewardAdFailedToLoad);
        }

        #endregion



        #region Event handlers

        private void OnRewardAdLoaded()
        {
            Invoke_OnAdRespond(0, AdActionResultType.Success, string.Empty, string.Empty);
        }
        
        
        private void OnRewardAdFailedToLoad(int errorCode)
        {
            string errorDescription = errorCode.ToAdsKitResultCode();
            Invoke_OnAdRespond(0, AdActionResultType.Error,  errorDescription, string.Empty);
        }
        

        public void OnRewardAdClosed()
        {
            if (isShowing)
            {
                isShowing = false;
                AdActionResultType resultType = wasRewardGiven ? AdActionResultType.Success : AdActionResultType.Skip;
                Invoke_OnAdHide(resultType, string.Empty, string.Empty, string.Empty, string.Empty);
                ApplicationFocusObserver.Instance.StopObserve();
                LoadVideo(latestLoadedAdId);
            }
            else
            {
                CustomDebug.Log("[HuaweiRewardedVideoModuleImplementor - OnRewardAdClosed] OnAdClosed already called for this show"); 
            }

        }
        

        public void OnRewardAdFailedToShow(int errorCode)
        {
            string errorDescription = errorCode.ToAdsKitResultCode();
            Invoke_OnAdRespond(0, AdActionResultType.Error,  errorDescription, string.Empty);
        }
        

        public void OnRewardAdOpened()
        {
            wasRewardGiven = false;
            isShowing = true;
            Invoke_OnAdShow(AdActionResultType.Success, 0, string.Empty, string.Empty);
        }

        
        public void OnRewarded(Reward reward)
        {
            wasRewardGiven = true;
        }

        #endregion

    }
}