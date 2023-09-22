using System;
using System.Threading.Tasks;


namespace Modules.Firebase.Installations
{
    internal class FirebaseInstallationsDummy : IFirebaseInstallations
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        public void FetchInstanceId(Action<string> callback)  => callback?.Invoke(string.Empty);
    }
}
