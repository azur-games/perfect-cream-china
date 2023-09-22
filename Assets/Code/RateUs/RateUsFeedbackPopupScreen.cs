using Modules.Advertising;
using Modules.General.Abstraction;
using UnityEngine;


public class RateUsFeedbackPopupScreen : UIMessageBox
{
    public static void ShowRatePopupIfCan()
    {
        RateUsFeedbackPopupScreen rateUsFeedbackPopupScreen = Env.Instance.UI.Messages.ShowRateUs();
        AdvertisingManager.Instance.LockAd(AdModule.Interstitial, "RateUs");
        RateUsPopup.Show(rateUsFeedbackPopupScreen.rootTransform, rateUsFeedbackPopupScreen.CloseSelf);
    }

    #region Fields
    
    [SerializeField] private Transform rootTransform;

    #endregion



    #region Methods

    private void CloseSelf()
    {
        AdvertisingManager.Instance.UnlockAd(AdModule.Interstitial, "RateUs");

        Close();
    }

    #endregion
}
