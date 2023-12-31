﻿using System;
using UnityEngine;


public class CollisionHandler : MonoBehaviour
{
    public event Action<Collision> OnCollisionEnterEvent;
    public event Action<Collision> OnCollisionExitEvent;
    
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerExitEvent;
    public event Action<Collider> OnTriggerStayEvent;
    
    
    
    private void OnCollisionEnter(Collision other)
    {
        OnCollisionEnterEvent?.Invoke(other);
    }

    private void OnCollisionExit(Collision other)
    {
        OnCollisionExitEvent?.Invoke(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
    }
}
