#if FIREBASE_DATABASE
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;
#if FIREBASE_AUTHENTICATION
using Firebase.Auth;
#endif


namespace Modules.Firebase.Database
{
    internal class FirebaseDatabaseImplementation : IFirebaseDatabase
    {
        public int InitializationPriority => 0;
        public Task Initialize(LLFirebaseSettings settings = null) => Task.CompletedTask;
        
        
        public async void SetUserData(string data, Action<bool> callback)
        {
            // We only support only data saving after authentication
            #if FIREBASE_AUTHENTICATION
                DatabaseReference database = FirebaseDatabase.DefaultInstance.RootReference;
                FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
                if (database != null && user != null)
                {
                    await database.Child(user.UserId).SetValueAsync(data).ContinueWithOnMainThread(task =>
                    {
                        bool result;

                        if (task.IsCanceled) 
                        {
                            Debug.LogError("Firebase SetUserData was canceled.");
                            result = false;
                        }
                        else if (task.IsFaulted) 
                        {
                            Debug.LogError($"Firebase SetUserData has encountered an error: {task.Exception}");
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }
                
                        callback?.Invoke(result);
                    });
                }
                else
                {
                    callback(false);
                }
            #else
                callback?.Invoke(false);
            #endif
            
        }


        public async void GetUserData(Action<string> callback)
        {
            // We only support only data saving after authentication
            #if FIREBASE_AUTHENTICATION
                FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
                if (user != null)
                {
                    await FirebaseDatabase.DefaultInstance.GetReference(user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        string result = string.Empty;
                        
                        if (task.IsFaulted)
                        {
                            Debug.LogError($"Firebase GetUserData has encountered an error: {task.Exception}");
                        }
                        else
                        {
                            DataSnapshot snapshot = task.Result;
                            if (snapshot != null &&
                                snapshot.Value != null &&
                                snapshot.Value is string snapshotString)
                            {
                                result = snapshotString;
                            }
                        }
                        
                        callback?.Invoke(result);
                    });
                }
                else
                {
                    callback(string.Empty);
                }
            #else
                callback?.Invoke(string.Empty);
            #endif
        }
    }
}
#endif
