using Modules.General.HelperClasses;
using UnityEngine;


public class RateUsService : SingletonMonoBehaviour<RateUsService>
{
    #region Fields

    private const string WasRatedKey = "was_rated";
    
    #endregion



    #region Properties

    public bool WasRated => CustomPlayerPrefs.HasKey(WasRatedKey);

    #endregion


    
    #region Public methods
    
    public bool TryToShowNativeRatePopupIOS()
    {
        bool isShowedPopup = false;

        #if UNITY_IOS
            isShowedPopup = UnityEngine.iOS.Device.RequestStoreReview();
        #endif

        if (!isShowedPopup)
        {
            Application.OpenURL(RateUsSettings.Instance.UrlIOS);
        }

        SetRated();
        
        return isShowedPopup;
    }


    public void RateApp()
    {
        #if UNITY_IOS
        Application.OpenURL(RateUsSettings.Instance.UrlIOS);
        #elif UNITY_ANDROID
        Application.OpenURL(RateUsSettings.Instance.UrlAndroid);
        #endif

        SetRated();
    }


    public void SetRated()
    {
        CustomPlayerPrefs.SetBool(WasRatedKey, true);
    }

    #endregion
}