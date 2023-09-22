using System.Runtime.InteropServices;
using UnityEngine;

public class iOSBridge : MonoBehaviour
{
#if UNITY_IOS
    public static bool Consent => consent;

    private static bool consent;
    
    [DllImport("__Internal")]
    private static extern void _ShowGDPR(string title, string message, string click, string okButton,  string termsText, string policyText, string termsURL, string policyURL);

    public static void ShowGDPR(string title, string okButton, string termsText, string policyText, string termsURL, string policyURL)
    {
        _ShowGDPR(title, "","iosCallback", okButton, termsText, policyText, termsURL, policyURL);
    }
    
    public static void UserClicked()
    {
        consent = true;
    }

#endif

}