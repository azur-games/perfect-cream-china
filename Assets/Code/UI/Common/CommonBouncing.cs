using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonBouncing : MonoBehaviour
{
    public float WidthAmplitude = 0.0f;
    public float WidthAmplitudeOffset = 1.0f;
    public float WidthFrequency = 0.0f;
    public float WidthTimeOffset = 0.0f;

    public float HeightAmplitude = 0.0f;
    public float HeightAmplitudeOffset = 1.0f;
    public float HeightFrequency = 0.0f;
    public float HeightTimeOffset = 0.0f;

    private Vector3 initialScale = Vector3.one;

    void Awake()
    {
        initialScale = this.transform.localScale;
    }

    void Update()
    {
        float widthScale = Mathf.Cos(Time.realtimeSinceStartup * WidthFrequency + WidthTimeOffset) * WidthAmplitude + WidthAmplitudeOffset;
        float heightScale = Mathf.Cos(Time.realtimeSinceStartup * HeightFrequency + HeightTimeOffset) * HeightAmplitude + HeightAmplitudeOffset;

        Vector3 newScale = new Vector3(
            initialScale.x * widthScale,
            initialScale.y * heightScale,
            initialScale.z);

        this.transform.localScale = newScale;
    }
}
