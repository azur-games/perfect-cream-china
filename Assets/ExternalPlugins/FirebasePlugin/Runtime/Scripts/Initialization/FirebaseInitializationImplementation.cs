#if FIREBASE_CORE
using Firebase;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Firebase.Initialization
{
    internal class FirebaseInitializationImplementation : IFirebaseInitialization
    {
        private FirebaseApp firebaseApp;
        
        
        public int InitializationPriority => 100;


        public Task Initialize(LLFirebaseSettings settings = null)
        {
            return FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    firebaseApp = FirebaseApp.DefaultInstance;
                    
                    if (CustomDebug.Enable)
                    {
                        FirebaseApp.LogLevel = LogLevel.Debug;
                    }
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
    }
}
#endif
