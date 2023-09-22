using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class UserConsentManager
{
    private static readonly Version ManagerVersion = new Version(2, 5, 0);
    private static readonly bool release = true;

    public static UserConsentManager Instance => instance ??= new UserConsentManager();
    
#if UNITY_IOS
    public bool HasUserConsent => _hasUserConsent && attConsent == ATTManagerAuthorizationStatus.Authorised;
    public ATTManagerAuthorizationStatus AttConsent => attConsent;
    private ATTManagerAuthorizationStatus attConsent;
#else
    public bool HasUserConsent => _hasUserConsent;
#endif
    
    private static UserConsentManager instance;

    private bool _hasUserConsent;
    private UserConsentSettings _userConsentSettings;
    private UserConsentLocalization _userConsentLocalization;
    private const string PlayerPrefsConsentKey = "cons";
    private const string GDPRSettingsFileName = "UserConsentSettings";
    private const string GDPRLocalizationFileName = "UserConsentLocalization";

    private UserConsentManager()
    {
        _userConsentSettings = Resources.Load<UserConsentSettings>(GDPRSettingsFileName);
        _userConsentLocalization = Resources.Load<UserConsentLocalization>(GDPRLocalizationFileName);

        if (!release)
        {
            Debug.Log($"[Azur User Consent Manager] {ManagerVersion.Major}.{ManagerVersion.Minor}rc{ManagerVersion.Build} version initialized!");

        }
        else
        {
            Debug.Log($"[Azur User Consent Manager] {ManagerVersion.Major}.{ManagerVersion.Minor} version initialized!");

        }
    }

    /// <summary>
    /// Use only in case if you want just to show screen again without any changes of user consent.
    /// </summary>
    public void ShowGdprWithoutConsent()
    {
        ShowGdprScreen(null, true);
    }
    
    /// <summary>
    /// Use only in case if you want just to show screen again without any changes of user consent.
    /// </summary>
    /// <param name="languageIso">Language iso code like ru-Ru, es-US</param>
    public void ShowGdprWithoutConsentWithLanguage(string languageIso)
    {
        _userConsentLocalization.SetLanguage(languageIso);
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
            if (_userConsentSettings.SetConsentEvenIfNotShown) SetConsent();

            onConfirmed?.Invoke();
            return;
        }

#if UNITY_IOS && !UNITY_EDITOR
        Version ver = Version.Parse(Device.systemVersion);
        
        bool needAtt;
        if (ver.Major == 14)
        {
            needAtt = ver.Minor >= 5;
        }
        else
        {
            needAtt = ver.Major >= 15;
        }
     
        if (needAtt)
        {
            if (!_hasUserConsent)
            {
                if (!_userConsentSettings.IgnoreIosGdpr)
                {
                    var listener = new GameObject();
                    listener.name = "iOSBridgeListener";
                    listener.AddComponent<iOSBridgeListener>();
                    iOSBridge.ShowGDPR(_userConsentLocalization.GetGDPRDescriptionLocalized(),
                        _userConsentLocalization.GetAcceptText(),
                        _userConsentLocalization.GetGDPRTermsButtonTextLocalized(false),
                        _userConsentLocalization.GetGDPRPrivacyButtonTextLocalized(false),
                        _userConsentSettings.TermsLink,
                        _userConsentSettings.PrivacyLink
                    );

                    while (!iOSBridge.Consent)
                    {
                        await Task.Delay(5);
                    }

                    SetConsent();
                    Object.Destroy(listener);
                }
            }

            if (!_userConsentSettings.IgnoreIosATT)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    ATTDialog.CallATTrackingDialog(s => attConsent = s);
                    while (attConsent == ATTManagerAuthorizationStatus.NotDetermined)
                    {
                        await Task.Delay(5);
                    }
                }
                else
                {
                    Debug.LogWarning($"ATT dialog is ignored on {Application.platform}");
                }
            }
            
            await Task.Delay(2000);
            onConfirmed?.Invoke();
        }
        else
        {
            if (_userConsentSettings.IgnoreIosGdpr) return;
            
            attConsent = ATTManagerAuthorizationStatus.Authorised;
            if (!_hasUserConsent)
            {
                ShowGdprScreen(onConfirmed, false);
            }
            else
            {
                onConfirmed.Invoke();
            }
       
        }
#else
       if (!_hasUserConsent)
        {
            ShowGdprScreen(onConfirmed, false);
        }
        else
        {
            onConfirmed.Invoke();
        }
#endif
    }

    private void ShowGdprScreen(UnityAction onConfirmed, bool ignoreConsent)
    {
        var popupWindowController = Object.Instantiate(_userConsentSettings.PopupPrefab).GetComponent<GDPRWindowController>();


        popupWindowController.Initialize(() =>
            {
                if (!ignoreConsent) SetConsent();

                Object.Destroy(popupWindowController.gameObject);
            },
            _userConsentLocalization.GetGDPRHeaderLocalized(),
            _userConsentLocalization.GetGDPRDescriptionLocalized(),
            _userConsentLocalization.GetGDPRTermsButtonTextLocalized(ignoreConsent),
            _userConsentLocalization.GetGDPRPrivacyButtonTextLocalized(ignoreConsent),
            _userConsentSettings.TermsLink,
            _userConsentSettings.PrivacyLink,
            _userConsentLocalization.GetAcceptText(),
            _userConsentLocalization.GetIsHindi(),
            _userConsentLocalization.IsOverflowRequired(),
            _userConsentLocalization.GetIsThai()
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
        if (_userConsentSettings.IgnoreCountry)
        {
            return true;
        }
        
        var countryCode = await GetCountry();
        
        return _userConsentSettings.IsRequiredToShowForCountryCode(countryCode);
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
