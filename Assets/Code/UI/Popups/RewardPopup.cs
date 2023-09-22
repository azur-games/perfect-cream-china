using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Modules.General
{
    public class RewardPopup : BasePopup
    {
        #region Fields

        [Header("UI Buttons Settings")]
        [SerializeField] private Button claimButton = null;

        [Header("Descriptions Settings")]
        [SerializeField] protected TextMeshProUGUI rewardDescriptionLabel;

        #endregion



        #region Unity lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();

            claimButton?.onClick.AddListener(ClaimButton_OnClick);
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            claimButton?.onClick.RemoveListener(ClaimButton_OnClick);
        }

        #endregion



        #region Methods

        public virtual void Show(string rewardDescription, Action<bool> callback)
        {
            base.Show(callback);

            if (rewardDescriptionLabel != null)
            {
                rewardDescriptionLabel.text = rewardDescription;
            }

            //Env.Instance.SendPopup("subscription_reward", "reward", "show");
        }

        #endregion



        #region Events hadler

        private void ClaimButton_OnClick()
        {
            Hide(true);
        }


        public override void PopupHiden()
        {
            closeCallback?.Invoke(closeResult);
            closeCallback = null;
        }

        #endregion
    }
}
