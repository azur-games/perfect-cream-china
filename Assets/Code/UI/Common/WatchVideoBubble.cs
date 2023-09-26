using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchVideoBubble : MonoBehaviour
{
    [SerializeField] private RectTransform ownTransform;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private int _price;

    private Action onVideoShowed = () => { };
    private string videoRewardName;

    public bool IsShowing
    {
        get
        {
            return gameObject.activeSelf;
        }
        private set
        {
            gameObject.SetActive(value);
        }
    }

    public void Hide()
    {
        IsShowing = false;
    }

    private void Awake()
    {
        button.onClick.AddListener(OnOwnButtonClick);
        button.interactable = false;

        IsShowing = false;
    }

    public void Show(Vector2 position, Sprite icon, string rewardName, Action onVideoShowed)
    {
        this.transform.localPosition = new Vector3(position.x, position.y, this.transform.localPosition.z);

        this.onVideoShowed = onVideoShowed;
        this.videoRewardName = rewardName;

        image.sprite = icon;

        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.rect.width,
            (float)image.rectTransform.rect.width *
            (float)icon.rect.height /
            (float)icon.rect.width);

        button.interactable = true;

        IsShowing = true;
        _priceText.text = _price.ToString();
    }

    private void OnOwnButtonClick()
    {
        if (Env.Instance.Inventory.Bucks >= _price)
        {
            Env.Instance.Inventory.TrySpendBucks(_price);
            Action onVideoShowed_cached = onVideoShowed;
            onVideoShowed = () => { };
            onVideoShowed_cached?.Invoke();
            
            IsShowing = false;
        }
        // button.interactable = false;
        // IsShowing = false;
        // Env.Instance.Sound.StopMusic();

        // AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, "meta_pers_bubble", 
        //     (resultType) =>
        // {
        //     button.interactable = true;
        //     Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);
        //
        //     switch (resultType)
        //     {
        //         case AdActionResultType.Success:
        //             System.Action onVideoShowed_cached = onVideoShowed;
        //             onVideoShowed = () => { };
        //             onVideoShowed_cached?.Invoke();
        //
        //             break;
        //
        //         default:
        //             Env.Instance.UI.Messages.ShowInfoBox("label_no_video".Translate(), () =>
        //             {
        //                 IsShowing = true;
        //                 button.interactable = true;
        //             });
        //             break;
        //     }
        // });
    }
}
