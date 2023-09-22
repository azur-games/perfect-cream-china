using UnityEngine;

public class iOSBridgeListener : MonoBehaviour
{
#if UNITY_IOS
    public void iosCallback()
    {
        iOSBridge.UserClicked();
    }
#endif
}
