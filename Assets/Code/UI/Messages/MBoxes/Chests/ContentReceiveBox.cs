using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ContentReceiveBox : UIMessageBox
{
    #region Fields

    [SerializeField] private Button _get;
    [SerializeField] private Image _icon;


    ContentItemIconRef iconRef;

    #endregion



    #region Unity lifecycle

    void OnDesroy()
    {
        if (iconRef != null)
        {
            Resources.UnloadAsset(iconRef);
        }
    }

    #endregion



    public void Init(ContentItemInfo itemInfo, Action onGet)
    {
        iconRef = Env.Instance.Content.LoadContentItemIconRef(itemInfo.AssetType, itemInfo.Name);

        _icon.sprite = iconRef.Icon;

        Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

        _get.onClick.AddListener(() => {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            Env.Instance.Inventory.Delivery.ApplyPrize(itemInfo);
            
            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) => 
            {
                overlay.Close();

                onGet?.Invoke();
                Close();
            });
        });
    }



}
