using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAnimation : MonoBehaviour
{
    public static void AddTo(GameObject go, AnimationCurve animationCurve, Vector3 targetPosition, System.Action onFinish)
    {
        MovingAnimation animation = go.AddComponent<MovingAnimation>();
        animation.startPosition = go.transform.position;
        animation.endPosition = targetPosition;
        animation.animationCurve = animationCurve;
        animation.time = 0.0f;
        animation.finishingTime = animationCurve.keys[animationCurve.keys.Length - 1].time;
        animation.onFinish = onFinish;
    }

    private Vector3 startPosition;
    private Vector3 endPosition;
    private AnimationCurve animationCurve;
    private float time = 0.0f;
    private float finishingTime = 0.0f;
    private System.Action onFinish;

    void Update()
    {
        time += Time.deltaTime;
        float step = animationCurve.Evaluate(time);
        this.transform.position = startPosition * (1.0f - step) + endPosition * step;
        if (time >= finishingTime)
        {
            onFinish?.Invoke();
            GameObject.Destroy(this);
        }
    }
}
