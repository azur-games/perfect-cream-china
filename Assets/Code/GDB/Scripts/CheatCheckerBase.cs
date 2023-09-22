using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{
    public class CheatCheckerBase: MonoBehaviourBase, ISubscriber
    {
        [SerializeField]
        private CONSOLE.Cheat       cheat =  CONSOLE.Cheat.None;
        [SerializeField]
        private List<GameObject>    objects = null;

        public string Description
        {
            get => "[CheatChecker] => " + name;
            set => throw new System.NotImplementedException();
        }

        public virtual void Reaction(Message message, params object[] parameters)
        {
            switch(message)
            {
                case Message.Cheat:
                    if (parameters.Get<CONSOLE.Cheat>() == cheat)
                    {
                        foreach (var go in objects)
                        {
                            go.SetActive(parameters.Get<bool>());
                        }
                    }
                    break;
            }
        }

        private void Reset()
        {
            objects = new List<GameObject>() { gameObject };
        }

        private void UpdateState()
        {
            foreach (var go in objects)
            {
                go.SetActive(CheatController.HasCheat(cheat));
            }
        }

        protected virtual void Start()
        {
            if (CheatController.CheatBuid)
            {
                CheatController.AddSubscriber(this);
                UpdateState();
            }
        }

        private void OnDestroy()
        {
            CheatController.RemoveSubscriber(this);
        }
    }
}