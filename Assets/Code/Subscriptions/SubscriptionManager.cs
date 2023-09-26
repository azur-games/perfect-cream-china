using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.General.Utilities;
using Modules.Hive.Ioc;
using Modules.InAppPurchase;
using System;
using System.Linq;


public enum SubscriptionPopupResult
{
    None = 0,
    SubscriptionPurchased = 1,
    SubscriptionRestored = 2,
    SubscriptionClosed = 3
}


[InitQueueService(10, bindTo: typeof(ISubscriptionRewardManager))]
public class SubscriptionManager : IInitializableService, ISubscriptionRewardManager
{
    #region Fields

    private const string SubscriptionRewardDateKey = "subscription_reward_date";

    private static ISubscriptionRewardManager instance;
    private IStoreManager storeManager;

    #endregion



    #region Properties

    public static ISubscriptionRewardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Services.GetService<ISubscriptionRewardManager>() ?? new SubscriptionManager();
            }

            return instance;
        }
    }


    public IStoreManager StoreManager
    {
        get
        {
            if (storeManager == null)
            {
                storeManager = Services.GetService<IStoreManager>();
            }

            return storeManager;
        }
    }

    public bool IsSubscriptionActive => StoreManager.HasAnyActiveSubscription;


    private DateTime LastSubscriptionRewardDate
    {
        get
        {
            return CustomPlayerPrefs.GetDateTime(SubscriptionRewardDateKey, DateTime.MinValue);
        }
        set
        {
            CustomPlayerPrefs.SetDateTime(SubscriptionRewardDateKey, value);
        }
    }

    #endregion



    #region Methods

    public void InitializeService(IServiceContainer container, Action onCompleteCallback, Action<IInitializableService, InitializationStatus> onErrorCallback)
    {
        onCompleteCallback?.Invoke();
    }


    public void ClaimReward()
    {

    }

    #endregion

    public bool IsRewardPopupAvailable { get; }
}