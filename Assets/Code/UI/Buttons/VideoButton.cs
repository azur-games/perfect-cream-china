using System;
using Modules.Advertising;
using Modules.General.Abstraction;
using UnityEngine;
using UnityEngine.UI;

public class VideoButton : MonoBehaviour
{
    private const int COUNTDOWN_SECONDS = 7;
    private Action onSuccess = () => { };
    private bool needCountdown = true;
    private bool spinnerAlways = true;
    private AdModule adModule = AdModule.RewardedVideo;
    private float? counterStartTime = null;

    [SerializeField] private Button buttonSelf;
    [SerializeField] private Spinner spinner;
    [SerializeField] private Text counter;
    [SerializeField] private Image videoIcon;
    [SerializeField] private RectTransform content;
    [SerializeField] private Vector2 contentPositionWithoutIcon;
    [SerializeField] private Vector2 contentSizeDeltaWithoutIcon;

    public bool IsIteractable = true;
    public bool IsFreeRewardAvailable = false;


    public Image VideoIcon => videoIcon;

    private string placement;

    private void Awake()
    {
        buttonSelf.onClick.AddListener(OnButtonClick);
    }

    public void Init(AdModule adModule, string placement, Action onSuccess)
    {
        this.adModule = adModule;
        this.placement = placement;
        this.onSuccess = onSuccess;
        buttonSelf.interactable = true;

        // load from configs
        needCountdown = BalanceDataProvider.Instance.WatchAdNeedCountdown;
        // spinnerAlways = BalanceDataProvider.Instance.WatchAdSpinnerAlways;

        // bool iva = IsVideoAvailable;
        // spinner.gameObject.SetActive(false);
        // counter.gameObject.SetActive(false);
        // videoIcon.gameObject.SetActive(false);

        // counterStartTime = null;

        // if (spinnerAlways)
        // {
        //     spinner.gameObject.SetActive(!iva);
        //     videoIcon.gameObject.SetActive(!spinner.gameObject.activeSelf);
        //
        //     if (needCountdown)
        //     {
        //         counter.gameObject.SetActive(!iva);
        //         if (counter.gameObject.activeSelf)
        //             counterStartTime = 0.0f;
        //     }
        // }
    }


    public void Reset()
    {
        Init(adModule, placement, onSuccess);
    }

    public void Reset(string newPlacement)
    {
        Init(adModule, newPlacement, onSuccess);
    }

    private bool IsVideoAvailable
    {
        get
        {
            return (Application.internetReachability != NetworkReachability.NotReachable &&
                    AdvertisingManager.Instance.IsAdModuleByPlacementAvailable(adModule, placement)) ||
                   IsFreeRewardAvailable;
        }
    }

    private bool analyticsSend = false;

    private void OnButtonClick()
    {
        if (!IsIteractable)
            return;

        bool iva = IsVideoAvailable;

        if (IsVideoAvailable)
        {
            buttonSelf.interactable = false;
            onSuccess?.Invoke();
            analyticsSend = false;
            IsIteractable = false;
            return;
        }
    }


    void Update()
    {
        // bool iva = IsVideoAvailable;
        //
        // if (spinner.gameObject.activeSelf && iva)
        //     spinner.gameObject.SetActive(false);
        //
        // videoIcon.gameObject.SetActive(!spinner.gameObject.activeSelf);
        //
        // if (counter.gameObject.activeSelf && iva)
        //     counter.gameObject.SetActive(false);
        //
        // if (counter.gameObject.activeSelf)
        // {
        //     counterStartTime += Time.deltaTime;
        //     int counterNum = COUNTDOWN_SECONDS - (int)(counterStartTime.Value);
        //
        //     if (counterNum >= 0)
        //         counter.text = counterNum.ToString();
        //     else
        //     {
        //         counter.gameObject.SetActive(false);
        //     }
        // }
        //
        // if (IsFreeRewardAvailable)
        // {
        //     content.anchoredPosition = contentPositionWithoutIcon;
        //     content.sizeDelta = contentSizeDeltaWithoutIcon;
        // }
        //
        // buttonSelf.interactable = iva;
        // IsIteractable = iva;
    }
}