using Modules.Advertising;
using Modules.General.Abstraction;
using System;
using UnityEngine;


public class RequestVideoShowController : MonoBehaviour
{
    public const float WAITING_PERIOD = 2.0f;

    private static RequestVideoShowController instance = null;
    public static RequestVideoShowController Instance
    {
        get
        {
            if (null == instance)
            {
                GameObject go = new GameObject("RequestVideoShowController");
                instance = go.AddComponent<RequestVideoShowController>();
            }

            return instance;
        }
    }

    #region Fields

    private Action<AdActionResultType> currentCallBack;
    private Action startShowingCallback;
    private Action cancelRewardVideoCallback;
    private string adShowingPlacement;
    private DateTime startTime;

    #endregion



    #region Unity lifecycle

    private void Update()
    {
        if (currentCallBack != null &&
            AdvertisingManager.Instance.IsAdModuleByPlacementAvailable(AdModule.RewardedVideo, adShowingPlacement))
        {
            startShowingCallback?.Invoke();
            AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, adShowingPlacement, (type) => 
            {
                currentCallBack?.Invoke(type);
            });

            this.gameObject.SetActive(false);
            return;
        }

        if ((DateTime.Now - startTime).TotalSeconds > WAITING_PERIOD)
        {
            Action<AdActionResultType> callbackCached = currentCallBack;
            CancelRequest();
            callbackCached?.Invoke(AdActionResultType.NoInternet);
        }
    }

    #endregion



    #region Public methods

    public void ShowVideo(Action startShowingCallback, Action cancelRewardVideoCallback,
        Action<AdActionResultType> currentCallBack, string adShowingPlacement = "")
    {
        this.startShowingCallback = startShowingCallback;
        this.cancelRewardVideoCallback = cancelRewardVideoCallback;
        this.currentCallBack = currentCallBack;
        this.adShowingPlacement = adShowingPlacement;

        startTime = DateTime.Now;
        this.gameObject.SetActive(true);
    }


    public void CancelRequest()
    {
        Action cancelRewardVideoCallback_cached = cancelRewardVideoCallback;
        cancelRewardVideoCallback = null;

        this.gameObject.SetActive(false);

        cancelRewardVideoCallback_cached?.Invoke();

        startShowingCallback = null;
        currentCallBack = null;
    }

    #endregion
}
