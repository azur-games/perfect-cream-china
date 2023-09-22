using UnityEngine;

public class Example : MonoBehaviour
{
    private UserConsentLocalization _userConsentLocalization;
    private const string GDPRLocalizationFileName = "UserConsentLocalization";
    
    void Start()
    {
        PlayerPrefs.SetInt("cons", 0);
        PlayerPrefs.Save();
        _userConsentLocalization = Resources.Load<UserConsentLocalization>(GDPRLocalizationFileName);
         UserConsentManager.Instance.HandleGdpr(() =>
        {
            
        });
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Destroy(FindObjectOfType<GDPRWindowController>().gameObject);
            
            _userConsentLocalization.NextLanguage();
            UserConsentManager.Instance.HandleGdpr(() =>
            {
            
            });
        }
    }
#endif
}
