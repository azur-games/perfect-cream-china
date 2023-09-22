using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace BoGD
{
    public class Localization : StaticBehaviour, ILocalization
    {
        [SerializeField]
        private SystemLanguage                  language = SystemLanguage.English;
        [SerializeField]
        private SystemLanguage                  editorLanguage = SystemLanguage.Russian;
        [SerializeField]
        private List<LocalizedItem>             items = null;
        [SerializeField]
        private List<SystemLanguage>            displayLanguages = null;
        [SerializeField]
        private List<SystemLanguage>            availableLanguages = null;
        [SerializeField]
        private int                             width = 128;
        [SerializeField]
        private int                             countOnPage = 32;
        

        public int Width => width;
        public int CountOnPage => countOnPage;

        public SystemLanguage CurrentLanguage => language;

        public List<SystemLanguage> AvailableLanguages => availableLanguages;

        public List<SystemLanguage> DisplayLanguages => displayLanguages;

        public List<LocalizedItem> Items => items;

        private Dictionary<string, LocalizedItem> localizedItems = null;

        private Dictionary<string, LocalizedItem> LocalizedItems
        {
            get
            {
                if (localizedItems == null)
                {
                    localizedItems = new Dictionary<string, LocalizedItem>();
                    foreach (var item in Items)
                    {
                        localizedItems[item.Key] = item;
                    }
                }
                return localizedItems;
            }
        }

        private Dictionary<string, string>  localizedKeys = null;
        private Dictionary<Text, string>    labelsKeys = null;

        public void SetSystemLanguage(SystemLanguage language)
        {
            this.language = language;
            Init(language);
            Event(Message.TranslateAll);
        }

        public List<SystemLanguage> GetAvailableLanguages()
        {
            return AvailableLanguages;
        }


        #region IStatic     
        public override StaticType StaticType => StaticType.Localization;
        #endregion

        #region ISubscriber 
        public override string Description
        {
            get
            {
                return "[Localization] " + name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Return System language enum element by it string name
        /// </summary>
        private SystemLanguage GetSystemLanguage(string language)
        {
            Array arr = Enum.GetValues(typeof(SystemLanguage));
            int arrItemsCount = arr.Length;
            for (int i = 0; i < arrItemsCount; ++i)
            {
                if (((SystemLanguage)(i)).ToString() == language)
                {
                    return (SystemLanguage)(i);
                }
            }

            return SystemLanguage.English;
        }

        public override void Reaction(Message type, params object[] parameters)
        {
            switch (type)
            {
                case Message.ProfileLoaded:
                    language = Application.isEditor ? editorLanguage : Application.systemLanguage;
                    Init(language);
                    break;

                case Message.OptionApply:
                    break;
            }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            Debug.Log(name + " AWAKE: " + language.ToString().FormatString("color:magenta"));
        }

        private void Start()
        {
            Init((Application.platform == RuntimePlatform.WindowsPlayer) ?
                        SystemLanguage.English :
                        Application.isEditor ?
                            editorLanguage :
                            Application.systemLanguage);
            StaticType.UI.AddSubscriber(this);
            
            if (Application.systemLanguage == SystemLanguage.Unknown)
            {
                SetSystemLanguage(SystemLanguage.English);
            }
        }

        private void Init(SystemLanguage language)
        {
            if (localizedKeys == null)
            {
                localizedKeys = new Dictionary<string, string>();
            }
            else
            {
                localizedKeys.Clear();
            }

            for (int i = 0; i < items.Count; i++)
            {
                LocalizedItem item = items[i];
                SystemLanguage tmpLanguage = language;

                if (!AvailableLanguages.Contains(tmpLanguage))
                {
                    tmpLanguage = SystemLanguage.English;
                }

                if (!item.LocalizedValues.ContainsKey(tmpLanguage))
                {
                    tmpLanguage = SystemLanguage.English;
                }

                if (item.LocalizedValues.ContainsKey(tmpLanguage))
                {
                    localizedKeys[item.Key] = item.LocalizedValues[tmpLanguage].Value;
                }
            }
            Debug.Log(name + " inited: " + language.ToString().FormatString("color:magenta"));
        }

        public bool TryLocalize(string key, out string value)
        {
            if (key.IsNullOrEmpty())
            {
                value = "none";
                return false;
            }

            bool result = localizedKeys.TryGetValue(key, out value);
            return result;
        }

        public Dictionary<string, object> Serialize()
        {
            var dictionary = new Dictionary<string, object>();

            //temporary test for equal word - keys in dictionary

            for (int i = 0; i < Items.Count - 1; ++i)
            {
                for (int j = i + 1; j < Items.Count; ++j)
                {
                    if (Items[i].Key == Items[j].Key && i != j)
                    {
                        Debug.LogError("Localization/Serialize It Is contains equal keys in dictionary: " + Items[i].Key);
                    }
                }
            }

            foreach (var item in Items)
            {
                if (dictionary.ContainsKey(item.Key))
                {
                    Debug.LogError("Localization/Serialize double key! " + item.Key + " value " + item.Values);
                }
                dictionary[item.Key] = item.Serialize();
            }
            return dictionary;
        }

        public void Deserealize(Dictionary<string, object> dictionary)
        {
            //Items.Clear();
            //LocalizedItems.Clear();
            //Items.Clear();
            //LocalizedItems.Clear();
            foreach (var pair in dictionary)
            {
                LocalizedItem item = null;
                if (!LocalizedItems.TryGetValue(pair.Key, out item))
                {
                    item = new LocalizedItem(pair.Key, (Dictionary<string, object>)pair.Value);
                    LocalizedItems[pair.Key] = item;
                    Items.Add(item);
                }
                else
                {
                    item.Deserealize((Dictionary<string, object>)pair.Value);
                }
            }
        }

        public void Save()
        {
            var file = Resources.Load<Files>("SystemInstantiator/Files");
            if(file == null)
			{
                CustomDebug.Log("file == null".Color(Color.green));
            }
            file.PutIntoFile(FileType.Localization, Serialize());
           
        }

        public void Load()
        {
            Deserealize(Resources.Load<Files>("SystemInstantiator/Files").GetFromFile(FileType.Localization));
        }
    }
}