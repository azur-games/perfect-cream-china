using UnityEngine;
using UnityEditor;
using HmsPlugin;
using System;
using UnityEditor.SceneManagement;

namespace HmsPlugin
{
    public class AdsToggleEditor : ToggleEditor, IDrawer
    {
        private Toggle.Toggle _toggle;

        public const string AdsKitEnabled = "AdsKit";

        public AdsToggleEditor(TabBar tabBar)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(AdsKitEnabled);
            _toggle = new Toggle.Toggle("Ads", enabled, OnStateChanged, true);
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
            HMSMainEditorSettings.Instance.Settings.SetBool(AdsKitEnabled, value);
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