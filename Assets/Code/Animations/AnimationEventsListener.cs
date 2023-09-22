using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsListener : MonoBehaviour
{
    private System.Action onEvent = () => { };

    public void AddListener(System.Action onEvent)
    {
        RemoveListener(onEvent);
        this.onEvent += onEvent;
    }

    public void RemoveListener(System.Action onEvent)
    {
        this.onEvent -= onEvent;
    }
    
    public void AnimationEvent()
    {
        onEvent.Invoke();
        onEvent = () => { };
    }
}
