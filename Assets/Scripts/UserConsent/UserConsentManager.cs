using System;
using System.Threading.Tasks;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class UserConsentManager
{
    public static UserConsentManager Instance => instance ??= new UserConsentManager();
    
#if UNITY_IOS
    public bool HasUserConsent => _hasUserConsent && ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED;
#else
    public bool HasUserConsent => _hasUserConsent;
#endif
    
    private static UserConsentManager instance;

    private bool _hasUserConsent;
    private GDPRSettings _gdprSettings;
    private const string PlayerPrefsConsentKey = "cons";
    private const string GDPRSettingsFileName = "GDPRSettings";

    private UserConsentManager()
    {
        _gdprSettings = Resources.Load<GDPRSettings>(GDPRSettingsFileName);
    }

    /// <summary>
    /// Use only in case if you want just to show screen again without any changes of user consent.
    /// </summary>
    public void ShowGdprWithoutConsent()
    {
        ShowGdprScreen(null, true);
    }
    
    public void HandleGdpr(UnityAction onConfirmed)
    {
        HandleGdprInternal(onConfirmed);
    }

    private async void HandleGdprInternal(UnityAction onConfirmed)
    {
        _hasUserConsent = PlayerPrefs.GetInt(PlayerPrefsConsentKey, 0) > 0;
        bool requiredForUser = await ConsentRequiredForUser();
        if (!requiredForUser)
        {
            if (_gdprSettings.SetConsentEvenIfNotShown) SetConsent();
            
            onConfirmed.Invoke();
            return;
        } 
        
        if (!_hasUserConsent)
        {
#if UNITY_IOS
           Version ver = Version.Parse(UnityEngine.iOS.Device.systemVersion);
            if (ver.Major >= 14)
            {
               // iOSBridge.ShowAlert(UserAgreementLocalizationAdapter.Instance.GetGDPRHeaderLocalizedIOS(), 
               //     UserAgreementLocalizationAdapter.Instance.GetGDPRDescriptionLocalizedIOS());
                
                if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
                    ATTrackingStatusBinding.RequestAuthorizationTracking();
                }
            
                while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    await Task.Delay(10);
                }
            } else {
                ShowGdprScreen(onConfirmed, false);
            }
#else

            ShowGdprScreen(onConfirmed, false);
#endif
        }
        else
        {
            onConfirmed.Invoke();
        }
    }

    private void ShowGdprScreen(UnityAction onConfirmed, bool ignoreConsent)
    {
        var popupWindowController = UnityEngine.Object.Instantiate(_gdprSettings.PopupPrefab).GetComponent<GDPRWindowController>();

        popupWindowController.Initialize(() =>
        {
            if (!ignoreConsent) SetConsent();
            
            UnityEngine.Object.Destroy(popupWindowController.gameObject);
        }, 
            UserConsentLocalizationAdapter.Instance.GetGDPRHeaderLocalized(),
            UserConsentLocalizationAdapter.Instance.GetGDPRDescriptionLocalized(),
            UserConsentLocalizationAdapter.Instance.GetGDPRTermsButtonTextLocalized(),
            UserConsentLocalizationAdapter.Instance.GetGDPRPrivacyButtonTextLocalized(),
            _gdprSettings.TermsLink,
            _gdprSettings.PrivacyLink,
            UserConsentLocalizationAdapter.Instance.GetAcceptText()

        );
        
        if (onConfirmed != null) popupWindowController.AddOnClickListener(onConfirmed);
    }

    private void SetConsent()
    {
        PlayerPrefs.SetInt(PlayerPrefsConsentKey, 1);
        PlayerPrefs.Save();
        _hasUserConsent = true;
    }

    private async Task<bool> ConsentRequiredForUser()
    {
        if (_gdprSettings.IgnoreCountry)
        {
            return true;
        }
        
        var countryCode = await GetCountry();
        
        return _gdprSettings.IsRequiredToShowForCountryCode(countryCode);
    }
    
    [Serializable]
    public class IpApiData
    {
        public string country_code;

        public static IpApiData CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<IpApiData>(jsonString);
        }
    }
    
    public async Task<string> GetCountry()
    {
        try
        {
            string uri = "http://ipwho.is/";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.timeout = 1; 
                webRequest.SendWebRequest();

                while (!webRequest.isDone)
                {
                    await Task.Delay(1);
                }
                
                if (!string.IsNullOrEmpty(webRequest.error))
                {
                    return "";
                }
               
                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                IpApiData ipApiData = IpApiData.CreateFromJSON(webRequest.downloadHandler.text);
                return ipApiData.country_code;
            }
        }
        catch
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            throw;
            #endif
            return "";
        }
    }
}
