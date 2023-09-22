using System;
using System.Threading.Tasks;


namespace Modules.Firebase.Database
{
    internal class FirebaseDatabaseDummy : IFirebaseDatabase
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        
        public void SetUserData(string data, Action<bool> callback) => callback?.Invoke(false);
        public void GetUserData(Action<string> callback) => callback?.Invoke(string.Empty);
    }
}
