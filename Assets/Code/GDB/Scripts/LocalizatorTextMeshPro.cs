using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace BoGD
{
    [System.Serializable]
    public class LocalizationLabelTextMeshPro
    {
        [SerializeField]
        private string              key = "";
        [SerializeField]
        private TextMeshProUGUI     text = null;

        public string Key => key;
        public TextMeshProUGUI Text => text;

        public void Localize()
        {
            if (text == null)
            {
                Debug.LogErrorFormat("Text field for key '{0}' was not found! —¿Õﬂ »—œ–¿¬‹!", key);
                return;
            }
            text.text = key.Translate();
        }
    }

    public class LocalizatorTextMeshPro : MonoBehaviourBase, ISubscriber
    {
        [SerializeField]
        private LocalizationLabelTextMeshPro[] labels;

        public string Description { get; set; }

        private List<TextMeshProUGUI> tranlated = new List<TextMeshProUGUI>();

        public void Reaction(Message message, params object[] parameters)
        {
            switch (message)
            {
                case Message.TranslateAll:
                    Translate();
                    break;
            }
        }

        private void Start()
        {
            Localizator.AddSubscriber(this);

            foreach (var label in labels)
            {
                if (label.Text == null)
                {
                    Debug.LogErrorFormat("Label {0} is null!!!", this.name);
                    continue;
                }

                if (tranlated.Contains(label.Text))
                {
                    Debug.LogErrorFormat(label.Text, "Label {0} is already localized!!! on object {1}", label.Text.name, this.name);
                    continue;
                }

                tranlated.Add(label.Text);
            }

            if (Application.isEditor)
            {
                TextMeshProUGUI[] allLabels = GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var label in allLabels)
                {
                    if (!tranlated.Contains(label))
                    {
                        //Debug.LogErrorFormat(label, "Label {0} is not localized!!!", label.name);
                    }
                }
            }

            Translate();
        }

		private void Translate()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Localize();
            }
        }

        private void OnDestroy()
        {
            Localizator.RemoveSubscriber(this);
        }
    }
}