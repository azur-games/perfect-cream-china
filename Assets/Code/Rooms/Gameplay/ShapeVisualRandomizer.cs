using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeVisualRandomizer:MonoBehaviour
{
    private void Awake()
    {
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(
            UnityEngine.Random.Range(0.0f, 360.0f), 
            UnityEngine.Random.Range(0.0f, 360.0f), 
            UnityEngine.Random.Range(0.0f, 360.0f));
    }

}
