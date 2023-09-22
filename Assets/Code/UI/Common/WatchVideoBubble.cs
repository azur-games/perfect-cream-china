using Modules.Advertising;
using Modules.Analytics;
using Modules.General.Abstraction;
using UnityEngine;
using BoGD;

public class WatchVideoBubble : MonoBehaviour
{
    [SerializeField] private RectTransform ownTransform;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private Vector3 positionOffset;

    private System.Action onVideoShowed = () => { };
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

    public void Show(Vector2 position, Sprite icon, string rewardName, System.Action onVideoShowed)
    {
        var positionX = position.x / Screen.width * image.rectTransform.sizeDelta.x;
        var positionY = position.y / Screen.height * image.rectTransform.sizeDelta.y;
        transform.localPosition = new Vector3(positionX, positionY, transform.localPosition.z) + positionOffset;

        this.onVideoShowed = onVideoShowed;
        this.videoRewardName = rewardName;

        image.sprite = icon;

        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.rect.width,
            (float)image.rectTransform.rect.width *
            (float)icon.rect.height /
            (float)icon.rect.width);

        button.interactable = true;

        IsShowing = true;
    }

    private void OnOwnButtonClick()
    {
        button.interactable = false;
        IsShowing = false;
        Env.Instance.Sound.StopMusic();

        AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, "meta_pers_bubble", 
            (resultType) =>
        {
            button.interactable = true;
            Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);

            switch (resultType)
            {
                case AdActionResultType.Success:
                    System.Action onVideoShowed_cached = onVideoShowed;
                    onVideoShowed = () => { };
                    onVideoShowed_cached?.Invoke();

                    break;

                default:
                    Env.Instance.UI.Messages.ShowInfoBox("label_no_video".Translate(), () =>
                    {
                        IsShowing = true;
                        button.interactable = true;
                    });
                    break;
            }
        });
    }
}
