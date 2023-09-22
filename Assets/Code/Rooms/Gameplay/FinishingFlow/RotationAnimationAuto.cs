using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimationAuto : MonoBehaviour
{
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private Vector3 axis = Vector3.zero;

    void Update()
    {
        this.transform.Rotate(axis, speed);
    }
}
