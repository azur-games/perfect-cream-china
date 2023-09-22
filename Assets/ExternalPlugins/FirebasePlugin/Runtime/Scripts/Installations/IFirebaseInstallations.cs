using System;


namespace Modules.Firebase.Installations
{
    internal interface IFirebaseInstallations : IFirebaseModule
    {
        void FetchInstanceId(Action<string> callback);
    }
}
