#if FIREBASE_AUTHENTICATION
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Firebase.Authentication
{
    internal class FirebaseAuthenticationImplementation : IFirebaseAuthentication
    {
        public int InitializationPriority => 0;
        public bool IsLoggedIn => FirebaseAuth.DefaultInstance.CurrentUser != null;


        public void SignIn(string token, Action<bool> callback)
        {
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SignInWithCredentialAsync(FacebookAuthProvider.GetCredential(token)).ContinueWith(task =>
            {
                bool result;

                if (task.IsCanceled) 
                {
                    Debug.LogError("Firebase sign in was canceled.");
                    result = false;
                }
                else if (task.IsFaulted) 
                {
                    Debug.LogError($"Firebase sign in has encountered an error: {task.Exception}");
                    result = false;
                }
                else
                {
                    result = true;
                }

                callback?.Invoke(result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
    }
}
#endif
