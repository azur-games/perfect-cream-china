using System;
using System.Threading.Tasks;


namespace Modules.Firebase.Crashlytics
{
    internal class FirebaseCrashlyticsDummy : IFirebaseCrashlytics
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        public void SetUserMetadata(string dataKey, string dataValue) { }
        public void SetUserId(string userId) { }
        public void LogException(Exception exception) { throw new Exception(exception.Message); }
    }
}
