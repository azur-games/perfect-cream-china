using Modules.General;
using Modules.General.HelperClasses;
using System;
using UnityEngine;


public class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    #region Fields

    public event Action<bool> OnSubscriptionPopupVisibilityChange;

    private Action<SubscriptionPopupResult> popupCloseCallback = null;

    [SerializeField] private SubscriptionPopup startSubscriptionExtendedPopupPrefab = default;
    [SerializeField] private SubscriptionPopup startSubscriptionPopupPrefab = default;
    [SerializeField] private RewardPopup successfulSubscriptionPopupPrefab = default;
    [SerializeField] private RewardPopup rewardSubscriptionPopupPrefab = default;
    [SerializeField] private MessagePopup messagePopupPrefab = default;
    [SerializeField] private BasePopup noInternetPopupPrefab = default;

    private const string IsStartSubscriptionShownKey = "is_start_subscription_shown";

    private SubscriptionPopup subscriptionPopup = null;
    private RewardPopup rewardPopup = null;

    #endregion



    #region Properties

    public bool IsSubscriptionPopupActive => subscriptionPopup != null;


    public bool IsRewardPopupActive => rewardPopup != null;


    public bool IsStartSubscriptionShown
    {
        get { return CustomPlayerPrefs.GetBool(IsStartSubscriptionShownKey, false); }
        set { CustomPlayerPrefs.SetBool(IsStartSubscriptionShownKey, value); }
    }

    #endregion



    #region Methods

    public void ShowSubscription(Action<SubscriptionPopupResult> resultCallback,
        bool isStartSubscription, Transform newHandler = null, string placement = null)
    {
        CloseSubscriptionPopup();
        if (newHandler != null)
        {
            if (startSubscriptionPopupPrefab == null)
            {
                CustomDebug.LogError("startSubscriptionPopupPrefab is null!");

                return;
            }

            subscriptionPopup = Instantiate(startSubscriptionPopupPrefab, newHandler);

            popupCloseCallback = resultCallback;

            subscriptionPopup?.Show(isStartSubscription, (callback) => { CloseSubscriptionPopup(callback); }, placement);
            OnSubscriptionPopupVisibilityChange?.Invoke(true);
        }
        else
        {
            CustomDebug.LogError("You should call Initialize method before ShowSubscription!");
        }
    }


    public void TryShowSubscriptionReward(string rewardDescription, Action<SubscriptionRewardResult> claimCallback,
        Transform newHandler = null)
    {
        if (newHandler != null)
        {
            if (rewardSubscriptionPopupPrefab == null)
            {
                CustomDebug.LogError("rewardSubscriptionPopupPrefab is null!");
                claimCallback?.Invoke(SubscriptionRewardResult.RewardNotAvailable);
            }
            else if (SubscriptionManager.Instance.IsRewardPopupAvailable)
            {
                rewardPopup = Instantiate(rewardSubscriptionPopupPrefab, newHandler);
                Env.Instance.SendPopup("subscription_reward", "reward", "show");
                rewardPopup.Show(rewardDescription, popupResult =>
                {
                    if (rewardPopup != null)
                    {
                        SubscriptionRewardResult result = popupResult ?
                            SubscriptionRewardResult.Claimed :
                            SubscriptionRewardResult.Skipped;
                        claimCallback?.Invoke(result);
                        Destroy(rewardPopup.gameObject);
                        rewardPopup = null;
                        if (popupResult)
                        {
                            SubscriptionManager.Instance.ClaimReward();

                            Env.Instance.SendPopup("subscription_reward", "reward", "claim");
                        }
                    }
                });
            }
            else
            {
                claimCallback?.Invoke(SubscriptionRewardResult.RewardNotAvailable);
            }
        }
        else
        {
            CustomDebug.LogError("You should call Initialize method before trying to show RewardPopup!");
            claimCallback?.Invoke(SubscriptionRewardResult.RewardNotAvailable);
        }
    }


    public void ShowExtendedSubscription(Action<SubscriptionPopupResult> resultCallback,
        bool isStartSubscription, Transform newHandler = null)
    {
        CloseSubscriptionPopup();
        if (newHandler != null)
        {
            if (startSubscriptionPopupPrefab == null)
            {
                Debug.LogError("startSubscriptionExtendedPopupPrefab is null!");

                return;
            }

            subscriptionPopup = Instantiate(startSubscriptionExtendedPopupPrefab, newHandler);

            popupCloseCallback = resultCallback;

            subscriptionPopup?.Show(isStartSubscription, CloseSubscriptionPopup);
            OnSubscriptionPopupVisibilityChange?.Invoke(true);
        }
        else
        {
            Debug.LogError("You should call Initialize method before ShowExtendedSubscription!");
        }
    }


    public void ShowSuccessfulSubscriptionPopup(string rewardDescription, Action<bool> claimCallback,
        Transform popupHandler = null)
    {
        if (popupHandler != null)
        {
            if (successfulSubscriptionPopupPrefab == null)
            {
                Debug.LogError("successfulSubscriptionPopupPrefab is null!");
                claimCallback?.Invoke(false);
            }
            else
            {
                RewardPopup popup = Instantiate(successfulSubscriptionPopupPrefab, popupHandler);

                popup.Show(rewardDescription, (result) => 
                {
                    if (popup != null)
                    {
                        claimCallback?.Invoke(result);
                        Destroy(popup.gameObject);
                    }
                });
            }
        }
        else
        {
            Debug.LogError("You should call Initialize method before trying to show SuccessfulSubscriptionPopup!");
            claimCallback?.Invoke(false);
        }
    }


    //use null message if has custom nointernet popup
    public void ShowMessagePopup(Action<bool> callback = null, string message = null, Transform messageHandler = null)
    {
        if (messageHandler != null)
        {
            if (string.IsNullOrEmpty(message))
            {
                if (noInternetPopupPrefab != null)
                {
                    BasePopup popup = Instantiate(noInternetPopupPrefab, messageHandler);
                    popup.Show(callback);
                }
                else
                {
                    message = "No internet connection";
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessagePopup popup = Instantiate(messagePopupPrefab, messageHandler);
                popup.Show(callback, message);
            }
        }
        else
        {
            Debug.LogError("You should set handler before trying to show MessagePopup!");
        }
    }


    private void CloseSubscriptionPopup(SubscriptionPopupResult result = SubscriptionPopupResult.None)
    {
        if (subscriptionPopup != null)
        {
            if (result != SubscriptionPopupResult.None)
            {
                popupCloseCallback?.Invoke(result);
                popupCloseCallback = null;
                    
                OnSubscriptionPopupVisibilityChange?.Invoke(false);
            }

            Destroy(subscriptionPopup.gameObject);
            subscriptionPopup = null;
        }
    }

    #endregion
}