using Modules.General.Abstraction.InAppPurchase;
using Modules.InAppPurchase;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Modules.General
{
    public class SubscriptionPopup : MonoBehaviour
    {
        #region Helpers

        [Serializable]
        public class SubscriptionButtonSettings
        {
            [HideInInspector] public IStoreItem storeItem;
            public SubscriptionType subscriptionType;
            public Button subscriptionButton;
            public TextMeshProUGUI trialPriceLabel;
            public TextMeshProUGUI priceLabel;
        }

        #endregion



        #region Fields

        private const string PrivacyPolicyUrl = "https://aigames.ae/policy#h.hn0lb3lfd0ij";
        private const string TermsOfUseUrl = "https://aigames.ae/policy#h.v7mztoso1wgw";

        private const string SubscriptionDescription =
            "Weekly Premium automatically renews for {0} per week after the 3-day free trial. Payment will be charged to your " +
            "{1} account at the end of the trial period of purchase. The subscription automatically renews unless auto-renew is " +
            "turned off at least 24 hours before the end of the current period. Your account will be charged for renewal within 24" +
            " hours prior to the end of the current period. You can manage and turn off auto-renewal of the subscription by going " +
            "to your account settings on the {2} after purchase. Any unused portion of a free trial period will be forfeited when " +
            "the user purchases a subscription to that publication, where applicable.";

        private const string PriceDescription = "Then {0} per week";

        protected const string AccountType =
            #if UNITY_IOS
                "Apple ID";
            #elif HIVE_HUAWEI
                "Huawei";
            #else
                "Google";
        #endif

        protected const string StoreType =
            #if UNITY_IOS
                "App Store";
            #elif HIVE_HUAWEI
                "Huawei AppGallery";
            #else
                "Google Play";
        #endif

        [Header("Inapps Settings")]
        [SerializeField] private List<SubscriptionButtonSettings> subscriptionButtonSettings = null;

        [Header("UI Buttons Settings")]
        [SerializeField] private Button closeButton = null;
        [SerializeField] private Button restoreButton = null;
        [SerializeField] private Button privacyButton = null;
        [SerializeField] private Button termsButton = null;
        [SerializeField] private GameObject loading = null;

        [Header("Animation Settings")]
        [SerializeField] private Animator animator = null;
        [SerializeField] private AnimationClip showSubscriptionClip = null;
        [SerializeField] private AnimationClip idleSubscriptionClip = null;
        [SerializeField] private AnimationClip hideSubscriptionClip = null;

        [Header("Descriptions Settings")]
        [SerializeField] protected TextMeshProUGUI descriptionLabel;

        private Action<SubscriptionPopupResult> closeCallback = null;

        protected bool isStartSubscription = false;

        protected SubscriptionPopupResult closeResult =
            SubscriptionPopupResult.None;
        protected IStoreManager storeManager;
        protected IStoreSettings storeSettings;

        #endregion



        #region Unity lifecycle

        private void OnEnable()
        {
            closeButton?.onClick.AddListener(CloseButton_OnClick);
            restoreButton?.onClick.AddListener(RestoreButton_OnClick);
            privacyButton?.onClick.AddListener(PrivacyButton_OnClick);
            termsButton?.onClick.AddListener(TermsButton_OnClick);

            storeManager = Services.GetService<IStoreManager>();
            storeSettings = Services.GetService<IStoreSettings>();

            foreach (SubscriptionButtonSettings settings in subscriptionButtonSettings)
            {
                settings.storeItem = storeSettings.GetStoreItems(storeManager).Find(storeItem => storeItem.SubscriptionType == settings.subscriptionType);

                if (settings.storeItem != null)
                {
                    settings.subscriptionButton?.onClick.AddListener(() =>
                        SubscriptionButton_OnClick(settings.storeItem));
                    UpdateButtonState(settings);
                }
            }

            storeManager.ItemDataReceived += OnItemDataReceived;
        }


        private void OnDisable()
        {
            closeButton?.onClick.RemoveListener(CloseButton_OnClick);
            restoreButton?.onClick.RemoveListener(RestoreButton_OnClick);
            privacyButton?.onClick.RemoveListener(PrivacyButton_OnClick);
            termsButton?.onClick.RemoveListener(TermsButton_OnClick);

            storeManager.ItemDataReceived -= OnItemDataReceived;
        }

        #endregion



        #region Methods

        public virtual void Show(bool isStartSubscription, Action<SubscriptionPopupResult> callback, string placement = null)
        {
            this.isStartSubscription = isStartSubscription;
            animator.Play(showSubscriptionClip.name);
            closeCallback = callback;
        }


        public virtual void Hide(SubscriptionPopupResult result)
        {
            closeResult = result;
            animator.Play(hideSubscriptionClip.name);
        }


        protected void PurchaseProcess(bool inProgress)
        {
            loading?.SetActive(inProgress);
        }


        protected virtual void UpdateButtonState(SubscriptionButtonSettings buttonSettings)
        {
            if (buttonSettings != null)
            {
                if (buttonSettings.priceLabel != null)
                {
                    buttonSettings.priceLabel.text = (string.IsNullOrEmpty(buttonSettings.storeItem.LocalizedPrice)) ?
                        ($"${buttonSettings.storeItem.TierPrice:F2}") :
                        (buttonSettings.storeItem.LocalizedPrice);
                }

                if (buttonSettings.trialPriceLabel != null)
                {
                    buttonSettings.trialPriceLabel.text = string.Format(PriceDescription,
                        (string.IsNullOrEmpty(buttonSettings.storeItem.LocalizedPrice)) ?
                            ($"${buttonSettings.storeItem.TierPrice:F2}") :
                            (buttonSettings.storeItem.LocalizedPrice));
                }

                TryUpdateDescriptionState(buttonSettings);
            }
        }


        protected virtual void TryUpdateDescriptionState(SubscriptionButtonSettings settings)
        {
            if (settings.subscriptionType == SubscriptionType.Weekly)
            {
                descriptionLabel.text = string.Format(SubscriptionDescription,
                    (string.IsNullOrEmpty(settings.storeItem.LocalizedPrice)) ?
                        ($"${settings.storeItem.TierPrice:F2}") :
                        (settings.storeItem.LocalizedPrice), AccountType, StoreType);
            }
        }


        protected virtual bool CheckIfPopupNeeded(out string message)
        {
            message = null;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return true;
            }
            if (!Services.StoreManager.IsInitialized)
            {
                message = "Store is not initialized yet. Try after 5 seconds";
                return true;
            }
            return false;
        }

        #endregion



        #region Events hadler

        public void CloseButton_OnClick()
        {
            Hide(SubscriptionPopupResult.SubscriptionClosed);
        }


        public virtual void RestoreButton_OnClick()
        {
            if (CheckIfPopupNeeded(out string message))
            {
                PopupManager.Instance.ShowMessagePopup(message: message, messageHandler: transform);
                return;
            }

            PurchaseProcess(true);
            storeManager.RestorePurchases(result =>
            {
                PurchaseProcess(false);

                PopupManager.Instance.ShowMessagePopup((callback) =>
                {
                    if (result.IsSucceeded && storeManager.HasAnyActiveSubscription)
                    {
                        Hide(SubscriptionPopupResult.SubscriptionRestored);
                    }
                }, ((result.IsSucceeded) ? ("label_purchases_restored") : ("Cannot connect to application store")), messageHandler: transform);
            });
        }


        public void PrivacyButton_OnClick()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopupManager.Instance.ShowMessagePopup(messageHandler: transform);

                return;
            }

            Application.OpenURL(PrivacyPolicyUrl);
        }


        public void TermsButton_OnClick()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PopupManager.Instance.ShowMessagePopup(messageHandler: transform);

                return;
            }

            Application.OpenURL(TermsOfUseUrl);
        }


        public virtual void SubscriptionButton_OnClick(IStoreItem storeItem)
        {
        }


        // Must coincide with animation event name or redeclared
        public virtual void SubscriptionShown()
        {
            animator.Play(idleSubscriptionClip.name);
        }


        // Must coincide with animation event name or redeclared
        public virtual void SubscriptionHiden()
        {
            closeCallback?.Invoke(closeResult);
            closeCallback = null;
        }


        private void OnItemDataReceived(IStoreItem item)
        {
            UpdateButtonState(subscriptionButtonSettings.Find(i => i.storeItem == item));
        }

        #endregion
    }
}
