using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BoGD
{
    [System.Serializable]
    public class LocalizedItem
    {
        [SerializeField]
        private string                               key = "";
        [SerializeField]
        private List<TranslatedValue>                values = new List<TranslatedValue>();

        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
            }
        }

        public List<TranslatedValue> Values
        {
            get
            {
                return values;
            }

            set
            {
                values = value;
            }
        }

        private Dictionary<SystemLanguage, TranslatedValue> localizedValues = null;
        public Dictionary<SystemLanguage, TranslatedValue> LocalizedValues
        {
            get
            {
                if (localizedValues == null)
                {
                    localizedValues = new Dictionary<SystemLanguage, TranslatedValue>();

                    for (int i = 0; i < Values.Count; i++)
                    {
                        localizedValues[Values[i].Language] = Values[i];
                    }
                }

                return localizedValues;
            }
        }

        public LocalizedItem()
        {
        }

        public LocalizedItem(string key, LocalizationType type, List<SystemLanguage> languages)
        {
            this.Key = key;
            for (int i = 0; i < languages.Count; i++)
            {
                Values.Add(new TranslatedValue(languages[i]));
            }
        }

        public LocalizedItem(string key, Dictionary<string, object> values)
        {
            Key = key;
            Deserealize(values);
        }

        public TranslatedValue GetValue(SystemLanguage language)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].Language == language)
                {
                    return Values[i];
                }
            }
            return null;
        }

        public bool Contains(string value)
        {
            foreach (var val in Values)
            {
                if (val.Value.Contains(value))
                {
                    return true;
                }
            }

            return Key.IndexOf(value) >= 0;
        }

        public Dictionary<string, object> Serialize()
        {
            var items = new Dictionary<string, object>();
            foreach (var item in values)
            {
                if (item.Value.Contains("TODO") || item.Value.Contains("ToDo"))
                {
                    continue;
                }

                if (items.ContainsKey(item.Language.ToSmall()))
                {
                    Debug.LogError("Item " + item.Value.ToString() + " lang " + item.Language.ToSmall() + " has double translation! check It!");
                }
                items.Add(item.Language.ToSmall(), item.Value);
            }
            return items;
        }

        public void Deserealize(Dictionary<string, object> dictionary)
        {
            foreach (var pair in dictionary)
            {
                SystemLanguage language = pair.Key.ToLanguage();
                TranslatedValue value = null;
                if (!LocalizedValues.TryGetValue(language, out value))
                {
                    value = new TranslatedValue(language, pair.Value == null ? "" : pair.Value.ToString());
                    Values.Add(value);
                    LocalizedValues[language] = value;
                }
                else
                {
                    value.Language = language;
                    value.Value = pair.Value == null ? "" : pair.Value.ToString();
                }

            }
        }
    }


    [System.Serializable]
    public class TranslatedValue
    {
        [SerializeField]
        private SystemLanguage   language = SystemLanguage.English;
        [SerializeField]
        private string           value = "Item";

        public SystemLanguage Language
        {
            get
            {
                return language;
            }

            set
            {
                language = value;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public TranslatedValue()
        {
        }

        public TranslatedValue(SystemLanguage language)
        {
            Language = language;
            Value = "TODO: " + language.ToString();
        }

        public TranslatedValue(SystemLanguage language, string value)
        {
            Language = language;
            Value = value;
        }

        public Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary[Language.ToSmall()] = Value;
            return dictionary;
        }
    }
}

