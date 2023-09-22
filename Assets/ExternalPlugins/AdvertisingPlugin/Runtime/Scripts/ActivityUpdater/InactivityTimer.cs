using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace Modules.Advertising
{
    public class InactivityTimer : SingletonMonoBehaviour<InactivityTimer>
    {
        #region Fields

        public event Action OnUpdateInactivityTimer;

        private float inactivityTime;

        public Func<bool> UpdateTimerCondition;

        #endregion

        public float InterstitialBackgroundTimer { get; set; }
        public bool InterstitialBackgroundTimerAvailable { get; set; }

        #region Properties

        public float InactivityTime => inactivityTime;

        #endregion


        #region Unity Lifecycle

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                ResetInactivityTimer();
            }
            else
            {
                if (UpdateTimerCondition == null || UpdateTimerCondition.Invoke())
                {
                    inactivityTime += Time.deltaTime;
                    OnUpdateInactivityTimer?.Invoke();
                }
            }

            if (InterstitialBackgroundTimerAvailable)
            {
                InterstitialBackgroundTimer += Time.deltaTime;

                if (InterstitialBackgroundTimer >= 10f)
                {
                    InterstitialBackgroundTimer = default;
                    InterstitialBackgroundTimerAvailable = false;
                }
            }
        }

        #endregion


        #region Public Methods

        public void ResetInactivityTimer()
        {
            inactivityTime = 0;
        }

        #endregion
    }
}