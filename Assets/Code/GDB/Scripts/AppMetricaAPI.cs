using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{
    // Declaration of the Receipt structure for getting information about the IAP.
    [System.Serializable]
    public struct Receipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;
    }

    // Additional information about the IAP for Android.
    [System.Serializable]
    public struct PayloadAndroid
    {
        public string Json;
        public string Signature;
    }


    [System.Serializable]
    public class AppMetricaAPI : AnalyticsBase
    {
        public const string VERSION = "5.1.0";

        private static bool s_isInitialized;

        private static IYandexAppMetrica s_metrica;
        private static readonly object s_syncRoot = new Object();

        [SerializeField] private string ApiKey;

        [SerializeField] private bool ExceptionsReporting = true;

        [SerializeField] private uint SessionTimeoutSec = 10;

        [SerializeField] private bool LocationTracking = false;

        [SerializeField] private bool Logs = false;

        [SerializeField] private bool HandleFirstActivationAsUpdate = false;

        [SerializeField] private bool StatisticsSending = true;

        [SerializeField] private bool RevenueAutoTrackingEnabled = false;

        private bool _actualPauseStatus;

        public static IYandexAppMetrica Instance
        {
            get
            {
                if (s_metrica == null)
                {
                    lock (s_syncRoot)
                    {
#if UNITY_IPHONE || UNITY_IOS
                    if (s_metrica == null && Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        s_metrica = new YandexAppMetricaIOS();
                    }
#elif UNITY_ANDROID
                        if (s_metrica == null && Application.platform == RuntimePlatform.Android)
                        {
                            s_metrica = new YandexAppMetricaAndroid();
                        }
#endif
                        if (s_metrica == null)
                        {
                            s_metrica = new YandexAppMetricaDummy();
                        }
                    }
                }

                return s_metrica;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!s_isInitialized)
            {
                s_isInitialized = true;
                DontDestroyOnLoad(gameObject);
                SetupMetrica();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Instance.ResumeSession();
        }

        private void OnEnable()
        {
            if (ExceptionsReporting)
            {
#if UNITY_5 || UNITY_5_3_OR_NEWER
                Application.logMessageReceived += HandleLog;
#else
			Application.RegisterLogCallback(HandleLog);
#endif
            }
        }

        private void OnDisable()
        {
            if (ExceptionsReporting)
            {
#if UNITY_5 || UNITY_5_3_OR_NEWER
                Application.logMessageReceived -= HandleLog;
#else
			Application.RegisterLogCallback(null);
#endif
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_actualPauseStatus != pauseStatus)
            {
                _actualPauseStatus = pauseStatus;
                if (pauseStatus)
                {
                    Instance.PauseSession();
                }
                else
                {
                    Instance.ResumeSession();
                }
            }
        }

        private void SetupMetrica()
        {
            YandexAppMetricaConfig configuration = new YandexAppMetricaConfig(ApiKey)
            {
                SessionTimeout = (int)SessionTimeoutSec,
                Logs = Logs,
                HandleFirstActivationAsUpdate = HandleFirstActivationAsUpdate,
                StatisticsSending = StatisticsSending,
                LocationTracking = LocationTracking,
                RevenueAutoTrackingEnabled = RevenueAutoTrackingEnabled
            };

            Instance.ActivateWithConfiguration(configuration);
        }

        private static void HandleLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                Instance.ReportErrorFromLogCallback(condition, stackTrace);
            }
        }

        public override StaticType StaticType => StaticType.AnalyticsAppMetrica;


        public override void SendEvent(string eventName, Dictionary<string, object> data)
        {
            //Debug.LogFormat("========== ");
            //Debug.LogFormat("eventName = {0}", eventName);
            //foreach (var dat in data)
            //{
            //    Debug.LogFormat("Key = {0}, Value = {1}", dat.Key, dat.Value);
            //}
            //Debug.LogFormat(" ");
            base.SendEvent(eventName, data);
#if APPMETRICA_SUPPORTED
            AppMetrica.Instance.ReportEvent(eventName, data);
            //AppMetrica.Instance.ReportUserProfile()
#endif
        }

        public override void SendBuffer()
        {
            base.SendBuffer();

#if APPMETRICA_SUPPORTED
            AppMetrica.Instance.SendEventsBuffer();
            //AppMetrica.Instance.ReportUserProfile()
#endif
        }

        public override void SendPurchase(IInAppItem item)
        {
            base.SendPurchase(item);

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["inapp_id"] = item.ID;
            dictionary["currency"] = item.ISO;
            dictionary["price"] = (float)item.LocalizedPrice;
            //dictionary["transaction_id"] = item.TransactionID;
            dictionary["inapp_type"] = item.Type;
            SendEvent("payment_succeed", dictionary);
            Debug.Log("SendPurchase to AppMetrica: ".Color(Color.red) + item.ID);
        }


		public override bool Validate(IInAppItem item, System.Action<IInAppItem, bool> callback)
        {
#if APPMETRICA_SUPPORTED
            string currency = item.ISO;
            var price = item.LocalizedPrice;

            // Creating the instance of the YandexAppMetricaRevenue class.
            YandexAppMetricaRevenue revenue = new YandexAppMetricaRevenue(price, currency);
            if (item.Receipt != null)
            {
                // Creating the instance of the YandexAppMetricaReceipt class.
                YandexAppMetricaReceipt yaReceipt = new YandexAppMetricaReceipt();
                Receipt receipt = JsonUtility.FromJson<Receipt>(item.Receipt);
#if UNITY_ANDROID
                PayloadAndroid payloadAndroid = JsonUtility.FromJson<PayloadAndroid>(receipt.Payload);
                yaReceipt.Signature = payloadAndroid.Signature;
                yaReceipt.Data = payloadAndroid.Json;
#elif UNITY_IPHONE
                    yaReceipt.TransactionID = receipt.TransactionID;
                    yaReceipt.Data = receipt.Payload;
#endif
                revenue.Receipt = yaReceipt;

                AppMetrica.Instance.ReportRevenue(revenue);
                Debug.Log("=====> Validate revenue ");
            }
#endif
            return true;
        }
    }
}