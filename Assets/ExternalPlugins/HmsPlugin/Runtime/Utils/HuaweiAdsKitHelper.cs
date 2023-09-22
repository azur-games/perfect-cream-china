using System;
using HuaweiMobileServices.Ads;

namespace Modules.HmsPlugin
{
    // https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/android-error-code-0000001130129080
    internal enum AdsKitResultCode 
    {
        INNER = 0,
        INVALID_REQUEST = 1,
        NETWORK_ERROR = 2,
        NO_AD = 3,
        AD_LOADING = 4,
        LOW_API = 5,
        BANNER_AD_EXPIRE = 6,
        BANNER_AD_CANCEL = 7,
        HMS_NOT_SUPPORT_SET_APP = 8
    }


    internal static class HuaweiAdsKitHelper
    {
        public static string ToAdsKitResultCode(this int resultCode)
        {
            if (Enum.IsDefined(typeof(AdsKitResultCode), resultCode))
            {
                return $"{(AdsKitResultCode) resultCode} ({resultCode})";
            }
            return resultCode.ToString();
        }
    }
}