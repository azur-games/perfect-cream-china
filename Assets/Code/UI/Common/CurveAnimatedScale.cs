using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveAnimatedScale : MonoBehaviour
{
    [SerializeField] private AnimationCurve ScaleXCurve;
    [SerializeField] private AnimationCurve ScaleYCurve;
    [SerializeField] private AnimationCurve ScaleZCurve;

    private float maxLifetime = float.MaxValue;
    private System.DateTime startTime = System.DateTime.Now;

    public void Play()
    {
        this.gameObject.SetActive(true);
        startTime = System.DateTime.Now;

        float xlifetime = (null == ScaleXCurve) ? 0.0f : ScaleXCurve.keys[ScaleXCurve.keys.Length - 1].time;
        float ylifetime = (null == ScaleYCurve) ? 0.0f : ScaleYCurve.keys[ScaleYCurve.keys.Length - 1].time;
        float zlifetime = (null == ScaleZCurve) ? 0.0f : ScaleZCurve.keys[ScaleZCurve.keys.Length - 1].time;
        maxLifetime = Mathf.Max(xlifetime, ylifetime, zlifetime);

        UpdateSelf();
    }

    void Update()
    {
        UpdateSelf();
    }

    private void UpdateSelf()
    {
        float timeSpan = (float)(System.DateTime.Now - startTime).TotalSeconds;
        float scaleX = (null == ScaleXCurve) ? 1.0f : ScaleXCurve.Evaluate(timeSpan);
        float scaleY = (null == ScaleYCurve) ? 1.0f : ScaleYCurve.Evaluate(timeSpan);
        float scaleZ = (null == ScaleZCurve) ? 1.0f : ScaleZCurve.Evaluate(timeSpan);
        this.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        if (timeSpan >= maxLifetime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
