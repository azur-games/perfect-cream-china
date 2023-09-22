using Modules.General;
using System;
using System.Reflection;
using UnityEngine;


namespace Modules.Max
{
    public static class LLMaxManager
    {
        #region Fields

        private static Action<bool> OnCompleteCallback;

        #endregion


        #region Properties

        private static MaxPrivacyManager PrivacyManager { get; set; }

        #endregion


        #region Methods

        public static void Initialize(MaxPrivacyManager privacyManager, Action<bool> onCompleteCallback)
        {
            if (!LLMaxSettings.DoesInstanceExist)
            {
                Debug.LogError("[MaxAdvertisingServiceImplementor - Initialize] Need LLMaxSettings asset to init Max");
                onCompleteCallback(false);
                return;
            }

            if (string.IsNullOrEmpty(LLMaxSettings.Instance.SdkKey))
            {
                Debug.LogError("[MaxAdvertisingServiceImplementor - Initialize] no SdkKey specified in LLMaxSettings");
                onCompleteCallback(false);
                return;
            }

            PrivacyManager = privacyManager;
            OnCompleteCallback = onCompleteCallback;

            MaxSdkCallbacks.OnSdkInitializedEvent += OnMaxSdkInitialized;
            MaxSdk.SetVerboseLogging(Debug.isDebugBuild);
            MaxSdk.SetSdkKey(LLMaxSettings.Instance.SdkKey);
            MaxSdk.InitializeSdk();
            MaxSdk.SetIsAgeRestrictedUser(false);
        }


        public static void ShowMediationDebugger()
        {
            MaxSdk.ShowMediationDebugger();
        }


        private static void ContinueInitialization()
        {
            MaxSdk.SetHasUserConsent(UserConsentManager.Instance.HasUserConsent);
            MaxSdk.SetIsAgeRestrictedUser(false);
            OnCompleteCallback(true);
        }

        #endregion


        #region Event handlers

        private static void OnMaxSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            UserConsentManager.Instance.HandleGdpr(() =>
            {
                PlayerPrefs.SetInt("IABTCF_gdprApplies", UserConsentManager.Instance.HasUserConsent ? 1 : 0);
                PlayerPrefs.Save();

                ContinueInitialization();
            });
        }

        #endregion
    }
}