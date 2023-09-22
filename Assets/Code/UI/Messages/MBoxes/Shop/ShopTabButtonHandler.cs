using System;
using UnityEngine;
using UnityEngine.UI;


public enum ShopTabButtonState
{
    Inactive = 0,
    Active = 1
}



public class ShopTabButtonHandler : MonoBehaviour
{
    #region Nested types

    [Serializable]
    class ShopTabButtonSettings
    {
        public Sprite background;
        public Sprite icon;
        public Vector2 iconOffset;
        public Vector2 iconSize;
    }

    #endregion



    #region Fields

    [SerializeField] Button button;
    [SerializeField] Image background;
    [SerializeField] Image icon;
    [SerializeField] Image newAvailableIcon;

    [Space]
    [Header("States settings")]
    [SerializeField] ShopTabButtonSettings activeStateSettings;
    [SerializeField] ShopTabButtonSettings inactiveStateSettings;


    ShopTabButtonState currentState = ShopTabButtonState.Inactive;

    #endregion



    #region Properties

    public ShopTabButtonState State
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


    public Button Button => button;

    #endregion



    #region States handling

    void SetState(ShopTabButtonState state)
    {
        currentState = state;

        switch (state)
        {
            case ShopTabButtonState.Active:
                background.sprite = activeStateSettings.background;
                icon.sprite = activeStateSettings.icon;
                break;

            default:
                background.sprite = inactiveStateSettings.background;
                icon.sprite = inactiveStateSettings.icon;
                break;
        }
    }

    #endregion



    #region Content updates

    public void UpdateContent(bool isNewItemAvailable)
    {
        newAvailableIcon.gameObject.SetActive(isNewItemAvailable);
    }

    #endregion
}