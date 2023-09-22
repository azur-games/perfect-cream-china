using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class ScrollRectHandler : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    #region Fields

    public event Action<bool> OnDragStateChanged;

    bool isDragging;

    #endregion
    
    
    
    #region Properties

    public bool IsDragging
    {
        get => isDragging;

        set
        {
            if (IsDragging != value)
            {
                isDragging = value;
                
                OnDragStateChanged?.Invoke(IsDragging);
            }
        }
    }
    
    
    public float LastTouchDuration { get; set; }

    #endregion



    #region Unity lifecycle

    void Update()
    {
        if (IsDragging)
        {
            LastTouchDuration += Time.deltaTime;
        }
    }

    #endregion



    #region IEndDragHandler

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }

    #endregion
        
    

    #region IBeginDragHandler

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        LastTouchDuration = 0;
    }

    #endregion
}