using System;
using TMPro;
using UnityEngine;


namespace Modules.General
{
    public class MessagePopup : BasePopup
    {
        #region Fields

        [Header("Descriptions Settings")]
        [SerializeField] private TextMeshProUGUI messageLabel;

        #endregion



        #region Methods

        public void Show(Action<bool> callback, string message)
        {
            base.Show(callback);

            if (messageLabel != null)
            {
                messageLabel.text = message;
            }
        }

        #endregion
    }
}
