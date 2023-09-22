#if FIREBASE_ANALYTICS
using Firebase.Analytics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Modules.Firebase.Analytics
{
    internal class FirebaseAnalyticsImplementation : IFirebaseAnalytics
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        
        
        public void SetUserConsent(bool isConsentAvailable)
        {
            CustomDebug.Log($"[Firebase] Requested set user consent to {isConsentAvailable}");
            // UnityEngine.Debug.LogError("SetAnalyticsCollectionEnabled: true");
            // According our terms of use and privacy policy 
            // user is not allowed to opt out from analytics
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            
            CustomDebug.Log($"[Firebase] Set user consent to {true}");
        }


        public void LogEvent(string eventName, IDictionary<string, object> parameters = null)
        {
            if (parameters != null && parameters.Count > 0)
            {
                FirebaseAnalytics.LogEvent(
                    eventName,
                    parameters.Select(pair =>
                    {
                        Parameter result;
                        switch (pair.Value)
                        {
                            case int intValue:
                                result = new Parameter(pair.Key, intValue);
                                break;
                            case long longValue:
                                result = new Parameter(pair.Key, longValue);
                                break;
                            case float floatValue:
                                result = new Parameter(pair.Key, floatValue);
                                break;
                            case double doubleValue:
                                result = new Parameter(pair.Key, doubleValue);
                                break;
                            default:
                                result = new Parameter(pair.Key, pair.Value == null ? "null" : pair.Value.ToString());
                                break;
                        }
                        
                        return result;
                    }).ToArray());
            }
            else
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
        }


        public void SetUserProperty(string propertyName, string propertyValue)
        {
            FirebaseAnalytics.SetUserProperty(propertyName, propertyValue);
        }


        public void SetScreenName(string screenName, string screenClass)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView, screenName, screenClass);
        }


        public void SetUserId(string userId)
        {
            FirebaseAnalytics.SetUserId(userId);
        }
    }
}
#endif
