using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAnimationAuto : MonoBehaviour
{
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private float amplitude = 0.0f;
    [SerializeField] private float timeOffset = 0.0f;

    private Vector3 defPosition;

    private void Start()
    {
        defPosition = this.transform.localPosition;
    }

    void Update()
    {
        this.transform.localPosition = defPosition + Vector3.up * Mathf.Sin(Time.realtimeSinceStartup * speed + timeOffset) * amplitude;
    }
}
