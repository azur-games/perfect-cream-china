using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GDPR/Settings")]
public class UserConsentSettings : ScriptableObject
{
    [Tooltip("In case of used old Applovin, which handles GDPR for ios itself and cannot be disabled.")][SerializeField] private bool ignoreIosGDPR;
    [Tooltip("In case of used old Applovin, which handles ATT for ios itself and cannot be disabled.")][SerializeField] private bool ignoreIosATT;
    [Tooltip("Ignore UserConsentLocalizationAdapter for android.")][SerializeField] private bool useCustomLocalizationForAndroidGDPR;
    [SerializeField] private string termsLink;
    [SerializeField] private string privacyLink;
    [Tooltip("For example if consent windows should not be shown for country. If this is TRUE, then consent will be also true")][SerializeField] private bool setConsentEvenIfNotShown;
    [Tooltip("Show consent for all countries.")][SerializeField] private bool ignoreCountry;
    [SerializeField] private string[] countryCodesForShowing;
    [SerializeField] private GameObject popupPrefab;
    
    public bool SetConsentEvenIfNotShown => setConsentEvenIfNotShown;
    public bool IgnoreCountry => ignoreCountry;
    public GameObject PopupPrefab => popupPrefab;

    public string TermsLink => termsLink;
    public string PrivacyLink => privacyLink;
    public bool IgnoreIosGdpr => ignoreIosGDPR;
    public bool IgnoreIosATT => ignoreIosATT;

    public bool IsRequiredToShowForCountryCode(string countryCode) => string.IsNullOrEmpty(countryCode) || countryCodesForShowing.Contains(countryCode);
    
}
