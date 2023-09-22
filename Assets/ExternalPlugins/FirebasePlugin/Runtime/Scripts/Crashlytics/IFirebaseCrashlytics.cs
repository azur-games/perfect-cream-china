using System;


namespace Modules.Firebase.Crashlytics
{
    internal interface IFirebaseCrashlytics : IFirebaseModule
    {
        void SetUserMetadata(string dataKey, string dataValue);
        void SetUserId(string userId);
        void LogException(Exception exception);
    }
}
