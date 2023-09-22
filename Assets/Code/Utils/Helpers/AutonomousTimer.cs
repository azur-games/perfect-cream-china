using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutonomousTimer : MonoBehaviour
{
    public static void Create(float time, Action onFinish, string name = null)
    {
        if (string.IsNullOrEmpty(name)) name = "timer";

        GameObject go = new GameObject(name);
        AutonomousTimer timer = go.AddComponent<AutonomousTimer>();
        timer.totalSeconds = time;
        timer.currentSeconds = 0.0f;
        timer.onFinish = onFinish;
    }

    private float totalSeconds;
    private float currentSeconds;
    private Action onFinish;
    private bool isStarted = true;    

    public float CurrentProgress => currentSeconds / totalSeconds;
    
    private void Update()
    {
        if (isStarted)
        {
            currentSeconds += Time.deltaTime;

            if (currentSeconds >= totalSeconds)
            {
                FinishTimer();
            }
        }
    }

    void FinishTimer()
    {
        isStarted = false;
        onFinish?.Invoke();
        GameObject.Destroy(this.gameObject);
    }
}
