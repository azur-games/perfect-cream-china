using UnityEngine;


namespace MoreMountains.NiceVibrations
{
    /// <summary>
    /// This class allows you to trigger permanent vibrations on both iOS and Android
    /// </summary>
    public static class PermanentVibrationManager
    {
        private const int AndroidVibrationPlayTime = 60 * 60 * 1000;
     
        public static float AndroidPlayDelay = 0.2f;
        
        private static HapticTypes delayedHapticType;
        private static bool shouldPlay = false;
        private static bool isDelayedVariant = false;
        private static int hapticPeriodCounter = 0;
        private static int hapticPeriod = 3;
        private static int androidAmplitude;
        private static float latestPlayTime;
        
        
        /// <summary>
        /// Update method, which should be called from your project Update, if you need permanent notifications
        /// </summary>
        public static void CustomUpdate()
        {
            if (shouldPlay)
            {
                if (isDelayedVariant)
                {
                    hapticPeriodCounter++;
                    
                    if (hapticPeriodCounter < hapticPeriod)
                    {
                        return;
                    }
                    
                    hapticPeriodCounter = 0;
                    MMVibrationManager.Haptic(delayedHapticType);
                }
                else
                {
                    #if UNITY_ANDROID
                        float nowTime = Time.realtimeSinceStartup;
                        
                        if (nowTime - latestPlayTime >= AndroidPlayDelay)
                        {
                            MMVibrationManager.AndroidVibrate(AndroidVibrationPlayTime, androidAmplitude);
                            latestPlayTime = Time.realtimeSinceStartup;
                        }
                    #elif UNITY_IOS
                    MMVibrationManager.Haptic(delayedHapticType);
                    #endif
                }
            }
        }
        
        
        /// <summary>
        /// Start playing of permanent vibration with delays
        /// </summary>
        /// <param name="hapticType">Type of vibration</param>
        /// <param name="skipUpdates">Ticks between vibrations</param>
        public static void EnableDelayedPermanentVibration(HapticTypes hapticType, int skipUpdates = 3)
        {
            isDelayedVariant = true;
            hapticPeriod = skipUpdates;
            delayedHapticType = hapticType;
            shouldPlay = true;
        }
        
        
        /// <summary>
        /// Start playing of permanent vibration
        /// </summary>
        /// <param name="amplitude">MMVibrationManager amplitude parameter</param>
        public static void EnablePermanentVibration(int amplitude)
        {
            isDelayedVariant = false;
            androidAmplitude = amplitude;
            shouldPlay = true;
        }
        
        
        /// <summary>
        /// Disable all active notifications
        /// </summary>
        public static void DisablePermanentVibration()
        {
            shouldPlay = false;
            #if UNITY_ANDROID
                MMVibrationManager.AndroidCancelVibrations();
            #endif
        }
    }
}
