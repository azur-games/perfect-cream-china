#if FIREBASE_INSTALLATIONS
using System;
using System.Threading.Tasks;
using Firebase.Installations;


namespace Modules.Firebase.Installations
{
    internal class FirebaseInstallationsImplementation : IFirebaseInstallations
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        
        
        public async void FetchInstanceId(Action<string> callback)
        {
            Task<string> requestTask = FirebaseInstallations.DefaultInstance.GetIdAsync();
            
            try
            {
                while (!requestTask.IsCompleted)
                {
                    await Task.Delay(LLFirebaseSettings.Instance.FirebaseIdFetchRetryDelay);
                }
            }
            catch
            {
                requestTask.Dispose();
            }
            
            if (requestTask.IsCompleted)
            {
                CustomDebug.Log($"Firebase Id fetched: {requestTask.Result}");
                callback?.Invoke(requestTask.Result);
            }
            else
            {
                CustomDebug.LogError($"Failed Firebase Id fetch");
                callback?.Invoke("");
            }
        }
    }
}
#endif
