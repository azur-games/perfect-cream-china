#if FIREBASE_ANALYTICS
using Firebase.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.Firebase.Analytics
{
    internal class FirebaseAnalyticsAttribution : IFirebaseAnalytics
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;


        public void SetUserConsent(bool isConsentAvailable)
        {
            CustomDebug.Log($"[Firebase] Requested set user consent to {isConsentAvailable}");
            // According our terms of use and privacy policy 
            // user is not allowed to opt out from analytics
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            
            CustomDebug.Log($"[Firebase] Set user consent to {true}");
        }


        public void LogEvent(string eventName, IDictionary<string, object> parameters = null) { }


        public void SetUserProperty(string propertyName, string propertyValue) { }


        public void SetScreenName(string screenName, string screenClass) { }


        public void SetUserId(string userId)
        {
            FirebaseAnalytics.SetUserId(userId);
        }
    }
}
#endif