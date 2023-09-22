using Modules.General;
using Modules.General.HelperClasses;
using System;
using System.Collections;
using UnityEngine;


namespace Modules.HmsPlugin
{
    /// <summary>
    ///  Костыль, который нужен того, чтобы отловить момент возвращения игрока после покупки. На некоторых девайсах
    ///  в таком случае не всегда отрабатывает LLApplicationStateRegister.OnApplicationEnteredBackground / OnApplicationFocus / OnApplicationPause
    /// </summary>
    internal class ApplicationFocusObserver : SingletonMonoBehaviour<ApplicationFocusObserver>
    {
        #region Fields

        private Action callback;

        private int framesWait;

        private bool isObserving;

        #endregion



        #region Methods

        public void StartObserve(Action callback, int framesWait = 10, float longIntervalTime = 1)
        {
            this.callback = callback;
            this.framesWait = framesWait;
            LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;
            StartCoroutine(WaitLongIntervalBetweenFrames(longIntervalTime));
            isObserving = true;

        }

        
        public void StopObserve()
        {
            if (isObserving)
            {
                LLApplicationStateRegister.OnApplicationEnteredBackground -= LLApplicationStateRegister_OnApplicationEnteredBackground;
                StopAllCoroutines();
                isObserving = false;
            }
        }
        
        
        private void LLApplicationStateRegister_OnApplicationEnteredBackground(bool enteredBackground)
        {
            if (!enteredBackground)
            {
                OnFocus();
            }
        }

        
        private void OnFocus()
        {
            IEnumerator WaitFrames(Action callback, int framesCount)
            {
                int count = 0;
                while (count < framesCount)
                {
                    yield return new WaitForEndOfFrame();
                    count++;
                }
                callback?.Invoke();
            }
            
            StopObserve();
            StartCoroutine(WaitFrames(callback, framesWait));
        }

        
        private IEnumerator WaitLongIntervalBetweenFrames(float longIntervalTime)
        {
            float lastFrameTimeAfterStart = Time.realtimeSinceStartup;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                if (realtimeSinceStartup - lastFrameTimeAfterStart >= longIntervalTime)
                {
                    break;
                }
                lastFrameTimeAfterStart = realtimeSinceStartup;
            }
            OnFocus();
        }

        #endregion
    }
}