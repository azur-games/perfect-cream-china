using System.Runtime.InteropServices;
using UnityEngine;

public class iOSBridge : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _ShowAlert(string title, string message);

    [DllImport("__Internal")]
    private static extern void _ShowAlertConfirmation(string title, string message, string callback);

    [DllImport("__Internal")]
    private static extern void _ShareMessage(string message, string url);
    
    public static void ShowAlert(string title, string message)
    {
        _ShowAlert(title, message);
    }

    public static void ShowAlertConfirmation(string title, string message, string callBack)
    {
        _ShowAlertConfirmation(title, message, callBack);
    }

    public static void ShareMessage(string message, string url = "")
    {
        _ShareMessage(message, url);
    }
#endif
}