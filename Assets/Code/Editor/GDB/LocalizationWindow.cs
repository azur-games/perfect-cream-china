using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BoGD.EDITOR
{
    public class LocalizationWindow : EditorWindow
    {
        private Localization                localization = null;
        private static LocalizationWindow   window = null;
        private Vector2                     scrollPos = new Vector2();
        private string                      id = "";

        private string                      filter = "";

        [MenuItem("Dev Tools/Localization")]
        private static void Open()
        {
            window = GetWindow<LocalizationWindow>();
            window.minSize = new Vector2(800, 600);
            window.titleContent.text = "Localization";
            window.Load();
        }

        private void Load()
        {
            localization = Resources.Load<Localization>("SystemInstantiator/Localization");
            Debug.Log(localization.Items.Count);
        }

        private int currentPage = 0;

        private void OnGUI()
        {
            if (localization == null)
            {
                return;
            }
            int heightPlus = 24;

            int y = 4 + 24;
            int x = 4 + 36;
            int height = 16;
            int width = localization.Width;
            int offset = 4;
            int countOnPage = localization.CountOnPage;
            Rect rect = new Rect(4, 4, width * (localization.DisplayLanguages.Count + 1), 16);

            rect.width /= 2;
            if (GUI.Button(rect, "Save"))
            {
                localization.Save();
            }

            rect.x += rect.width + 16;
            if (GUI.Button(rect, "Load"))
            {
                localization.Load();
            }

            rect.y += 16;
            rect.x = 4;

            EditorGUI.LabelField(rect, "Search:");
            rect.x += 92;
            filter = EditorGUI.TextField(rect, filter);
            y += 16;

            List<LocalizedItem> items = new List<LocalizedItem>();
            foreach (var item in localization.Items)
            {
                if (!item.Contains(filter))
                {
                    continue;
                }
                items.Add(item);
            }

            currentPage = DrawPages(4, y, 28, 20, currentPage, countOnPage, items);
            y += 12;
            scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, new Rect(0, 0, (localization.DisplayLanguages.Count + 2) * width, countOnPage * 1.1f * heightPlus));
            BeginWindows();

            Hat(x, y, offset, width, height, height);
            height += heightPlus / 2;
            height += 8;
            Table(x, y, offset, width, height, heightPlus, currentPage, countOnPage, items);

            y += height + (Mathf.Min(countOnPage, items.Count) + 2) * (heightPlus);
            currentPage = DrawPages(4, y, 28, 20, currentPage, countOnPage, items);

            EndWindows();
            GUI.EndScrollView();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(localization);
            }
        }

        private int DrawPages(int x, int y, int width, int height, int currentPage, int countOnPage, List<LocalizedItem> items)
        {
            var pageRect = new Rect(x, y, width, height);
            for (int i = 0; i < (items.Count / countOnPage) + 1; i++)
            {
                GUI.skin.GetStyle("Button").richText = true;
                if (GUI.Button(pageRect, i.ToString().Color(i == currentPage ? Color.yellow : Color.white)))
                {
                    currentPage = i;
                }
                pageRect.x += width;
            }
            y += 24;
            return currentPage;
        }

        private void Hat(float x, float y, float offset, float width, float height, float heightPlus)
        {
            EditorGUI.LabelField(new Rect(4, y + height, 32, heightPlus), "N");
            EditorGUI.LabelField(new Rect(x + 0 * (width + offset), y + height, width, heightPlus), "Key");

            Rect rect = new Rect(0, y + height, width - 32, heightPlus);
            for (int i = 0; i < localization.DisplayLanguages.Count; i++)
            {
                rect.width = width - 32;
                rect.x = x + (i + 1) * (width + offset);
                localization.DisplayLanguages[i] = (SystemLanguage)EditorGUI.EnumPopup(rect, localization.DisplayLanguages[i]);
                rect.x += width - 28;
                rect.width = 28;
                rect.height = heightPlus;
                if (GUI.Button(rect, "-"))
                {
                    localization.DisplayLanguages.RemoveAt(i);
                    break;
                }
            }

            if (GUI.Button(new Rect(x + (localization.DisplayLanguages.Count + 1) * (width + offset), y + height, 32, heightPlus), "+"))
            {
                localization.DisplayLanguages.Add(SystemLanguage.English);
            }
        }

        private void Table(float x, float y, float offset, float width, float height, float heightPlus, int page, int count, List<LocalizedItem> items)
        {
            Rect rect = new Rect(4, y + height, heightPlus - 2, heightPlus - 2);

            if (GUI.Button(rect, "+"))
            {
                AddItems(LocalizationType.Interface, id);
            }

            rect.x = x;
            rect.width = width;
            id = EditorGUI.TextField(rect, id);

            height += heightPlus;

            for (int i = (items.Count - 1) - (page * count); i >= Mathf.Max((items.Count - 1) - (page * count) - count, 0); i--)
            {
                rect.y = y + height;

                LocalizedItem item = items[i];

                if (!item.Contains(filter))
                {
                    continue;
                }

                rect.x = 4;
                EditorGUI.LabelField(rect, i.ToString());

                rect.x = x + 0 * (width + offset);

                rect.width = width;
                item.Key = EditorGUI.TextField(rect, item.Key);

                for (int j = 0; j < localization.DisplayLanguages.Count; j++)
                {
                    SystemLanguage language = localization.DisplayLanguages[j];
                    TranslatedValue translateValue = item.GetValue(language);
                    if (translateValue == null)
                    {
                        translateValue = new TranslatedValue(language);
                        item.Values.Add(translateValue);
                    }

                    rect.x = x + (j + 1) * (width + offset);
                    translateValue.Value = EditorGUI.TextField(rect, translateValue.Value);
                }

                rect.x = x + (localization.DisplayLanguages.Count + 1) * (width + offset);
                rect.width = 36;

                if (GUI.Button(rect, "-"))
                {
                    RemoveItem(item);
                    break;
                }

                height += heightPlus;
            }
        }

        private void RemoveItem(LocalizedItem item)
        {
            localization.Items.Remove(item);
        }

        public void AddItems(LocalizationType type, int id)
        {
            if (id == -1)
            {
                return;
            }
            AddItems(type, id.ToString());

        }

        /// <summary>
        /// Добавление итема
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public void AddItems(LocalizationType type, string id)
        {
            if (id == "")
            {
                return;
            }

            List<SystemLanguage> languages = localization.AvailableLanguages;
            localization.Items.Add(new LocalizedItem(id, type, languages));
        }

    }
}