using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.General.ServicesInitialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Modules.Firebase
{
    public class FirebaseAnalyticsServiceImplementor : IAnalyticsService
    {
        #region Fields

        public event Action<IInitializable, InitializationStatus> OnServiceInitialized;
        private LLFirebaseSettings settings;
        private bool isInitialized = false;
        private string savedDeviceId = null;
        
        #endregion



        #region Properties

        public bool IsAsyncInitializationEnabled => false; // Necessary to call in main thread.
        
        public bool IsAsyncWorkAvailable => true;

        #endregion



        #region Class lifecycle

        public FirebaseAnalyticsServiceImplementor(LLFirebaseSettings firebaseSettings = null) => settings = firebaseSettings;

        #endregion



        #region Initialize

        public void PreInitialize() { }

        
        public void SetUserConsent(bool isConsentAvailable)
        {
            FirebaseManager.SetUserConsent(isConsentAvailable);
        }

        
        public void Initialize()
        {
            FirebaseManager.OnInitialize += FirebaseManager_OnInitialize;
            FirebaseManager.Initialize(settings);
        }
        
        #endregion



        #region Analytics

        public void SendEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            Dictionary<string, object> firebaseParameters = parameters?.ToDictionary(
                pair => pair.Key,
                pair => (object)pair.Value);
            
            if (IsAsyncWorkAvailable)
            {
                Task.Run(() =>
                {
                    FirebaseManager.LogEvent(eventName, firebaseParameters);
                });
            }
            else
            {
                FirebaseManager.LogEvent(eventName, firebaseParameters);
            }
        }


        public void SetUserProperty(string propertyName, string propertyValue)
        {
            FirebaseManager.SetUserProperty(propertyName, propertyValue);
        }


        public void SetScreenName(string screenName, string screenClass)
        {
            FirebaseManager.SetScreenName(screenName, screenClass);
        }
        
        
        public void SetDeviceId(string userId)
        {
            savedDeviceId = userId;
            if (isInitialized)
            {
                FirebaseManager.SetUserId(savedDeviceId);
            }
        }

        #endregion



        #region Remote Config

        public float GetRemoteSettingsFloat(string key)
        {
            return FirebaseManager.GetRemoteSettingsFloat(key);
        }
        
        
        public int GetRemoteSettingsInt(string key)
        {
            return FirebaseManager.GetRemoteSettingsInt(key);
        }


        public bool GetRemoteSettingsBool(string key)
        {
            return FirebaseManager.GetRemoteSettingsBool(key);
        }


        public string GetRemoteSettingsString(string key)
        {
            return FirebaseManager.GetRemoteSettingsString(key);
        }

        #endregion



        #region Authentication

        public static bool IsLoggedIn => FirebaseManager.IsLoggedIn;


        public static void SignIn(string token, Action<bool> callback)
        {
            FirebaseManager.SignIn(token, callback);
        }

        #endregion



        #region Database

        public static void SetUserData(string data, Action<bool> callback)
        {
            FirebaseManager.SetUserData(data, callback);
        }


        public static void GetUserData(Action<string> callback)
        {
            FirebaseManager.GetUserData(callback);
        }

        #endregion


        
        #region Crashlytics
        
        public void SetCrashlyticsUserMetadata(string dataKey, string dataValue)
        {
            FirebaseManager.SetCrashlyticsUserMetadata(dataKey, dataValue);
        }

        
        public void SetCrashlyticsUserId(string userId)
        {
            FirebaseManager.SetCrashlyticsUserId(userId);
        }
        
        
        public void LogException(Exception exception)
        {
            FirebaseManager.LogException(exception);
        }
        
        #endregion
        
        
        
        #region Installations

        public void FetchInstanceId(Action<string> callback)
        {
            FirebaseManager.FetchInstanceId(callback);
        }
   
        #endregion
        
        

        #region Events handlers

        private void FirebaseManager_OnInitialize()
        {
            FirebaseManager.OnInitialize -= FirebaseManager_OnInitialize;
            isInitialized = true;
            
            Action<string> deviceIdAction = deviceId =>
            {
                SetDeviceId(savedDeviceId);
                OnServiceInitialized?.Invoke(this, InitializationStatus.Success);
            };
            
            if (string.IsNullOrEmpty(savedDeviceId))
            {
                DeviceInfo.RequestDeviceId(deviceIdAction);
            }
            else
            {
                deviceIdAction(savedDeviceId);
            }
        }

        #endregion
    }
}
