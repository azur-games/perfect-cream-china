using UnityEngine;

/// <summary>
/// It's not an example!
/// </summary>
public class TestUserConsent : MonoBehaviour
{
    private UserConsentLocalization _userConsentLocalization;
    private const string GDPRLocalizationFileName = "UserConsentLocalization";
    
    void Start()
    {
        _userConsentLocalization = Resources.Load<UserConsentLocalization>(GDPRLocalizationFileName);
         UserConsentManager.Instance.HandleGdpr(() =>
        {
            
        });
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        //for checking languages, editor only please
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
