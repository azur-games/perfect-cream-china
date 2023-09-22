using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    public event Action PointerDown;
    public override void OnPointerDown(PointerEventData eventData)
    {
        PointerDown?.Invoke();
        base.OnPointerDown(eventData);
    }

    public event Action PointerUp;
    public override void OnPointerUp(PointerEventData eventData)
    {
        PointerUp?.Invoke();
        base.OnPointerDown(eventData);
    }
}
