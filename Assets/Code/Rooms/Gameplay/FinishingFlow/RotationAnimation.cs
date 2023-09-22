using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    public static void AddTo(GameObject go, AnimationCurve animationCurve)
    {
        RotationAnimation animation = go.AddComponent<RotationAnimation>();
        animation.startRotation = go.transform.rotation.eulerAngles;
        animation.animationCurve = animationCurve;
        animation.time = 0.0f;
    }

    private Vector3 startRotation;
    private AnimationCurve animationCurve;
    private float time = 0.0f;

    void Update()
    {
        time += Time.deltaTime;
        float step = animationCurve.Evaluate(time);
        Vector3 rotation = startRotation;
        rotation.y += step * 360.0f;

        this.transform.rotation = Quaternion.Euler(rotation);
    }
}
