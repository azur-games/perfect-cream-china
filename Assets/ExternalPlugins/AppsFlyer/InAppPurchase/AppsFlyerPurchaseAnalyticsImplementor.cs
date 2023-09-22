using Modules.General.Abstraction;
using Modules.General.InitializationQueue;


namespace Modules.AppsFlyer
{
    [InitQueueService(-7500, typeof(IPurchaseAnalytics))]
    public class PurchaseAnalyticsImplementor : IPurchaseAnalytics
    {
        #region IPurchaseAnalytics

        public string AnalyticsUserId => AppsFlyerSDK.AppsFlyer.getAppsFlyerId();

        
        public string AnalyticsAppId => LLAppsFlyerSettings.AppID();


        public void LogPurchase(
            string productId,
            string currencyCode,
            string price,
            string transactionId,
            string androidPurchaseDataJson,
            string androidPurchaseSignature,
            string androidPublicKey)
        {
            AppsFlyerAnalyticsServiceImplementor.LogPurchase(
                productId,
                currencyCode,
                price,
                transactionId,
                androidPurchaseDataJson,
                androidPurchaseSignature,
                androidPublicKey);
        }

        #endregion
    }
}
