using System;
using UnityEngine;

public class Score
{
    private const float PROGRESS_OFFSET_SEC = 8.0f;
    private const float tick = 0.1f;
    private float score_per_sec = 3;

    int[] _comboTicks = new int[] { 7, 15, 39, 47, 87, 111, 167, 175, 247, 287 };

    public event Action Reseted;
    public event Action Charged;

    public float Total { get; private set; }

    private float targetTime;
    private float progressOffset = PROGRESS_OFFSET_SEC;

    public Score(float targetTime, float score_per_sec, float speedScale)
    {
        Total = 0.0f;
        this.targetTime = targetTime;
        this.score_per_sec = score_per_sec;
        progressOffset = PROGRESS_OFFSET_SEC / speedScale;
    }

    public float Progress
    {
        get
        {
            return Mathf.Clamp01(Total / targetTime);
        }
    }

    public float ProgressWithOffset
    {
        get
        {
            return Mathf.Clamp01((float)Total / (float)(targetTime + progressOffset));
        }
    }

    public bool IsComplete
    {
        get
        {
            return (Progress >= 1.0f);
        }
    }

    public void Update()
    {
        Total += Time.deltaTime;
        Charged?.Invoke();
    }

    public void ResetCombos()
    {
        /*_successfullTicks = 0;
        _comboIndex = 0;
        Factor = 1;
        Combo = 0;*/

        Reseted?.Invoke();
    }

}
