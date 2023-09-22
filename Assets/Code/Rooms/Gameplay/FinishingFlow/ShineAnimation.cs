using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShineAnimation : MonoBehaviour
{
    public static void AddTo(ShineAnimation original, Transform parent)
    {
        ShineAnimation clone = GameObject.Instantiate(original.gameObject).GetComponent<ShineAnimation>();
        clone.transform.parent = parent;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localScale = Vector3.one;
        clone.transform.localRotation = Quaternion.identity;

        clone.Init();
    }


    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private Transform shineSprite;

    private System.DateTime startTime = System.DateTime.Now;

    private float finishingTime = 0.0f;

    private void Init()
    {
        startTime = System.DateTime.Now;
        finishingTime = Mathf.Max(
            rotationCurve.keys[rotationCurve.keys.Length - 1].time,
            scaleCurve.keys[scaleCurve.keys.Length - 1].time);

        UpdateSelf();
    }

    void Update()
    {
        UpdateSelf();
    }

    private void UpdateSelf()
    {
        float timeStep = (float)(System.DateTime.Now - startTime).TotalSeconds;

        float scaleStep = scaleCurve.Evaluate(timeStep);
        shineSprite.transform.localScale = Vector3.one * scaleStep;

        float rotationStep = rotationCurve.Evaluate(timeStep);
        shineSprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotationStep * 360.0f);

        if (timeStep >= finishingTime)
        {
            GameObject.Destroy(this);
        }
    }
}
