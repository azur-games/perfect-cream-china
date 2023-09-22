using System;


namespace Modules.Firebase.Database
{
    internal interface IFirebaseDatabase : IFirebaseModule
    {
        void SetUserData(string data, Action<bool> callback);
        void GetUserData(Action<string> callback);
    }
}
