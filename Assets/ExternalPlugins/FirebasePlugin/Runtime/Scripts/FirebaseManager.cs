#if FIREBASE_CORE
using Firebase.Extensions;
#endif
using Modules.Firebase.Analytics;
using Modules.Firebase.Authentication;
using Modules.Firebase.Crashlytics;
using Modules.Firebase.Database;
using Modules.Firebase.Initialization;
using Modules.Firebase.Installations;
using Modules.Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Firebase
{
    public static class FirebaseManager
    {
        #region Fields

        private static readonly Dictionary<Type, IFirebaseModule> FirebaseModules = new Dictionary<Type, IFirebaseModule>()
        {
            #if FIREBASE_ANALYTICS
            { typeof(IFirebaseAnalytics), new FirebaseAnalyticsImplementation() },
            #else
            { typeof(IFirebaseAnalytics), new FirebaseAnalyticsDummy() },
            #endif
            #if FIREBASE_AUTHENTICATION
            { typeof(IFirebaseAuthentication), new FirebaseAuthenticationImplementation() },
            #else
            { typeof(IFirebaseAuthentication), new FirebaseAuthenticationDummy() },
            #endif
            #if FIREBASE_CRASHLYTICS
            { typeof(IFirebaseCrashlytics), new FirebaseCrashlyticsImplementation() },
            #else
            { typeof(IFirebaseCrashlytics), new FirebaseCrashlyticsDummy() },
            #endif
            #if FIREBASE_DATABASE
            { typeof(IFirebaseDatabase), new FirebaseDatabaseImplementation() },
            #else
            { typeof(IFirebaseDatabase), new FirebaseDatabaseDummy() },
            #endif
            #if FIREBASE_CORE
            { typeof(IFirebaseInitialization), new FirebaseInitializationImplementation() },
            #else
            { typeof(IFirebaseInitialization), new FirebaseInitializationDummy() },
            #endif
            #if FIREBASE_REMOTE_CONFIG
            { typeof(IFirebaseRemoteConfig), new FirebaseRemoteConfigImplementation() },
            #else
            { typeof(IFirebaseRemoteConfig), new FirebaseRemoteConfigDummy() },
            #endif
            #if FIREBASE_INSTALLATIONS
            { typeof(IFirebaseInstallations), new FirebaseInstallationsImplementation() },
            #else
            { typeof(IFirebaseInstallations), new FirebaseInstallationsDummy() },
            #endif
        };
        
        public static event Action OnInitialize;
        private static bool isConsentSpecified = false;

        #endregion
        


        #region Initialization

        public static void SetUserConsent(bool isConsentAvailable)
        {
            // According our terms of use and privacy policy 
            // user is not allowed to opt out from analytics
            GetModule<IFirebaseAnalytics>().SetUserConsent(isConsentAvailable);
            
            isConsentSpecified = true;
        }
        
        public static void Initialize(LLFirebaseSettings settings = null)
        {
            if (!isConsentSpecified)
            {
                Debug.LogError("You should call SetUserConsent method before Initialize!");
            
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                return;
            }

            // By default we don't allow send game events to the Firebase analytics
            #if FIREBASE_ANALYTICS
                if (settings == null || !settings.ShouldSendGameAnalytics)
                {
                    FirebaseModules[typeof(IFirebaseAnalytics)] = new FirebaseAnalyticsAttribution();
                }
            #endif

            List<IFirebaseModule> modules = new List<IFirebaseModule>(FirebaseModules.Values);
            modules.Sort((module, firebaseModule) =>
            {
                if (module.InitializationPriority > firebaseModule.InitializationPriority)
                {
                    return -1;
                }
                return module.InitializationPriority < firebaseModule.InitializationPriority ? 1 : 0;
            });
            
            #if FIREBASE_CORE
                InitializeModule(0);
               
                async void InitializeModule(int moduleIndex)
                {
                    if (moduleIndex < modules.Count)
                    {
                        List<Task> tasks = new List<Task> { modules[moduleIndex].Initialize(settings) };
                        int i = moduleIndex + 1;
                        while (i < modules.Count)
                        {
                            IFirebaseModule currentModule = modules[i];
                            if (currentModule.InitializationPriority == modules[i - 1].InitializationPriority)
                            {
                                tasks.Add(currentModule.Initialize(settings));
                            }
                            else
                            {
                                break;
                            }
                            
                            i++;
                        }
                        await Task.WhenAll(tasks).ContinueWithOnMainThread(_ =>
                        {
                            InitializeModule(i);
                        });
                    }
                    else
                    {
                        OnInitialize?.Invoke();
                    }
                }
            #endif
        }

        #endregion
        


        #region Analytics

        public static void LogEvent(string eventName, IDictionary<string, object> parameters = null)
        {
            GetModule<IFirebaseAnalytics>().LogEvent(eventName, parameters);
        }
        
        
        public static void SetUserProperty(string propertyName, string propertyValue)
        {
            GetModule<IFirebaseAnalytics>().SetUserProperty(propertyName, propertyValue);
        }
        
        
        public static void SetScreenName(string screenName, string screenClass)
        {
            GetModule<IFirebaseAnalytics>().SetScreenName(screenName, screenClass);
        }
        
        
        public static void SetUserId(string userId)
        {
            GetModule<IFirebaseAnalytics>().SetUserId(userId);
        }

        #endregion
        
        
        
        #region Authentication
        
        public static bool IsLoggedIn => GetModule<IFirebaseAuthentication>().IsLoggedIn;
        

        public static void SignIn(string token, Action<bool> callback)
        {
            GetModule<IFirebaseAuthentication>().SignIn(token, callback);
        }

        #endregion



        #region RemoteConfig

        public static float GetRemoteSettingsFloat(string key)
        {
            return GetModule<IFirebaseRemoteConfig>().GetRemoteSettingsFloat(key);
        }
        
        
        public static int GetRemoteSettingsInt(string key)
        {
            return GetModule<IFirebaseRemoteConfig>().GetRemoteSettingsInt(key);
        }
        
        
        public static bool GetRemoteSettingsBool(string key)
        {
            return GetModule<IFirebaseRemoteConfig>().GetRemoteSettingsBool(key);
        }
        
        
        public static string GetRemoteSettingsString(string key)
        {
            return GetModule<IFirebaseRemoteConfig>().GetRemoteSettingsString(key);
        }

        #endregion



        #region Database

        public static void SetUserData(string data, Action<bool> callback)
        {
            GetModule<IFirebaseDatabase>().SetUserData(data, callback);
        }
        
        
        public static void GetUserData(Action<string> callback)
        {
            GetModule<IFirebaseDatabase>().GetUserData(callback);
        }

        #endregion

        
        
        #region Crashlytics
        
        public static void SetCrashlyticsUserMetadata(string dataKey, string dataValue)
        {
            GetModule<IFirebaseCrashlytics>().SetUserMetadata(dataKey, dataValue);
        }

        
        public static void SetCrashlyticsUserId(string userId)
        {
            GetModule<IFirebaseCrashlytics>().SetUserId(userId);
        }
        
        
        public static void LogException(Exception exception)
        {
            GetModule<IFirebaseCrashlytics>().LogException(exception);
        }
        
        #endregion
        
        

        #region Installations

        public static void FetchInstanceId(Action<string> callback)
        {
            GetModule<IFirebaseInstallations>().FetchInstanceId(callback);
        }
   
        #endregion
        
        
        
        #region Methods

        private static T GetModule<T>() where T : class, IFirebaseModule
        {
            T result = null;
            if (FirebaseModules.ContainsKey(typeof(T)))
            {
                result = (T)FirebaseModules[typeof(T)];
            }
            
            return result;
        }

        #endregion
    }
}
