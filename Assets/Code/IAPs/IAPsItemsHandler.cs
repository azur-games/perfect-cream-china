using Modules.General;
using Modules.General.Abstraction.InAppPurchase;


public class IAPsItemsHandler
{
    #region Items IDs

    public const string NoAdsId = "noads";
    public const string WeeklySubscriptionId = "diamondweekly";

    #endregion



    #region Instancing

    private static IAPsItemsHandler instance;

    public static IAPsItemsHandler Instance => instance ?? (instance = new IAPsItemsHandler());

    #endregion



    #region Properties

    public bool IsNoAdsCheatActive { get; set; } = false;

    public IStoreItem NoAds { get; }

    public IStoreItem WeeklySubscription { get; }

    #endregion



    #region Constructors

    private IAPsItemsHandler()
    {
        IStoreManager storeManager = Services.GetService<IStoreManager>();
        NoAds = storeManager.GetStoreItem(NoAdsId);
        WeeklySubscription = storeManager.GetStoreItem(WeeklySubscriptionId);
    }

    #endregion
}