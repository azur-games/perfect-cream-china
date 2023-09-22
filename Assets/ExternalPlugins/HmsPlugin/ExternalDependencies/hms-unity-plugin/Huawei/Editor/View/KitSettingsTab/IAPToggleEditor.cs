using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class IAPToggleEditor : ToggleEditor, IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string IAPKitEnabled = "IAPKit";

        public IAPToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(IAPKitEnabled);
            _toggle = new Toggle.Toggle("IAP", enabled, OnStateChanged, true);
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                CreateManagers();
            }
            else
            {
                DestroyManagers();
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(IAPKitEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            base.CreateManagers();
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            Enabled = false;
        }

        public override void DisableManagers()
        {
            Enabled = false;
        }
    }
}
