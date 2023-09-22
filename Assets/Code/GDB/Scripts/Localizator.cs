using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoGD
{
    [Serializable]
    public class LocalizationLabel
    {
        [SerializeField] private string key = "";
        [SerializeField] private Text text;

        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public void Localize()
        {
            if (text == null)
            {
                textMeshProUGUI.text = key.Translate();
            }
            else
            {
                text.text = key.Translate();
            }
        }
    }

    public class Localizator : MonoBehaviourBase, ISubscriber
    {
        [SerializeField] private LocalizationLabel[] labels;

        public string Description { get; set; }


        private void Start()
        {
            Localizator.AddSubscriber(this);
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Localize();
            }
        }


        private void Translate()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Localize();
            }
        }

        public void Reaction(Message message, params object[] parameters)
        {
            switch (message)
            {
                case Message.TranslateAll:
                    Translate();

                    break;
            }
        }


        private void OnDestroy()
        {
            Localizator.RemoveSubscriber(this);
        }
    }
}