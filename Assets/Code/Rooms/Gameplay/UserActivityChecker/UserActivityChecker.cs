using Modules.General;
using Modules.General.HelperClasses;
using System;
using UnityEngine;


public class UserActivityChecker
{
    #region Singletone

    private static UserActivityChecker instance;

    public static UserActivityChecker Instance => instance ?? (instance = new UserActivityChecker());

    #endregion



    #region Fields

    private const string LAST_LEAVE_KEY = "last_leave_date";
    private const string LAST_COINS_BOX_KEY = "last_coins_box_date";
    private const string LAST_COINS_BOX_OPEN_KEY = "last_coins_box_open_date";
    private const string LAST_COINS_BOX_FULL_KEY = "last_coins_box_full_date";

    private bool isCoinsBoxPeriodicRewardChecked = false;
    private int lastBucksBoxValue = 0;

    #endregion



    #region Properties

    public DateTime LastLeaveDate
    {
        get
        {
            return CustomPlayerPrefs.GetDateTime(LAST_LEAVE_KEY, DateTime.Now);
        }

        private set
        {
            CustomPlayerPrefs.SetDateTime(LAST_LEAVE_KEY, value, true);
        }
    }


    public TimeSpan TimeSinceLastLeave => DateTime.Now - LastLeaveDate;


    public DateTime LastCoinsBoxPeriodicRewardDate
    {
        get
        {
            return CustomPlayerPrefs.GetDateTime(LAST_COINS_BOX_KEY, DateTime.Now);
        }

        private set
        {
            CustomPlayerPrefs.SetDateTime(LAST_COINS_BOX_KEY, value, true);
        }
    }


    public TimeSpan TimeSinceLastCoinsBoxPeriodicReward => DateTime.Now - LastCoinsBoxPeriodicRewardDate;


    public bool IsCoinsBoxPeriodicRewardAvailable
    {
        get
        {
            bool result = BalanceDataProvider.Instance.IsCoinsBoxEnabled && 
                          !isCoinsBoxPeriodicRewardChecked && 
                          TimeSinceLastCoinsBoxPeriodicReward.TotalHours >= 4.0 && 
                          TimeSinceLastLeave.TotalHours >= 4.0;

            isCoinsBoxPeriodicRewardChecked = true;

            return result;
        }
    }


    public DateTime LastCoinsBoxOpenDate
    {
        get
        {
            return CustomPlayerPrefs.GetDateTime(LAST_COINS_BOX_OPEN_KEY, DateTime.MinValue);
        }

        private set
        {
            CustomPlayerPrefs.SetDateTime(LAST_COINS_BOX_OPEN_KEY, value, true);
        }
    }


    public DateTime LastCoinsBoxFullDate
    {
        get
        {
            return CustomPlayerPrefs.GetDateTime(LAST_COINS_BOX_FULL_KEY, DateTime.MinValue);
        }

        private set
        {
            CustomPlayerPrefs.SetDateTime(LAST_COINS_BOX_FULL_KEY, value, true);
        }
    }

    #endregion



    #region Initialization

    public void Initialize()
    {
        Env.Instance.Inventory.OnBucksBoxValueUpdated += Inventory_OnBucksBoxValueUpdated;
        LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;

        InitializeValues();
    }


    private void InitializeValues()
    {
        lastBucksBoxValue = Env.Instance.Inventory.BucksBox;

        if (!CustomPlayerPrefs.HasKey(LAST_COINS_BOX_KEY))
        {
            LastCoinsBoxPeriodicRewardDate = DateTime.Now;
        }

        if (!CustomPlayerPrefs.HasKey(LAST_COINS_BOX_OPEN_KEY))
        {
            LastCoinsBoxOpenDate = DateTime.Now;
        }
    }

    #endregion



    #region Coins box handling

    public void UpdateCoinsBoxPeriodicRewardTime()
    {
        LastCoinsBoxPeriodicRewardDate = DateTime.Now;
    }


    public void UpdateCoinsBoxOpenTime()
    {
        LastCoinsBoxOpenDate = DateTime.Now;
    }

    #endregion



    #region Events handling

    private void Inventory_OnBucksBoxValueUpdated(int amount, Transform animationRoot, Action callback)
    {
        int maxAmount = BalanceDataProvider.Instance.CoinsBoxMaxAmount;
        if (lastBucksBoxValue < maxAmount && Env.Instance.Inventory.BucksBox >= maxAmount)
        {
            LastCoinsBoxFullDate = DateTime.Now;
        }

        lastBucksBoxValue = Env.Instance.Inventory.BucksBox;
    }


    private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEnteredBackground)
    {
        if (isEnteredBackground)
        {
            LastLeaveDate = DateTime.Now;
            isCoinsBoxPeriodicRewardChecked = false;
        }
    }

    #endregion
}