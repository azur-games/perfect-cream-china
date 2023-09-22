
using AppsFlyerConnector;
using AppsFlyerSDK;
using Modules.General.HelperClasses;

public class AppsFlyerMarker : SingletonMonoBehaviour<AppsFlyerMarker>, IAppsFlyerValidateReceipt, IAppsFlyerPurchaseValidation
{
    public void onConversionDataSuccess(string conversionData)
    {
        CustomDebug.Log($"[AppsFlyer] onConversionDataSuccess {conversionData}");
    }

    public void onConversionDataFail(string error)
    {
        CustomDebug.LogError($"[AppsFlyer] onConversionDataFail {error}");
    }

    public void onAppOpenAttribution(string attributionData)
    {
        CustomDebug.Log($"[AppsFlyer] onAppOpenAttribution {attributionData}");
    }

    public void onAppOpenAttributionFailure(string error)
    {
        CustomDebug.LogError($"[AppsFlyer] onAppOpenAttributionFailure {error}");
    }

    public void didFinishValidateReceipt(string result)
    {
        
        CustomDebug.Log($"[AppsFlyer] didFinishValidateReceipt {result}");
    }

    public void didFinishValidateReceiptWithError(string error)
    {
        CustomDebug.LogError($"[AppsFlyer] didFinishValidateReceiptWithError {error}");
    }

    public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
    {
        CustomDebug.Log($"[AppsFlyer] didReceivePurchaseRevenueValidationInfo {validationInfo}");
    }

    public void didReceivePurchaseRevenueError(string error)
    {
        CustomDebug.LogError($"[AppsFlyer] didReceivePurchaseRevenueError {error}");
    }
}