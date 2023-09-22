using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoGD
{
    public class ItemCheat : MonoBehaviour
    {
        public static void Create(Transform root, CONSOLE.Cheat cheat, System.Action<CONSOLE.Cheat, bool> callback, bool on)
        {
            var item = Instantiate( Resources.Load<ItemCheat>("UI/Items/ItemCheat"));
            item.transform.SetParent(root);
            item.cheat = cheat;
            item.OnSwitchCheat = callback;
            item.toggle.isOn = on;
            item.text.text = cheat.ToString();
            item.name = "[ItemCheat] " + cheat.ToString();
        }


        [SerializeField]
        private Toggle          toggle = null;
        [SerializeField]
        private Text            text = null;

        private CONSOLE.Cheat   cheat = CONSOLE.Cheat.None;

        public System.Action<CONSOLE.Cheat, bool> OnSwitchCheat
        {
            get;
            set;
        }


        private void Start()
        {
            toggle.onValueChanged.AddListener(OnChangeToggle);
        }

        private void OnChangeToggle(bool value)
        {
            OnSwitchCheat?.Invoke(cheat, value);
        }
    }
}