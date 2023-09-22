using System;
using System.Threading.Tasks;


namespace Modules.Firebase.Authentication
{
    internal class FirebaseAuthenticationDummy : IFirebaseAuthentication
    {
        public int InitializationPriority => 0;
        public bool IsLoggedIn => false;
        
        
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        public void SignIn(string token, Action<bool> callback) => callback?.Invoke(false);
    }
}
