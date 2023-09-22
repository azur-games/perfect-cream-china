using Modules.General;
using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.HelperClasses;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Modules.Max
{
    [InitQueueService(-100, typeof(IPrivacyManager))]
    public class MaxPrivacyManager : MonoBehaviour, IPrivacyManager, IInitializableService
    {
        #region Fields

        private const string SetUserConsentMethodName = "SetUserConsent";
        private const string WasPrivacyPopupsShownKey = "was_max_privacy_shown";
        private const string PrivacyUrl = "https://aigames.ae/policy#privacy";
        private const string TermsUrl = "https://aigames.ae/policy#terms";

        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject eventSystem;
        private Action<bool, bool> gdprCallback;
        
        #endregion



        #region Properties

        public bool WasPrivacyPopupsShown
        {
            get => CustomPlayerPrefs.GetBool(WasPrivacyPopupsShownKey, false);
            set => CustomPlayerPrefs.SetBool(WasPrivacyPopupsShownKey, value);
        }

        public bool IsPrivacyButtonAvailable => true;
        
        public bool WasPersonalDataDeleted { get; set; }
        
        public void GetTermsAndPolicyURI(Action<bool, string> callback)
        {
            callback(true, LLMaxSettings.Instance.TermsUrl);
        }

        #endregion



        #region Methods

        public void InitializeService(IServiceContainer container, Action onCompleteCallback, Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            LLMaxManager.Initialize(this, (isSuccess) =>
            {
                if (isSuccess)
                {
                    SetUserConsent(MaxSdk.HasUserConsent() || !MaxSdk.HasUserConsent());
                    
                    // Azure requirement
                    Scheduler.Instance.CallMethodWithDelay(Scheduler.Instance, () =>
                    {
                        onCompleteCallback?.Invoke();
                    }, 2.0f);
                }
                else
                {
                    onErrorCallback?.Invoke(this, InitializationStatus.Failed);
                }
            });
        }
        
        private void SetUserConsent(bool isConsentAvailable)
        {
            object[] parameters = {isConsentAvailable};

            foreach (string className in LLMaxSettings.Instance.ConsentApiClassesNamesIncludingAssemblies)
            {
                Type classType = Type.GetType(className);
            
                if (classType != null)
                {
                    MethodInfo currentMethod = classType.GetMethod(SetUserConsentMethodName);
            
                    if (currentMethod != null)
                    {
                        currentMethod.Invoke(classType, parameters);
                    }
                }
            }
        }
        
        #endregion
    }
}
