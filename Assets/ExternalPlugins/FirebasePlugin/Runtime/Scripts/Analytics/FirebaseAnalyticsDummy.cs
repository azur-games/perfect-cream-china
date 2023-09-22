using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.Firebase.Analytics
{
    internal class FirebaseAnalyticsDummy : IFirebaseAnalytics
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        public void SetUserConsent(bool isConsentAvailable) { }
        public void LogEvent(string eventName, IDictionary<string, object> parameters = null) { }
        public void SetUserProperty(string propertyName, string propertyValue) { }
        public void SetScreenName(string screenName, string screenClass) { }
        public void SetUserId(string userId) { }
    }
}
