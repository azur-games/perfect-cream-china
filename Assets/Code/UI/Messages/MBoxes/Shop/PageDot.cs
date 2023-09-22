using System;
using UnityEngine;
using UnityEngine.UI;


public enum PageDotState
{
    Inactive = 0,
    Active = 1
}



public class PageDot : MonoBehaviour
{
    #region Fields

    [SerializeField] Image icon;

    [Space]
    [Header("States settings")]
    [SerializeField] Sprite activeStateIcon;
    [SerializeField] Sprite inactiveStateIcon;


    PageDotState currentState = PageDotState.Inactive;

    #endregion



    #region Properties

    public PageDotState State
    {
        get => currentState;
        set
        {
            if (currentState != value)
            {
                SetState(value);
            }
        }
    }

    #endregion



    #region Unity lifecycle

    void Awake()
    {
        SetState(PageDotState.Inactive);
    }

    #endregion



    #region States handling

    void SetState(PageDotState state)
    {
        currentState = state;

        switch(state)
        {
            case PageDotState.Active:
                icon.sprite = activeStateIcon;
                break;

            default:
                icon.sprite = inactiveStateIcon;
                break;
        }
    }

    #endregion
}