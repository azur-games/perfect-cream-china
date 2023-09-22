using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelitiousSmallPiece : MonoBehaviour
{
    private Vector3 fromPoint;
    private Vector3 toPoint;
    private Quaternion fromRotation;
    private Quaternion toRotation;

    public float timer;
    private System.Action<Vector3> onFinish;

    public void Init(Vector3 fromPoint, Vector3 toPoint, float timer, System.Action<Vector3> onFinish)
    {
        this.fromPoint = fromPoint;
        this.toPoint = toPoint;
        this.transform.localPosition = fromPoint;
        this.onFinish = onFinish;

        this.timer = timer;

        fromRotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
        toRotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
    }

    public void SetTargetRotation(Vector3 euler)
    {
        toRotation = Quaternion.Euler(euler);
    }

    void Update()
    {
        if (null == onFinish) return;

        float stepBeforeUpdate = Mathf.Clamp01(timer);

        timer += Time.deltaTime;

        float step = Mathf.Clamp01(timer);
        step *= step;
        this.transform.localPosition = Vector3.Lerp(fromPoint, toPoint, step);
        this.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, step);

        if (step < 1.0f) return;
        if (stepBeforeUpdate < step) return;

        onFinish?.Invoke(toPoint);
        onFinish = null;
    }
}
