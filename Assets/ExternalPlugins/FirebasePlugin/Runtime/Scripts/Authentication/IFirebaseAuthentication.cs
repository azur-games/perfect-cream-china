using System;


namespace Modules.Firebase.Authentication
{
    internal interface IFirebaseAuthentication : IFirebaseModule
    {
        bool IsLoggedIn { get; }
        void SignIn(string token, Action<bool> callback);
    }
}
