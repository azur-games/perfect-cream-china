using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CheatUIRootMarker: CheatCheckerBase
    {
        private bool        subscribed = false;

        protected override void Start()
        {
            base.Start();
            if(!subscribed)
            {
                CheatController.OnChangeOptions += UpdateCheatState;
                UpdateCheatState();
                subscribed = true;
            }
        }

        private void OnEnable()
        {
            UpdateCheatState();
            if(!subscribed)
            {
                CheatController.OnChangeOptions += UpdateCheatState;
                subscribed = true;
            }
        }

        private void UpdateCheatState()
        {
            if(CheatController.CheatBuid)
            {
                gameObject.SetActive(!CheatController.HasCheat(CONSOLE.Cheat.EnableUI));
            }
        }

        private void OnDestroy()
        {
            CheatController.OnChangeOptions -= UpdateCheatState;
        }
    }
}