using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GDPR/Settings")]
public class GDPRSettings : ScriptableObject
{
    [SerializeField] private string termsLink;
    [SerializeField] private string privacyLink;
    [SerializeField] private bool setConsentEvenIfNotShown;
    [SerializeField] private bool ignoreCountry;
    [SerializeField] private string[] countryCodesForShowing;
    [SerializeField] private GameObject popupPrefab;

    public bool SetConsentEvenIfNotShown => setConsentEvenIfNotShown;
    public bool IgnoreCountry => ignoreCountry;
    public GameObject PopupPrefab => popupPrefab;
    public string TermsLink => termsLink;
    public string PrivacyLink => privacyLink;

    public bool IsRequiredToShowForCountryCode(string countryCode) => string.IsNullOrEmpty(countryCode) || countryCodesForShowing.Contains(countryCode);
    
}
