#if FIREBASE_CRASHLYTICS
using System;
using System.Threading.Tasks;
using UnityEngine.CrashReportHandler;


namespace Modules.Firebase.Crashlytics
{
    internal class FirebaseCrashlyticsImplementation : IFirebaseCrashlytics
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        
        
        public void SetUserMetadata(string dataKey, string dataValue)
        {
            global::Firebase.Crashlytics.Crashlytics.SetCustomKey(dataKey, dataValue);
            CrashReportHandler.SetUserMetadata(dataKey, dataValue);
        }


        public void SetUserId(string userId)
        {
            global::Firebase.Crashlytics.Crashlytics.SetUserId(userId);
        }


        public void LogException(Exception exception)
        {
            global::Firebase.Crashlytics.Crashlytics.LogException(exception);
        }
    }
}
#endif
