using System;
using System.Globalization;
using System.Linq;
using ArabicSupport;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

[CreateAssetMenu(menuName = "GDPR/GDPRLocalization")]
public class UserConsentLocalization : ScriptableObject
{
#if UNITY_IOS
    [DllImport("__Internal")]
    extern static public string _getPreferenceLanguageString();
#endif
    private const string EnglishIso = "en-US";
    
    [Serializable]
    public class UserConsentLanguage
    {
        private const string ArabicIso = "ar";
        private const string ThaiIso = "th";
#if UNITY_ANDROID        
        private const string HebrewIso = "iw-IL";
#elif UNITY_IOS
        private const string HebrewIso = "he";
#endif
        private const string HindiIso = "hi";
        public string languageIso;
        
        [SerializeField] private string GDPRDescription;
        [SerializeField] private string GDPRAcceptButton;
        [SerializeField] private string GDPRHeader;
        [SerializeField] private string GDPRTermsButton;
        [SerializeField] private string GDPRPrivacyButton;

        public bool GoesOverflow()
        {
            return IsArabic() || IsHebrew();
        }
        
        public string Description
        {
            get
            {
#if UNITY_IOS
                if (IsNativeDialog())
                {
                    return GDPRDescription.Trim();
                }
#endif
                if (IsArabic())
                {
                    var arabicReversed = ArabicFixer.Fix(GDPRDescription, true);
                    arabicReversed = arabicReversed.Insert(arabicReversed.IndexOf('A') - 1, "\n");
                    arabicReversed = arabicReversed.Insert(arabicReversed.IndexOf('ط') - 1, "\n");
                    var lines = arabicReversed.Split('\n');
                    arabicReversed = lines[2] + "\n" + lines[1] + "\n" + lines[0];
                    return arabicReversed;
                }

                if (IsHebrew())
                {
                    return "ינא ,וז היצקילפאב שומיש ידי לע" + "\n" + "שומישה יאנתל יתמכסה תא עיבמ" + "\n" + "רשאמו Azur Games לש" + "\n" + ".תויטרפה תוינידמ תא יתארקש";
                }
   
                
                return GDPRDescription.Trim();
            }
        }

        public string Accept
        {
            get
            {
#if UNITY_IOS
                if (IsNativeDialog())
                {
                    return GDPRAcceptButton.Trim();
                }
#endif
                if (IsArabic())
                    return ArabicFixer.Fix(GDPRAcceptButton);
                if (IsHebrew())
                    return TextRtlConverter.SwitchRTL(GDPRAcceptButton);
                
                return GDPRAcceptButton.Trim();
            }
        }

        public string Header
        {
            get
            {
#if UNITY_IOS
                if (IsNativeDialog())
                {
                    return GDPRHeader.Trim();
                }
#endif
                
                if (IsArabic())
                    return ArabicFixer.Fix(GDPRHeader);
                if (IsHebrew())
                    return TextRtlConverter.SwitchRTL(GDPRHeader);
                
                return GDPRHeader.Trim();
            }
        }

        public string Terms
        {
            get
            {
#if UNITY_IOS
                if (IsNativeDialog())
                {
                    return GDPRTermsButton.Trim();
                }
#endif
                if (IsArabic())
                    return ArabicFixer.Fix(GDPRTermsButton);
                if (IsHebrew())
                    return TextRtlConverter.SwitchRTL(GDPRTermsButton);
                
                return GDPRTermsButton.Trim();
            }
        }

        public string Privacy
        {
            get
            {
#if UNITY_IOS
                if (IsNativeDialog())
                {
                    return GDPRPrivacyButton.Trim();
                }
#endif
                if (IsArabic())
                    return ArabicFixer.Fix(GDPRPrivacyButton);
                if (IsHebrew())
                    return TextRtlConverter.SwitchRTL(GDPRPrivacyButton);
                
                return GDPRPrivacyButton.Trim();
            }
        }

        public bool FallbackIso(string iso)
        {
            return iso.Split('-')[0] == languageIso.Split('-')[0];
        }

        public bool IsArabic()
        {
            return languageIso == ArabicIso;
        }
        
        public bool IsHindi()
        {
            return languageIso == HindiIso;
        }
        
        public bool IsHebrew()
        {
            return languageIso == HebrewIso;
        }

        public bool IsThai()
        {
            return languageIso == ThaiIso;
        }
    }
    
    public UserConsentLanguage[] localizations;
    
    private string currentLanguage;
    private int currentLanguageIndex;
    
    private void OnEnable()
    {
        currentLanguage = GetLanguageIso();
        try
        {
            currentLanguageIndex = Array.IndexOf(localizations, GetLocalization());
        }
        catch (InvalidOperationException)
        {
            Debug.LogError($"[Azur User Consent Manager] Can't find language with iso {currentLanguage}!");
            currentLanguage = EnglishIso;
            currentLanguageIndex = Array.IndexOf(localizations, GetLocalization());
        }
    }

#if UNITY_IOS
    public static bool IsNativeDialog()
    {
        #if !UNITY_EDITOR
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

        return needAtt;
        #endif
        return false;
    }
#endif
    
    public void SetLanguage(string languageIso) => currentLanguage = languageIso.ToLowerInvariant();
    
#if UNITY_EDITOR
    public void NextLanguage()
    {
        currentLanguageIndex++;
        
        if (currentLanguageIndex == localizations.Length)
        {
            currentLanguageIndex = 0;
        }

        currentLanguage = localizations[currentLanguageIndex].languageIso.ToLowerInvariant();
        Debug.Log(currentLanguage);
    }
#endif
    public bool IsOverflowRequired() => GetLocalization().GoesOverflow();

    public bool GetIsHindi() => GetLocalization().IsHindi();
    public bool GetIsThai() => GetLocalization().IsThai();
    public string GetGDPRHeaderLocalized() => GetLocalization().Header;

    public string GetGDPRDescriptionLocalized() => GetLocalization().Description;

    public string GetGDPRTermsButtonTextLocalized(bool forceNotNative)
    {
#if UNITY_IOS
        if (!forceNotNative && IsNativeDialog())
        {
            return GetLocalization().Terms;
        }
        var resultString = GetLocalization().Terms;
        if (resultString.Length > 17)
        {
            var lastSpaceIndex = resultString.LastIndexOf(' ');
            
            if (lastSpaceIndex < 0) return resultString;
            
            resultString = resultString.Remove(lastSpaceIndex, 1);
            resultString = resultString.Insert(lastSpaceIndex, "\n"); //it is necessary for proper look of window
        }

        return resultString.Trim();
#else
        var resultString = GetLocalization().Terms;
        if (resultString.Length > 17)
        {
            var lastSpaceIndex = resultString.LastIndexOf(' ');
            
            if (lastSpaceIndex < 0) return resultString;
            
            resultString = resultString.Remove(lastSpaceIndex, 1);
            resultString = resultString.Insert(lastSpaceIndex, "\n"); //it is necessary for proper look of window
        }

        return resultString.Trim();
#endif
    }

    public string GetGDPRPrivacyButtonTextLocalized(bool forceNotNative)
    {
#if UNITY_IOS
        if (!forceNotNative && IsNativeDialog())
        {
            return GetLocalization().Privacy;    
        } 
        var resultString = GetLocalization().Privacy;
        if (resultString.Length > 17)
        {
            var lastSpaceIndex = resultString.LastIndexOf(' ');
            
            if (lastSpaceIndex < 0) return resultString;
            
            resultString = resultString.Remove(lastSpaceIndex, 1);
            resultString = resultString.Insert(lastSpaceIndex, "\n");//it is necessary for proper look of window
        }

        return resultString.Trim();
#else
        var resultString = GetLocalization().Privacy;
        if (resultString.Length > 17)
        {
            var lastSpaceIndex = resultString.LastIndexOf(' ');
            
            if (lastSpaceIndex < 0) return resultString;
            
            resultString = resultString.Remove(lastSpaceIndex, 1);
            resultString = resultString.Insert(lastSpaceIndex, "\n");//it is necessary for proper look of window
        }

        return resultString.Trim();
#endif
    }

    public string GetAcceptText() => GetLocalization().Accept.Trim();
    
    private UserConsentLanguage GetLocalization()
    {
#if UNITY_IOS
        if (currentLanguage.Contains("hi"))
        {
            bool needAtt = true;
#if !UNITY_EDITOR
            Version ver = Version.Parse(Device.systemVersion);
            if (ver.Major == 14)
            {
                needAtt = ver.Minor >= 5;
            }
            else
            {
                needAtt = ver.Major >= 15;
            }
#endif

            if (needAtt)
            {
                return localizations.LastOrDefault(l => l.FallbackIso(currentLanguage));
            }
        }
#endif
       
        return localizations.FirstOrDefault(l => l.languageIso.ToLowerInvariant() == currentLanguage) 
               ?? localizations.First(l => l.FallbackIso(currentLanguage));
    }

    private string GetLanguageIso()
    {
        string languageIso = "";
#if UNITY_EDITOR
        languageIso = CultureInfo.CurrentCulture.ToString();
#elif UNITY_ANDROID
        if (AndroidApiVersion() >= 24)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
       
            if (activity == null)
            {
                Debug.LogError("[Azur User Consent Manager] Android current activity is null");
            }
        
            try {  
                AndroidJavaObject locale = activity
                    .Call<AndroidJavaObject>("getResources")
                    .Call<AndroidJavaObject>("getConfiguration")
                    .Call<AndroidJavaObject>("getLocales")
                    .Call<AndroidJavaObject>("get", 0);
                if (locale != null)
                {
                    languageIso = locale.Call<string>("getLanguage") + "-" + locale.Call<string>("getCountry");
                    Debug.Log("[Azur User Consent Manager] Android lang: " + languageIso);
                }
                else
                {
                    Debug.Log("[Azur User Consent Manager] locale null");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Azur User Consent Manager] Error! \n {e}");
            }
        }
        else
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
            {
                if (cls != null)
                {
                    using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
                    {
                        if (locale != null)
                        {
                            languageIso = locale.Call<string>("getLanguage") + "-" + locale.Call<string>("getCountry");
                            Debug.Log("[Azur User Consent Manager] Android lang: " + languageIso);
                        }
                        else
                        {
                            Debug.Log("[Azur User Consent Manager] locale null");
                        }
                    }
                }
                else
                {
                    Debug.LogError("[Azur User Consent Manager] Android native class is null");
                }
            }
        }
       
        
#elif UNITY_IOS
        languageIso = _getPreferenceLanguageString();
#endif
        var parts = languageIso.Split('-');
        if (parts.Length > 2)
        {
            languageIso = parts[0] + "-" + parts[1];
        }
        Debug.Log($"[Azur User Consent Manager] Lang ISO {languageIso.ToLowerInvariant()}");

        return languageIso.ToLowerInvariant();
    }

#if UNITY_ANDROID
    private int AndroidApiVersion()
    {
        var versionName = SystemInfo.operatingSystem;
        var words = versionName.Split(' ');
        var apiWord = words.First(w => w.ToLowerInvariant().StartsWith("api-"));
        int api = int.Parse(apiWord.Split('-')[1]);
        Debug.Log($"[Azur User Consent Manager] Android system ver: {api}");
        return api;
    }
#endif
    
    public string GetCurrentLanguage()
    {
        return GetLanguageIso();
    }
}
