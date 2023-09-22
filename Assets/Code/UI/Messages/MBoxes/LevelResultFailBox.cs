using Modules.Advertising;
using Modules.General.Abstraction;
using System;
using UnityEngine;
using UnityEngine.UI;



public class LevelResultFailBox : UIMessageBox
{
    #region Fields

    [SerializeField] Button okButton;

    Action onClose = null;

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        okButton.onClick.AddListener(OkButton_OnClick);
    }


    void OnDisable()
    {
        okButton.onClick.RemoveListener(OkButton_OnClick);
    }

    #endregion



    #region Initialization

    public void Init(Action onClose = null)
    {
        AdvertisingManager.Instance.TryShowAdByModule(AdModule.Interstitial, AdPlacementType.BeforeResult);

        this.onClose = onClose;
    }

    #endregion



    #region Events handling

    void OkButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        AdvertisingManager.Instance.TryShowAdByModule(AdModule.Interstitial, AdPlacementType.AfterResult,
            (result) =>
            {
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
                {
                    Close();
                    onClose?.Invoke();
                    overlay.Close();
                });
            });

        Env.Instance.SendResultWindow("win", "close", 0, 0);
    }

    #endregion
}