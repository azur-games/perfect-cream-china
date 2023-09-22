using System.Collections.Generic;


namespace Modules.Firebase.Analytics
{
    internal interface IFirebaseAnalytics : IFirebaseModule
    {
        void SetUserConsent(bool isConsentAvailable);
        void LogEvent(string eventName, IDictionary<string, object> parameters = null);
        void SetUserProperty(string propertyName, string propertyValue);
        void SetScreenName(string screenName, string screenClass);
        void SetUserId(string userId);
    }
}
