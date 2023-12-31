﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Helpers
{
    public static class ApplicationFlow
    {
        public static bool RestartApplication()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
            intent.Call<AndroidJavaObject>("setFlags", 0x20000000);//Intent.FLAG_ACTIVITY_SINGLE_TOP

            AndroidJavaClass pendingIntent = new AndroidJavaClass("android.app.PendingIntent");
            AndroidJavaObject contentIntent = pendingIntent.CallStatic<AndroidJavaObject>("getActivity", currentActivity, 0, intent, 0x8000000); //PendingIntent.FLAG_UPDATE_CURRENT = 134217728 [0x8000000]
            AndroidJavaObject alarmManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "alarm");
            AndroidJavaClass system = new AndroidJavaClass("java.lang.System");
            long currentTime = system.CallStatic<long>("currentTimeMillis");
            alarmManager.Call("set", 1, currentTime + 1000, contentIntent); // android.app.AlarmManager.RTC = 1 [0x1]

            Debug.LogError("alarm_manager set time " + currentTime + 1000);
            currentActivity.Call("finish");

            AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);

            return true;
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            Debug.LogError("Can not restart iOS application");
            return false;
#endif
            Debug.LogError("Can not restart application in Editor");
            return false;
        }
    }
}
