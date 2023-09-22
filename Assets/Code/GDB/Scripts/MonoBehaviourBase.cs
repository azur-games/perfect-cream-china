using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{
    public class MonoBehaviourBase : MonoBehaviour
    {      
        private static IDevelopmentInfo developmentInfo = null;
        public static IDevelopmentInfo DevelopmentInfo
        {
            get
            {
                if (developmentInfo == null)
                {
                    developmentInfo = StaticType.DevelopmentInfo.Instance<IDevelopmentInfo>();
                }
                return developmentInfo;
            }
        }

        private static ICheatController cheatController = null;
        public static ICheatController CheatController
        {
            get
            {
                if (cheatController == null)
                {
                    cheatController = StaticType.CheatController.Instance<ICheatController>();
                }
                return cheatController;
            }
        }

        private static ILocalization localizator = null;
        public static ILocalization Localizator
        {
            get
            {
                if (localizator == null)
                {
                    localizator = StaticType.Localization.Instance<ILocalization>();
                }
                return localizator;
            }
        }
        private static IAnalytics analytics = null;
        public static IAnalytics Analytics
        {
            get
            {
                if (analytics == null)
                {
                    analytics = StaticType.Analytics.Instance<IAnalytics>();
                }
                return analytics;
            }
        }
        public static string GetUniqueID()
        {            
            return System.Guid.NewGuid().ToString();
        }
    }
}