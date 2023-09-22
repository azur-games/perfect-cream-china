using AbTest;
using Modules.Advertising;
using System.Collections.Generic;
using Modules.General.Abstraction;


public class CreamAbTestData : IAbTestData
{
    public bool isNeedObstacles = true;
    public bool isNeedShowKeysChests = true;

    public int nonFatalHitsNumber = 1;
    public int extraLevelsPeriod = 3;
    public int showSafeChestLevelsPeriod = 2;
    public float levelLengthStart = 150.0f;
    public float levelLengthDelta = 1.0f;
    public float levelLengthMax = 155.0f;

    // NEW
    public float feverModeChance = 0.415f; // - turn off by default; // 0.415f; // every 2-3 levels
    public int feverModeMinLevel = 1;
    public int feverModeShapesMin = 5;
    public int feverModeShapesMax = 8;

    // Coins box
    public bool isCoinsBoxEnabled = true; // - turn off by default
    public bool isCoinsBoxRewardOnlyForVideo = false;
    public bool isCoinsBoxBubbleEnabled = true;
    public int coinsBoxExchange = 5;
    public int coinsBoxPeriodicReward = 30;
    public int coinsBoxMaxAmount = 50;
    public int coinsBoxVideoMultiplier = 2;

    public bool isReceivedShapeHighlightingEnabled = true;

    public int cafeUpgradeStartPrice = 50;
    public int cafeUpgradeDeltaPrice = 75;

    public int starsForDessertUnlock = 15;

    public int metagamePersBubblePeriod = 2;

    public float ingameKeysChanceApear = 1.0f;
    public int ingameKeysCount = 1;

    public float ingameCoinsChanceApear = 1.0f;
    public int ingameCoinsCount = 1;
    public int ingameCoinsMinLevel = 2;

    public int barGradeLevel = 0;

    public int itemCoinsPrice = 500;

    public int videoCoinsMultiplierOnComplete = 2;

    public bool watchAdSpinnerAlways = true;
    public bool watchAdNeedCountdown = true;

    public float gameplaySpeed = 1.5f;

    public bool isSubscriptionAvailable = true;
    #if UNITY_IOS
    public bool isSubscriptionExtended = true;
    #else
    public bool isSubscriptionExtended = false;
    #endif
    public int subscriptionCoinsReward = 100;

    public bool isNoAdsButtonOnResultsScreenEnabled = true;
    public int levelsPeriodToShowNoAds = 5;

    public bool isLotteryEnabled = true;
    public int timesPlayerCanGetAdditionalCard = 4;
    public int lotteryFrequency = 3;
    public int countOfAdditionalPrizes = 1;
    public int coinsMultiplierInLottery = 5;
    public int lotteryCountCardsPerAdShow = 1;
    public int lotteryCountCardsAtStart = 1;

    public bool isRandomShopItemPurchase = true;
    public int coinsVideoRewardShop = 150;

    //obsolete in versions 1.8+
    public bool isStarsVideoActiveOnComplete = false;
    public bool isCoinsVideoActiveOnComplete = false;

    //used only in versions 1.7.1
    public bool isFixedSubscriptionAvailable = false;

    // InApps
    public bool inAppsEnabled = true;
}


