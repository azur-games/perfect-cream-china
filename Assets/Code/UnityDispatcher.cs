using Modules.General;
using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityDispatcher : MonoBehaviour
{
    private readonly static ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

    private bool isInactivityTimerRegistered = false;

    public static void Enqueue(Action action)
    {
        _queue.Enqueue(action);
    }
    

    void Update()
    {
        if (_queue.IsEmpty)
            return;

        Action action;
        while (_queue.TryDequeue(out action))
        {
            action?.Invoke();
        }
    }
}
