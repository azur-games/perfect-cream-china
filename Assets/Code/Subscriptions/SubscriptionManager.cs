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


    public bool IsRewardPopupAvailable
    {
        get
        {
            return StoreManager.HasAnyActiveSubscription && SubscriptionDaysForReward > 0;
        }
    }

    public bool IsSubscriptionActive => StoreManager.HasAnyActiveSubscription;

    public int SubscriptionDaysForReward
    {
        get
        {
            DateTime lastReward = LastSubscriptionRewardDate;
            DateTime checkedDate = DateTime.UtcNow;
            TimeSpan span = new TimeSpan();

            if (lastReward != DateTime.MinValue)
            {
                ISubscriptionInfo lastSubscriptionReward = StoreManager.GetSubscriptionsForDate(lastReward).FirstOrDefault();
                ISubscriptionInfo currentSubscriptionReward = StoreManager.GetSubscriptionsForDate(checkedDate).FirstOrDefault();

                if (lastSubscriptionReward == null || currentSubscriptionReward == null)
                {
                    return 0;
                }

                if (lastSubscriptionReward.ProductId == currentSubscriptionReward.ProductId)
                {
                    span = checkedDate - lastReward;
                }
                else
                {
                    TimeSpan lastSpan = lastSubscriptionReward.ExpirationDate - lastReward;
                    TimeSpan currentSpan = checkedDate - currentSubscriptionReward.PurchaseDate;
                    span = lastSpan + currentSpan;
                }
            }
            else
            {
                ISubscriptionInfo result = StoreManager.GetLastSubscription();

                if (result != null && !string.IsNullOrEmpty(result.ProductId))
                {
                    LastSubscriptionRewardDate = result.PurchaseDate;
                    span = checkedDate - LastSubscriptionRewardDate;

                    return (StoreManager.IsSandboxEnvironment ? (span.Minutes / 2) : (span.Days)) + 1;
                }
            }

            return StoreManager.IsSandboxEnvironment ? (span.Minutes / 2) : (span.Days);
        }
    }


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
        DateTime checkedDate = DateTime.UtcNow;
        ISubscriptionInfo currentSubscriptionReward = StoreManager.GetSubscriptionsForDate(checkedDate).FirstOrDefault();
        TimeSpan span = checkedDate - LastSubscriptionRewardDate;

        if (currentSubscriptionReward != null && !string.IsNullOrEmpty(currentSubscriptionReward.ProductId))
        {
            LastSubscriptionRewardDate = StoreManager.IsSandboxEnvironment ?
                LastSubscriptionRewardDate.AddMinutes((span.Minutes / 2) * 2.0f) :
                LastSubscriptionRewardDate.AddDays(span.Days);
        }
        else
        {
            LastSubscriptionRewardDate = checkedDate;
        }
    }

    #endregion
}