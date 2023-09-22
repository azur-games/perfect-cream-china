public class BalanceDataProvider
{
    private readonly BalanceData balanceData;

    #region Properties
    public int ExtraLevelsPeriod => balanceData.extraLevelsPeriod;

    // Coins box
    public bool IsCoinsBoxEnabled => balanceData.isCoinsBoxEnabled;
    public bool IsCoinsBoxRewardOnlyForVideo => balanceData.isCoinsBoxRewardOnlyForVideo;
    public int CoinsBoxPeriodicReward => balanceData.coinsBoxPeriodicReward;
    public int CoinsBoxMaxAmount => balanceData.coinsBoxMaxAmount;
    public int CoinsBoxVideoMultiplier => balanceData.coinsBoxVideoMultiplier;

    public int CafeUpgradeStartPrice => balanceData.cafeUpgradeStartPrice;
    public int CafeUpgradeDeltaPrice => balanceData.cafeUpgradeDeltaPrice;

    public int StarsForDesertUnlock => balanceData.starsForDessertUnlock;

    public int BarGradeLevel => balanceData.barGradeLevel;
    public int ItemCoinsPrice => balanceData.itemCoinsPrice;

    public bool WatchAdSpinnerAlways => balanceData.watchAdSpinnerAlways;
    public bool WatchAdNeedCountdown => balanceData.watchAdNeedCountdown;

    public bool IsSubscriptionAvailable => balanceData.isSubscriptionAvailable;
    public int SubscriptionCoinsReward => balanceData.subscriptionCoinsReward;

    public int TimesPlayerCanGetAdditionalCard => balanceData.timesPlayerCanGetAdditionalCard;
    public int CoinsMultiplierInLottery => balanceData.coinsMultiplierInLottery;
    public int LotteryCountCardsPerAdShow => balanceData.lotteryCountCardsPerAdShow;
    public int LotteryCountCardsAtStart => balanceData.lotteryCountCardsAtStart;

    public bool IsRandomShopItemPurchase => balanceData.isRandomShopItemPurchase;
    public int CoinsVideoRewardShop => balanceData.coinsVideoRewardShop;
    public int InGameKeysCount => balanceData.ingameKeysCount;
    public float InGameKeysChanceAppear => balanceData.ingameKeysChanceApear;
    // InApps
    public bool InAppsEnabled => balanceData.inAppsEnabled;


    public int InGameCoinsCount => balanceData.ingameCoinsCount;
    public float InGameCoinsChanceAppear => balanceData.ingameCoinsChanceApear;
    public int InGameCoinsMinLevel => balanceData.ingameCoinsMinLevel;
    public float FeverModeChance => balanceData.feverModeChance;
    public int FeverModeMinLevel => balanceData.feverModeMinLevel;
    public int FeverModeShapesMin => balanceData.feverModeShapesMin;
    public int FeverModeShapesMax => balanceData.feverModeShapesMax;
    public bool IsLotteryEnabled => balanceData.isLotteryEnabled;
    public int LotteryFrequency => balanceData.lotteryFrequency;
    public int CoinsBoxExchange => balanceData.coinsBoxExchange;
    public int CountOfAdditionalPrizes => balanceData.countOfAdditionalPrizes;
    public float GameplaySpeed => balanceData.gameplaySpeed;

    public bool IsReceivedShapeHighlightingEnabled => balanceData.isReceivedShapeHighlightingEnabled;
    public bool IsCoinsBoxBubbleEnabled => balanceData.isCoinsBoxBubbleEnabled;
    public int VideoCoinsMultiplierOnComplete => balanceData.videoCoinsMultiplierOnComplete;
    public int LevelsPeriodToShowNoAds => balanceData.levelsPeriodToShowNoAds;
    public bool IsNoAdsButtonOnResultsScreenEnabled => balanceData.isNoAdsButtonOnResultsScreenEnabled;
    public bool IsNeedShowKeysChests => balanceData.isNeedShowKeysChests;

    private static BalanceDataProvider instance;
    public static BalanceDataProvider Instance => instance;

    public BalanceDataProvider(BalanceData balanceData)
    {
        instance = this;
        this.balanceData = balanceData;
    }

    #endregion

    public float GetTimeToFinish(LevelAsset currentLevel) => currentLevel.levelBalanceData.GetLengthStart(Env.Instance.Inventory.CurrentLevelIndex) 
                                                              + currentLevel.levelBalanceData.GetLengthDelta(Env.Instance.Inventory.CurrentLevelIndex) * Env.Instance.Inventory.CurrentLevelIndex;

    public int GetNonFatalHitsNumber(LevelAsset currentLevel) => currentLevel.levelBalanceData.GetNonFatalHitsNumber(Env.Instance.Inventory.CurrentLevelIndex);
}