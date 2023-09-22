using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoGD
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class CheatButtonBase: CheatCheckerBase
    {
        [SerializeField]
        private UnityEngine.UI.Button       button = null;

        protected override void Start()
        {
            base.Start();

            if(button == null)
            {
                button = GetComponent<UnityEngine.UI.Button>();
            }

            if(button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
        }

        protected virtual void OnButtonClick()
        {
            
        }

        public void Reset()
        {
            if(button == null)
            {
                button = GetComponent<UnityEngine.UI.Button>();
            }
        }
    }
}