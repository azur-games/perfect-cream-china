using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoGD.UI.PERFECTCREAM
{
    using Modules.General;

    public class WindowWarningInterstitial : UIMessageBox
    {
        [SerializeField] private Button noAdsButton = null;
        [SerializeField] private Button showAdsButton = null;

        private System.Action<bool> onClose = null;
        private string placement;

        public void Init(string placement, System.Action<bool> onClose)
        {
            this.onClose = onClose;
            Time.timeScale = 0;
            this.placement = placement;
            Env.Instance.SendPopup("interstitial_warning", placement, "show");

            Services.AdvertisingManager.CanShowInactivityInterstitial = false;
        }

        private void Start()
        {
            noAdsButton.onClick.AddListener(OnNoAdsClick);
            showAdsButton.onClick.AddListener(OnShowAdsClick);
        }

        private void OnNoAdsClick()
        {
            Time.timeScale = 1;
            Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Subscription, onClose, isStartSubscription: false, placement: "warning_interstitial");
            onClose = null;
            Close();

            Env.Instance.SendPopup("interstitial_warning", placement, "click_no_ads");
        }

        private void OnShowAdsClick()
        {
            Time.timeScale = 1;
            Close();
            onClose?.Invoke(true);
            onClose = null;
            Services.AdvertisingManager.CanShowInactivityInterstitial = true;
            Env.Instance.SendPopup("interstitial_warning", placement, "click_show_ads");
        }
    }
}