using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClocksSpinner : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float speed;
    [SerializeField] private float onStartDelay;

    private float time = 0.0f;
    private float rotations = 0.0f;
    
    void Update()
    {
        onStartDelay -= Time.deltaTime;
        if (onStartDelay > 0.0f) return;

        time += Time.deltaTime * speed;
        
        float period = curve.keys[curve.keys.Length - 1].time;
        if (time > period)
        {
            time -= period;
            rotations = 1.0f - rotations;
        }

        float angle = curve.Evaluate(time);
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, (rotations + angle) * 180.0f);
    }
}
