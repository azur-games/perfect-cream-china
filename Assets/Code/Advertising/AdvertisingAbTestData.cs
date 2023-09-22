using AbTest;
using Modules.Advertising;
using Modules.General.Abstraction;
using System.Collections.Generic;
using UnityEngine;


public class AdvertisingAbTestData : IInGameAdvertisingAbTestData, IAbTestRewardSettingsData
{
    private AdsData adsData;
    public  AdsData AdsData
	{
        get
        {
            if (adsData == null)
            {
                adsData = Resources.Load<AdsData>("AsdBalance/AdsData");
            }
            return adsData;
        }
    }

    public Dictionary<AdModule, float> advertisingAvailabilityEventInfo { get; set; } = 
        new Dictionary<AdModule, float>()
        {
            { AdModule.RewardedVideo, 10.0f },
            { AdModule.Interstitial, 15.0f },
            { AdModule.Banner, 5.0f },
        };

    public int minLevelForInterstitialShowing 
    {
        get => AdsData.MinLevelForInterstitialShowing;
        set { }
    }
    public int minLevelForBannerShowing
    {
        get => AdsData.MinLevelForBannerShowing;
        set { }
    }
    public float delayBetweenInterstitials
    {
        get => AdsData.DelayBetweenInterstitials;
        set { }
    }
    public bool isNeedShowInterstitialAfterResult
    {
        get => AdsData.IsNeedShowInterstitialAfterResult;
        set { }
    }

    public bool isNeedShowInterstitialBeforeResult
    {
       get => AdsData.IsNeedShowInterstitialBeforeResult;
        set { }
    }

    public bool isNeedShowInterstitialAfterBackground
	{
        get=> AdsData.IsNeedShowInterstitialAfterBackground;
        set { }

    }
    public bool isNeedShowInactivityInterstitial
    {
       get => AdsData.IsNeedShowInactivityInterstitial;
        set { }
    }
    public float delayBetweenInactivityInterstitials
    {
        get => AdsData.DelayBetweenInactivityInterstitials;
        set { }
    }

    public bool isNeedShowSettingsOpenInterstitials
    {
        get => AdsData.IsNeedShowSettingsOpenInterstitials;
        set { }
    }

    public bool isNeedShowSettingsCloseInterstitials
	{
        get => AdsData.IsNeedShowSettingsCloseInterstitials;
        set { }

    }
    public bool isNeedShowGalleryOpenInterstitials
    {
        get => AdsData.IsNeedShowGalleryOpenInterstitials;
        set { }
    }

    public bool isNeedShowGalleryCloseInterstitials
    {
        get => AdsData.IsNeedShowGalleryCloseInterstitials;
        set { }
    }

    public bool isNeedShowInGameRestartInterstitial
    {
        get => AdsData.IsNeedShowInGameRestartInterstitial;
        set { }
    }
    public bool isNeedShow9ChestInterstitial
    {
        get => AdsData.IsNeedShow9ChestInterstitial;
        set { }
    }
    public bool isNeedShowInterstitialAfterSegment
    {
        get => AdsData.IsNeedShowInterstitialAfterSegment;
        set { }
    }


    public AbTestRvSettings BubbleRewardSettings = new AbTestRvSettings
    {
        RewardedMain = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.FreeReward,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.Rewarded
        },
        RewardedLoop = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.Rewarded
        },
        RewardValue = 50.0f
    };


    public AbTestRvSettings ChestRewardSettings = new AbTestRvSettings
    {
        RewardedMain = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.FreeReward,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.Rewarded
        },
        RewardedLoop = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.Rewarded
        },
        RewardValue = 50.0f
    };

    public AbTestRvSettings LotteryRewardSettings = new AbTestRvSettings
    {
        RewardedMain = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.FreeReward,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.Rewarded
        },
        RewardedLoop = new[]
        {
            RewardedVideoShowingAdsState.Rewarded,
            RewardedVideoShowingAdsState.None
        },
        RewardValue = 50.0f
    };


    public static AbTestRvSettings EmptyRewardSettings => new AbTestRvSettings
    {
        RewardedMain = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None
        },
        RewardedLoop = new[]
        {
            RewardedVideoShowingAdsState.None,
            RewardedVideoShowingAdsState.None
        },
        RewardValue = 50.0f
    };


    public AbTestRvSettings ResultCoinsRewardSettings = EmptyRewardSettings;


    public AbTestRvSettings ResultStarsRewardSettings = EmptyRewardSettings;


    public List<RvPlacementAbTestData> GetRewardsSettings()
    {
        return new List<RvPlacementAbTestData>
        {
            new RvPlacementAbTestData(AdsPlacements.RESULTCOINS, ResultCoinsRewardSettings),
            new RvPlacementAbTestData(AdsPlacements.RESULTSTARS, ResultStarsRewardSettings),
            new RvPlacementAbTestData(AdsPlacements.CHEST, ChestRewardSettings),
            new RvPlacementAbTestData(AdsPlacements.BUBBLE, BubbleRewardSettings),
            new RvPlacementAbTestData(AdsPlacements.LOTTERY, LotteryRewardSettings)
        };
    }
}

