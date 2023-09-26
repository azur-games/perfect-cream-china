using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Balance Data")]
public class BalanceData : ScriptableObject
{
    [Header("Leveling rules")]
    
    [Tooltip("Count of levels to pass until EXTRA level appear.")] 
    public int extraLevelsPeriod = 3;
    
    [Tooltip("Chance of fever mode start")]
    public float feverModeChance = 0.415f;
    [Tooltip("Since this level fever mode will become available. In this level it will be with 100% chance")]
    public int feverModeMinLevel = 1;
    public int feverModeShapesMin = 5;
    public int feverModeShapesMax = 8;
    
    public bool isReceivedShapeHighlightingEnabled = true;
    
    [Header("Coins Box")]
    public bool isCoinsBoxEnabled = true; // - turn off by default
    public bool isCoinsBoxRewardOnlyForVideo = false;
    public bool isCoinsBoxBubbleEnabled = true;
    public int coinsBoxExchange = 10;
    public int coinsBoxPeriodicReward = 30;
    public int coinsBoxMaxAmount = 50;
    public int coinsBoxVideoMultiplier = 2;
    
    [Header("Cafe")]
    public int cafeUpgradeStartPrice = 50;
    public int cafeUpgradeDeltaPrice = 75;

    public int starsForDessertUnlock = 15;
    [Space(10)]
    public float ingameKeysChanceApear = 1.0f;
    public int ingameKeysCount = 1;
    public bool isNeedShowKeysChests = true;
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
    
    // InApps
    public bool inAppsEnabled = true;
}

