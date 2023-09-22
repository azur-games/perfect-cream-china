using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlayBackground : MonoBehaviour
{
    [SerializeField] protected Camera ownCamera;
    [SerializeField] protected UnityEngine.UI.Image ownImage;

    protected Action onReached { get; private set; }
    protected bool isTargetReached = false;

    public Color currentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public Color CurrentColor
    {
        get
        {
            return currentColor;
        }

        private set
        {
            currentColor = value;
            OnColorChanged();
        }
    } 

    protected Color targetColor { get; private set; }

    private void Awake()
    {
        OnAwake();

        GameObject.DontDestroyOnLoad(this.gameObject);
        ownImage.color = CurrentColor;
        ownCamera.depth = Env.Instance.UI.Config.OverlayCameraDepth;
    }

    private void Update()
    {
        UpdateSelf(Time.deltaTime);
    }

    protected virtual void OnAwake()
    {
        CurrentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    protected virtual void OnColorChanged()
    {

    }

    public void SetTarget(Color color, bool raycastable, bool immediately, Action onReached)
    {
        this.onReached = onReached;

        targetColor = color;

        ownImage.raycastTarget = raycastable;

        isTargetReached = false;

        if (immediately)
        {
            CurrentColor = targetColor;
        }
    }

    protected virtual void UpdateSelf(float deltaTime)
    {
        if (isTargetReached) return;

        Color deltaColor = targetColor - CurrentColor;
        float currentDistance = Mathf.Max(
            Mathf.Abs(deltaColor.r), 
            Mathf.Abs(deltaColor.g), 
            Mathf.Abs(deltaColor.b), 
            Mathf.Abs(deltaColor.a));

        float availDistance = deltaTime * Env.Instance.UI.Config.OverlayChangingSpeed;

        float interpolationStep = (Mathf.Approximately(currentDistance, 0.0f)) ? 1.0f : (availDistance / currentDistance);

        CurrentColor = Color.Lerp(CurrentColor, targetColor, interpolationStep);

        if (interpolationStep >= 1.0f)
        {
            isTargetReached = true;
            onReached?.Invoke();
        }
    }
}
