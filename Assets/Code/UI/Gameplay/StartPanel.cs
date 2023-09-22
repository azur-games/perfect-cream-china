using System;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    //public Button SettingsButton;
    //public Button ShopButton;
    //public Button MissionsButton;

    [SerializeField] private CustomButton _startButton;
    
    //[SerializeField] private Button _noAdsButton;

    //[SerializeField] private Image _shopNotification;


    public event Action StartClicked;


    private void Awake()
    {
        _startButton.PointerDown += () => StartClicked?.Invoke();
        //ShopButton.onClick.AddListener(() => { Env.Instance.UI.Messages.ShowShopBox(true, true, ShopBox.Tabs.Skins, ShopBox.Tabs.Tubes, ShopBox.Tabs.Confiture); });
    }


    private void OnEnable()
    {
        //_shopNotification.gameObject.SetActive(ShopItemNotificationInfo.ShouldHighlightNew);
        ShopItemNotificationInfo.OnHighlightNewChanged += ShopItemNotificationInfo_OnHighlightNewChanged;
    }


    private void OnDisable()
    {
        ShopItemNotificationInfo.OnHighlightNewChanged -= ShopItemNotificationInfo_OnHighlightNewChanged;
    }


    void ShopItemNotificationInfo_OnHighlightNewChanged(bool shouldHighlight)
    {
        //_shopNotification.gameObject.SetActive(shouldHighlight);
    }
}
