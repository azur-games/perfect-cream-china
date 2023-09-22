using System;
using System.Globalization;
using Modules.General.HelperClasses;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.RateUs
{
    public class RateUsCoolDown : MonoBehaviour
    {
        [SerializeField] private RateUsCoolDownData _rateUsCoolDownData;
        [SerializeField] private bool _coolDownAvailable;

        private MetagameRoomUI _metaGameRoomUI;
        private DateTime _midnightTimeUpdate;
        private DateTime _lastShowTime;

        public bool CoolDownAvailable
        {
            get => _coolDownAvailable;
            set => _coolDownAvailable = value;
        }

        public RateUsCoolDownData RateUsCoolDownData
        {
            get => _rateUsCoolDownData;
            set => _rateUsCoolDownData = value;
        }

        public void Constructor(MetagameRoomUI metaGameRoomUI)
        {
            _metaGameRoomUI = metaGameRoomUI;
        }

        public void ResetCoolDown()
        {
            _lastShowTime = default;
            _midnightTimeUpdate = DateTime.Now.AddDays(1);
            RateUsCoolDownData.availableImpressions--;
            CoolDownAvailable = false;
            SaveCoolDownData();
        }

        public bool CheckShowAvailable()
        {
            var ts = DateTime.Now - _lastShowTime;
            var isNewDay = IsNewDay();

            if (isNewDay)
            {
                if (!CustomPlayerPrefs.GetBool("DayLevelIndexReset"))
                {
                    CustomPlayerPrefs.SetInt("CurrentDayLevelIndexData", default);
                }
                
                CustomPlayerPrefs.SetBool("DayLevelIndexReset", true);
            }

            if (ts.TotalSeconds >= _metaGameRoomUI.RateUsConfiguration.MinTimeForReShowing)
            {
                if (RateUsCoolDownData.availableImpressions > 0)
                {
                    if (isNewDay)
                    {
                        if (CustomPlayerPrefs.GetInt("CurrentDayLevelIndexData") >= 2)
                        {
                            CoolDownAvailable = true;
                        }
                    }
                }
            }

            return CoolDownAvailable;
        }

        private bool IsNewDay()
        {
            var midnightTimeCoolDown = _midnightTimeUpdate - DateTime.Now;
            var isNewDay = (float)midnightTimeCoolDown.TotalSeconds <= 0;

            return isNewDay;
        }

        private void SaveCoolDownData()
        {
            RateUsCoolDownData.lastShowTime = DateTime.Now.ToString();
            RateUsCoolDownData.nextDayTimeUpdate = _midnightTimeUpdate.ToString();
            RateUsCoolDownData.initialized = true;

            var newJson = JsonUtility.ToJson(RateUsCoolDownData);
            PlayerPrefs.SetString("RateUsCoolDownData", newJson);
        }

        public void LoadCoolDownData()
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("RateUsCoolDownData"), RateUsCoolDownData);

            if (RateUsCoolDownData.initialized)
            {
                _lastShowTime = DateTime.Parse(RateUsCoolDownData.lastShowTime);
                _midnightTimeUpdate = DateTime.Parse(RateUsCoolDownData.nextDayTimeUpdate);
            }
            else
            {
                CoolDownAvailable = true;

                _midnightTimeUpdate = DateTime.Now.AddDays(1);
            }
        }
    }

    [Serializable]
    public class RateUsCoolDownData
    {
        public bool initialized;
        public int availableImpressions = 3;
        public int daySessions;
        public string nextDayTimeUpdate = "0";
        public string lastShowTime = "0";
    }
}