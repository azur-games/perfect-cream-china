using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Timer
{
    private float totalSeconds;
    private float currentSeconds;
    private Action onFinish;
    private bool isStarted = true;
    
    public Timer(float time, Action onFinish)
    {
        totalSeconds = time;
        currentSeconds = 0.0f;
        this.onFinish = onFinish;
    }

    public float CurrentProgress => currentSeconds / totalSeconds;

//    public void Start()
//    {
//        isStarted = true;
//    }
//
//    public void Pause()
//    {
//        isStarted = false;
//    }

    public void CustomUpdate(float deltaTime)
    {
        if (isStarted)
        {
            currentSeconds += deltaTime;

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
    }
}
