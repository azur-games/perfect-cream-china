using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{
    public class Analytics : AnalyticsBase
    {
        [SerializeField]
        private List<StaticType>    purchasesAnalitycs = new List<StaticType>() { StaticType.AnalyticsAppsFlyer , StaticType.AnalyticsAppMetrica };
        [SerializeField]
        private List<StaticType>    eventsAnalitycs = new List<StaticType>() { StaticType.AnalyticsAppMetrica };
        [SerializeField]
        private List<StaticType>    adAnalitycs = new List<StaticType>() { StaticType.AnalyticsFacebook, StaticType.AnalyticsFirebase };
        [SerializeField]
        private List<StaticType>    purchasesValidators = new List<StaticType>() { StaticType.AnalyticsAppsFlyer };
        [SerializeField]
        private bool                needLifeTime = true;
        [SerializeField]
        private bool                debug = false;

        private List<IAnalytics>    purchasesAnalyticsInstances = new List<IAnalytics>();
        private List<IAnalytics>    eventsAnalyticsInstances = new List<IAnalytics>();
        private List<IAnalytics>    purchasesValidatorsInstances = new List<IAnalytics>();
        private List<IAnalytics>    adAnalyticsInstances = new List<IAnalytics>();

        private Dictionary<StaticType, IAnalytics> dicts = new Dictionary<StaticType, IAnalytics>();

        public override void SendBuffer()
        {
            base.SendBuffer();
            foreach (var analytics in eventsAnalyticsInstances)
            {
                if (analytics == null)
                {
                    Debug.LogErrorFormat("Analytics {0} is null", analytics);
                    continue;
                }

                analytics.SendBuffer();
            }
        }

        private IAnalytics GetAnalytics(StaticType staticType)
        {
            IAnalytics res = null;
            if (!dicts.TryGetValue(staticType, out res))
            {
                res = staticType.Instance<IAnalytics>();
                if (res == null)
                {
                    Debug.LogErrorFormat("StaticType: {0} is NULL!", staticType);
                }
                dicts[staticType] = res;
            }

            return res;
        }

        private void Start()
        {
            if (Application.platform != RuntimePlatform.WindowsPlayer)
            {
                foreach (var staticType in purchasesAnalitycs)
                {
                    purchasesAnalyticsInstances.Add(GetAnalytics(staticType));
                }

                foreach (var staticType in eventsAnalitycs)
                {
                    eventsAnalyticsInstances.Add(GetAnalytics(staticType));
                }

                foreach (var staticType in purchasesValidators)
                {
                    purchasesValidatorsInstances.Add(GetAnalytics(staticType));
                }
                foreach (var staticType in adAnalitycs)
                {
                    adAnalyticsInstances.Add(GetAnalytics(staticType));
                }
            }
        }

        public override bool Validate(IInAppItem item, System.Action<IInAppItem, bool> callback)
        {
            if (purchasesValidatorsInstances.Count == 0)
            {
                return true;
            }

            foreach (var analytics in purchasesValidatorsInstances)
            {
                if (!analytics.Validate(item, callback))
                {
                    return false;
                }
            }

            return true;
        }

        public override void SendPurchase(IInAppItem item)
        {
            base.SendPurchase(item);

            foreach (var analytics in purchasesAnalyticsInstances)
            {
                analytics.SendPurchase(item);
            }
            if (debug)
            {
                Debug.LogWarningFormat("Send Purchase: {0}", item.ID);
            }
        }

        public override void SendEvent(string eventName, Dictionary<string, object> data)
        {
            foreach(var pair in Env.GetGlobalParameters())
            {
                data[pair.Key] = pair.Value;
            }

            base.SendEvent(eventName, data);

            foreach (var analytics in eventsAnalyticsInstances)
            {
                if (analytics == null)
                {
                    Debug.LogErrorFormat("Analytics {0} is null", analytics);
                    continue;
                }
                analytics.SendEvent(eventName, data);
            }

            if (debug)
            {
                Debug.LogWarningFormat("Sent Event: {0}; Data: {1}", eventName, data.ToJSON());
            }
        }
        
        public override void SendTechData()
        {
            var data = new Dictionary<string, object>
            {
                { "ram", SystemInfo.systemMemorySize },
                { "gpu_name", SystemInfo.graphicsDeviceName },
                { "cpu_name", SystemInfo.processorType },
                { "cpu_cores", SystemInfo.processorCount },
                { "cpu_freq", SystemInfo.processorFrequency },
                { "resolution", Screen.currentResolution.width + "x" + Screen.currentResolution.height },
                { "density", Screen.dpi }
            };

            Analytics.SendEvent("tech_data", data);
        }


        public override void SendADS(string eventName, Dictionary<string, object> data)
        {
            base.SendADS(eventName, data);

            foreach (var analytics in eventsAnalyticsInstances)
            {
                analytics.SendADS(eventName, data);
            }
        }
    }
}