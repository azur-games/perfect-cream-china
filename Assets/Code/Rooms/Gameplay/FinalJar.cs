using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalJar : MonoBehaviour
{
    public AnimationCurve StressCurve;

    private const float ShowingSpringPower = 0.25f;
    private const float HidingSpringPower = 0.025f;

    private float currentHeight = 5.0f;
    private float targetHeight = 0.0f;
    private float endCurveTime = 0.0f;
    private float animationTime = -1.0f;
    private float movingSpringPower = ShowingSpringPower;
    private bool worked = true;

    private void Start()
    {
        this.transform.localPosition = Vector3.up * currentHeight;
        endCurveTime = StressCurve.keys[StressCurve.keys.Length - 1].time;
    }

    private void Update()
    {
        currentHeight = currentHeight * (1.0f - movingSpringPower) + targetHeight * movingSpringPower;

        if (animationTime >= 0.0f)
        {
            animationTime += Time.deltaTime;
            currentHeight = -StressCurve.Evaluate(animationTime);

            if (animationTime >= endCurveTime)
            {
                animationTime = -1.0f;
                onAnimationFinished?.Invoke();
            }
        }

        this.transform.localPosition = Vector3.up * currentHeight;
    }

    public void GetOut()
    {
        targetHeight = 5.0f;
        animationTime = -1.0f;
        movingSpringPower = HidingSpringPower;
        worked = false;
    }

    private System.Action onAnimationFinished;
    public void Press(System.Action onFinish)
    {
        if (!worked) return;
        animationTime = 0.0f;
        onAnimationFinished = onFinish;
    }
}
