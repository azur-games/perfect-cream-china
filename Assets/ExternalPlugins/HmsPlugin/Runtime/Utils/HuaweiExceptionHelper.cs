using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using System;
using System.Text.RegularExpressions;


namespace Modules.HmsPlugin
{
    internal enum HMSResultCode : int
    {
        //https://developer.huawei.com/consumer/en/doc/development/HMSCore-References/client-error-code-0000001050746111
        ORDER_STATE_SUCCESS = 0,
        ORDER_STATE_FAILED = -1,
        ORDER_STATE_DEFAULT_CODE = 1,
        
        ORDER_STATE_CANCEL = 60000,
        ORDER_STATE_PARAM_ERROR = 60001,
        ORDER_STATE_IAP_NOT_ACTIVATED = 60002,
        ORDER_STATE_PRODUCT_INVALID = 60003,
        ORDER_STATE_CALLS_FREQUENT = 60004,
        ORDER_STATE_NET_ERROR = 60005,
        ORDER_STATE_PMS_TYPE_NOT_MATCH = 60006,
        ORDER_STATE_PRODUCT_COUNTRY_NOT_SUPPORTED = 60007,
        
        ORDER_HWID_NOT_LOGIN = 60050,
        ORDER_PRODUCT_OWNED = 60051,
        ORDER_PRODUCT_NOT_OWNED = 60052,
        ORDER_PRODUCT_CONSUMED = 60053,
        ORDER_ACCOUNT_AREA_NOT_SUPPORTED = 60054,
        ORDER_HIGH_RISK_OPERATIONS = 60056,
        
        // В одном и том же месте могут быть коды из разных таблиц
        // https://developer.huawei.com/consumer/en/doc/development/HMS-2-References/hmssdk_jointOper_api_reference_errorcode
        CERT_FINGERPRINT_ERROR = 6003,
        GET_SCOPE_ERROR = 907135700,
        ARGUMENTS_INVALID = 907135000
    }
    
    
    internal static class HuaweiExceptionHelper
    {
        /// <summary>
        ///  Почему-то настощий ErrorCode не в exception.ErrorCode, а в WrappedExceptionMessage. Разработчики hms-unity-plugin обещали,
        /// что в следующих версиях (после 2.0.10) сделают более осмысленные логи 
        /// </summary>
        // TODO Проверить работоспособность после обновления hms-unity-plugin с 2.0.10
        public static HMSResultCode GetErrorCode(this HMSException exception)
        {
            IapApiException iapException = exception.AsIapApiException();
            if (iapException?.Status != null && Enum.IsDefined(typeof(HMSResultCode), iapException.Status.StatusCode))
            {
                return (HMSResultCode) iapException.Status.StatusCode;
            }
            if (HMSResultCode.TryParse(exception.WrappedExceptionMessage, out HMSResultCode resultCode))
            {
                return resultCode;
            }
            var match = Regex.Match(exception.WrappedExceptionMessage, "(^[0-9]+).*:");
            if (match.Success && HMSResultCode.TryParse(match.Groups[1].Value, out HMSResultCode resultCodeFromRegex))
            {
                return resultCodeFromRegex;
            }
            return (HMSResultCode) exception.ErrorCode; // Здесь ненастоящий ErrorCode, а просто ORDER_STATE_SUCCESS
        }
        

        public static string GetDescription(this HMSException exception)
        {
            HMSResultCode errorCode = exception.GetErrorCode();
            string description = $"HMSException : {errorCode.ToString()} ({(int) errorCode}). ";
            switch (errorCode)
            {
                case HMSResultCode.ARGUMENTS_INVALID:
                    description += "Verify that related input parameters are correctly set. For example, " +
                                   "check if agconnect-services.json is added to Assets/StreamingAssets";
                    break;
                case HMSResultCode.CERT_FINGERPRINT_ERROR:
                    description += "SHA-256 certificate fingerprint doesn't match";
                    break;
                case HMSResultCode.ORDER_HWID_NOT_LOGIN:
                    description += "Not signed in Huawei AppGallery";
                    break;
            }
            return description;
        }
    }
}