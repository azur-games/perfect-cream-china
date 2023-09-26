using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.General.Abstraction.InAppPurchase;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using BoGD;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private GameObject loading;

    private static bool? isVibroEnabled = null;
    public static bool IsVibroEnabled
    {
        get
        {
            if (!isVibroEnabled.HasValue) IsVibroEnabled = CustomPlayerPrefs.GetBool("vibro", true);
            return isVibroEnabled.Value;
        }

        set
        {
            isVibroEnabled = value;
            CustomPlayerPrefs.SetBool("vibro", value, true);
        }
    }


    public Button Close;

    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;

    [SerializeField] private Button _vibro;
    [SerializeField] private Image _vibroIcon;
    [SerializeField] private Button _sound;
    [SerializeField] private Image _soundIcon;

    [SerializeField] private Button _restorePurchases;
    [SerializeField] private Button _noAds;

    [SerializeField] private Button _termsAndPrivacy;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private VerticalLayoutGroup buttonsLayoutGroup;

    [SerializeField] private Button _removeSubscriptionButton;
    [SerializeField] private GameObject _removeSubscriptionWindow;

    private void Awake()
    {
        InitForTermsAndPrivacy();

        Close.onClick.AddListener(() =>
        {
            PlayerPrefs.Save();

            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            if (IsVibroEnabled)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

            this.gameObject.SetActive(false);

            if (null != Env.Instance.Rooms.GameplayRoom)
            {
                GameplayController.IsGameplayActive = true;
                Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicGameplay);
            }

            Env.Instance.SendPopup("settings", "click_button", "close");
        });

        _vibro.onClick.AddListener(() =>
        {
            IsVibroEnabled = !IsVibroEnabled;
            UpdateVibroColor();

            if (IsVibroEnabled)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            Env.Instance.SendSettings(IsVibroEnabled ? "vibro_on" : "vibro_off");
        });

        _sound.onClick.AddListener(() =>
        {
            Env.Instance.Sound.IsSoundEnabled = !Env.Instance.Sound.IsSoundEnabled;
            UpdateSoundColor();

            if (Env.Instance.Rooms.CurrentRoom is MetagameRoom)
            {
                Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);
            }

            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            if (IsVibroEnabled)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
            Env.Instance.SendSettings(Env.Instance.Sound.IsSoundEnabled ? "sound_on" : "sound_off");
        });

        bool inAppsEnabled = BalanceDataProvider.Instance.InAppsEnabled;
        if (Services.AdvertisingManagerSettings.AdvertisingInfo.IsNoAdsActive || !inAppsEnabled)
            _noAds.gameObject.SetActive(false);

        if (null != _restorePurchases)
        {
            // _restorePurchases.gameObject.SetActive(inAppsEnabled);

            // _restorePurchases.onClick.AddListener(() =>
            // {
            //     if (IsVibroEnabled)
            //         MMVibrationManager.Haptic(HapticTypes.LightImpact);
            //
            //     Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);
            //
            //     if (Application.internetReachability == NetworkReachability.NotReachable)
            //     {
            //         Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Message);
            //         return;
            //     }
            //     loading.SetActive(true);
            //     IStoreManager storeManager = Services.GetService<IStoreManager>();
            //     storeManager.RestorePurchases(result =>
            //     {
            //         loading.SetActive(false);
            //         Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Message,
            //             message: ((result.IsSucceeded) ? ("label_purchases_restored".Translate()) : ("label_cannot_connect".Translate()))) ;
            //     });
            //     Env.Instance.SendSettings("restore_purchases");
            // });
        }
        if (_removeSubscriptionButton != null)
        {
            _removeSubscriptionButton.onClick.AddListener(OnRemoveSubscriptionClick);
        }
    }

    private void OnRemoveSubscriptionClick()
    {
        _removeSubscriptionWindow.SetActive(true);

        Env.Instance.SendSettings("click_remove_subscription");
    }

    private void InitForTermsAndPrivacy()
    {
        bool needToShowPrivacyButton = ((null != _termsAndPrivacy));
        // bool needToShowPrivacyButton = ((null != _termsAndPrivacy) && true);

        if (needToShowPrivacyButton)
        {
            // _termsAndPrivacy.gameObject.SetActive(true);
            // _termsAndPrivacy.onClick.AddListener(OnTermsAndPrivacyButtonClick);

            _backgroundImage.rectTransform.sizeDelta = new Vector2(_backgroundImage.rectTransform.sizeDelta.x, 1400.0f);
            _backgroundImage.transform.localPosition = new Vector3(0.0f, -43.76f, 0.0f);
            buttonsLayoutGroup.padding.top = -110;
            buttonsLayoutGroup.spacing = 100.0f;
            buttonsLayoutGroup.transform.localPosition = new Vector3(0.0f, -100.0f, 0.0f);
        }
        else
        {
            // if (null != _termsAndPrivacy) _termsAndPrivacy.gameObject.SetActive(false);
            if (null != _backgroundImage)
            {
                _backgroundImage.rectTransform.sizeDelta = new Vector2(_backgroundImage.rectTransform.sizeDelta.x, 1200.0f);
                _backgroundImage.transform.localPosition = new Vector3(0.0f, -43.76f, 0.0f);
            }
            if (null != buttonsLayoutGroup)
            {
                buttonsLayoutGroup.padding.top = 0;
                buttonsLayoutGroup.spacing = 150.0f;
                buttonsLayoutGroup.transform.localPosition = new Vector3(0.0f, -100.0f, 0.0f);
            }
        }
    }

    private void OnTermsAndPrivacyButtonClick()
    {   
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Message);
            return;
        }
        loading.SetActive(true);
        #if UNITY_EDITOR
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                loading.SetActive(false);
                if (Random.value >= 0.5f)
                {
                    Services.GetService<IPrivacyManager>().GetTermsAndPolicyURI((bool success, string url) =>
                    {
                        loading.SetActive(false);
                        if (success)
                        {
                            Application.OpenURL(url);
                        }
                    });
                }
                else
                {
                    Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Message, message: "Something went wrong");
                }
            }, 2.0f);
#else
            Services.GetService<IPrivacyManager>().GetTermsAndPolicyURI((bool success, string url) =>
            {
                loading.SetActive(false);
                if (success)
                {
                    Application.OpenURL(url);
                }
                else
                {
                    Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Message, message: "Something went wrong");
                }
            });
#endif
        Env.Instance.SendSettings("click_terms");
    }


    public void Show()
    {
        UpdateSoundColor();
        UpdateVibroColor();

        this.gameObject.SetActive(true);
        if (_removeSubscriptionButton != null)
        {
            // _removeSubscriptionButton.gameObject.SetActive(Application.platform == RuntimePlatform.Android && ((SubscriptionManager)SubscriptionManager.Instance).IsSubscriptionActive);
        }

        Env.Instance.SendPopup("settings", "click_button", "show");
    }

    private void UpdateSoundColor()
    {
        _soundIcon.color = Env.Instance.Sound.IsSoundEnabled ? _onColor : _offColor;
    }

    private void UpdateVibroColor()
    {
        _vibroIcon.color = IsVibroEnabled ? _onColor : _offColor;
    }
}
