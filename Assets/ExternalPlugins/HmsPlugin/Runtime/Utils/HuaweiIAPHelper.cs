using HuaweiMobileServices.IAP;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Modules.HmsPlugin
{
    internal static class HuaweiIAPHelper
    {
        public static PriceType ToHmsPriceType(this ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Consumable: return PriceType.IN_APP_CONSUMABLE;
                case ProductType.NonConsumable: return PriceType.IN_APP_NONCONSUMABLE;
                case ProductType.Subscription: return PriceType.IN_APP_SUBSCRIPTION;
                default: 
                    CustomDebug.LogError($"[HMSIAPHelper - ToHmsPriceType] Undefined UnityEngine.Purchasing.ProductType {productType}");
                    return null;
            }
        }


        public static PurchaseFailureReason GetFailureReason(this PurchaseResultInfo status)
        {
            switch (status.ReturnCode)
            {
                case OrderStatusCode.ORDER_PRODUCT_OWNED: return PurchaseFailureReason.DuplicateTransaction;
                case OrderStatusCode.ORDER_STATE_CANCEL : return PurchaseFailureReason.UserCancelled;
                default:
                    CustomDebug.LogError($"[HMSIAPHelper - ToFailureReason] Undefined PurchaseResultInfo.ReturnCode {status.ReturnCode}");
                    return PurchaseFailureReason.Unknown;
            }
        }
    }
}