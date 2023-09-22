using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using Object = UnityEngine.Object;
using System;
using UnityEngine;



namespace Modules.Max
{
    public class MaxDebuggerEnabler : MonoBehaviour
    {
        public void ShowMediationDebugger()
        {
            LLMaxManager.ShowMediationDebugger();
        }
    }

    
    [InitQueueService(0)]
    public class MaxDebuggerService : IInitializableService
    {
        public void InitializeService(IServiceContainer container, Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            if (BuildInfo.IsDebugBuild)
            {
                var go = new GameObject("MaxDebuggerEnabler");
                Object.DontDestroyOnLoad(go);
                go.AddComponent<MaxDebuggerEnabler>();
            }
            onCompleteCallback?.Invoke();
        }
    }
}