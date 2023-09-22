using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.RateUs
{
    public class RateUsButton : MonoBehaviour
    {
        private Button _button;
        private int _currentRateUsResult;

        private void OnEnable()
        {
            _currentRateUsResult = PlayerPrefs.GetInt("RateUsResult");
            _button = GetComponent<Button>();

            if (_currentRateUsResult >= 5)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _button.onClick.AddListener(() =>
                {
                    RateUsFeedbackPopupScreen.ShowRatePopupIfCan();
                    Env.Instance.SendSettings("rate_us");
                });
            }
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}