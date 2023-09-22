using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoGD
{
    public class CheatController : StaticBehaviour, ICheatController
    {
        private static CheatController instance = null;

        public static CheatController Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = Instantiate(Resources.Load<CheatController>("SystemInstantiator/CheatController"));
                }
                return instance;
            }
        }

        public static bool CheatEnabled(CONSOLE.Cheat cheat)
        {
            return instance.HasCheat(cheat);
        }

        [Header("UI options")]
        [SerializeField]
        private Button                  buttonActivateOptions;

        [SerializeField]
        private GameObject              bgObject = null;
        [SerializeField]
        private Transform               root = null;

        [Header("CHEATS")]
        [SerializeField]
        private List<CONSOLE.Cheat>     availableCheats = new List<CONSOLE.Cheat>() { CONSOLE.Cheat.ForcePuchase, CONSOLE.Cheat.EnableAds, CONSOLE.Cheat.EnableUI };
        [SerializeField]
        private List<CONSOLE.Cheat>     activeCheats = new List<CONSOLE.Cheat>();
        [SerializeField]
        private bool                    cheatBuild = false;

        public override StaticType StaticType => StaticType.CheatController;

        public bool CheatBuid => cheatBuild;

        public Action OnChangeOptions
        {
            get;
            set;
        }

        private bool                    showOptions = false;

        protected override void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
            instance = this;
        }

        protected override void OnDestroy()
        {
            if(instance!= this)
            {
                return;
            }

            base.OnDestroy();
            instance = null;
        }

        private void Start()
        {
            buttonActivateOptions.gameObject.SetActive(cheatBuild);
            if (cheatBuild)
            {
                buttonActivateOptions.onClick.AddListener(OnCheatClick);

                foreach (var cheat in availableCheats)
                {
                    ItemCheat.Create(root, cheat, OnSwitchCheat, activeCheats.Contains(cheat));
                }
            }

            showOptions = false;
            UpdateOptionsVisible();
        }

        public bool HasCheat(CONSOLE.Cheat cheat)
        {
            return activeCheats.Contains(cheat);
        }

        private void OnSwitchCheat(CONSOLE.Cheat cheat, bool on)
        {
            if (on)
            {
                activeCheats.Add(cheat);
            }
            else
            {
                activeCheats.Remove(cheat);
            }
            OnChangeOptions?.Invoke();
            Event(Message.Cheat, cheat, on);
        }

        private void OnCheatClick()
        {
            showOptions = !showOptions;
            UpdateOptionsVisible();
        }

        private void UpdateOptionsVisible()
        {
            bgObject?.SetActive(showOptions);
            root?.gameObject.SetActive(showOptions);
        }
    }
}