using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoGD.UI.PERFECTCREAM
{
    public class WindowCancelMembership : MonoBehaviour
    {
        [SerializeField] private string url = "https://myaccount.google.com/u/2/subscriptions";
        [SerializeField] private Button buttonNo = null;
        [SerializeField] private Button buttonURL = null;
        [SerializeField] private Button buttonClose = null;

        [SerializeField] private TextMeshProUGUI labelEveryDayReward;

        private void Start()
        {
            buttonNo.onClick.AddListener(OnNoClick);
            buttonURL.onClick.AddListener(OnURLClick);
            buttonClose.onClick.AddListener(OnNoClick);

            labelEveryDayReward.text = string.Format("label_every_day".Translate(), BalanceDataProvider.Instance.SubscriptionCoinsReward);
        }

        private void OnNoClick()
        {
            gameObject.SetActive(false);
        }

        private void OnURLClick()
        {
            Application.OpenURL(url);
        }
    }
}