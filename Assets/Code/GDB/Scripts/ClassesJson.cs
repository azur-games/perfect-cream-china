using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NEWTOTSOFT_JSON_SUPPORTED
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
#endif

using System.IO;
using System.Linq;

namespace BoGD
{
    [System.Serializable]
    public class FileSettings
    {
        [SerializeField]
        private string                      path = "";
        [SerializeField]
        private FileType                    type = FileType.None;

        private string                      text = null;
        private string                      fullPath = null;

        private Dictionary<string, object>  dictionary = null;

        public Dictionary<string, object> Dictionary => dictionary;

        public string Path => path;


        public FileType Type => type;

        public string Text => text;

        public void Load(string basePath, string phSec)
        {
            fullPath = basePath + "/" + path;
            //Debug.LogFormat("Try to load '{0}'", fullPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarningFormat("File '{0}' is not found!", fullPath.Color(Color.blue));
                return;
            }

            string txt = File.ReadAllText(fullPath);
            text = txt;
            //text = EncStr.Decrypt(txt, phSec);

            dictionary = JsonHelper.FromJSON(text);
        }

        public void Save(string basePath, Dictionary<string, object> dictionary, string phSec)
        {
            this.dictionary = dictionary;
            Save(basePath, phSec);
        }

        public void Save(string basePath, string phSec)
        {
            fullPath = basePath + "/" + path;
            string text = "";
#if NEWTOTSOFT_JSON_SUPPORTED
            text = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
#endif
            //File.WriteAllText(fullPath, EncStr.Encrypt(text, phSec));
            File.WriteAllText(fullPath, text);
            //Debug.LogFormat("Save '{0}' to file '{1}'", type, fullPath);

            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.Formatting = Formatting.Indented;
            //
            //using (StreamWriter file = File.CreateText(fullPath))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serializer.Serialize(file, dictionary);
            //}
            //

            //JObject jobj = new JObject(dictionary);
            //
            //using (StreamWriter file = File.CreateText(@"c:\videogames.json"))
            //using (JsonTextWriter writer = new JsonTextWriter(file))
            //{
            //    jobj.WriteTo(writer);
            //}
        }

        public FileSettings()
        {

        }

        public FileSettings(string path, FileType type)
        {
            this.path = path;
            this.type = type;
        }
    }

    public static class JsonHelper
    {
        public static string ToJSON<T1, T2>(this Dictionary<T1, T2> dictionary)
        {
            if (dictionary == null)
            {
                return "";
            }

#if NEWTOTSOFT_JSON_SUPPORTED
            return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
#endif
            return "";
        }

        public static string ToJSON(this Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
            {
                return "";
            }

#if NEWTOTSOFT_JSON_SUPPORTED
            return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
#endif
            return "";
        }

        public static Dictionary<string, object> GetByKeys(this Dictionary<string, object> dictionary, params object[] keys)
        {
            Dictionary<string, object> result = dictionary;
            string key = "";
            for (int i = 0; i < keys.Length; i++)
            {
                key += keys[i] + " -> ";
                object tmp = null;
                if (!result.TryGetValue(keys[i].ToString(), out tmp))
                {
                    Debug.LogErrorFormat("Key not found: '{0}'", key);
                    return result;
                }

                result = (Dictionary<string, object>)tmp;
            }
            return result;
        }

        public static Dictionary<string, object> FromJSON(this string json)
        {
            return (Dictionary<string, object>)json.Deserealize();
        }

        public static object Deserealize(this string json)
        {
#if NEWTOTSOFT_JSON_SUPPORTED
            return ToObject(JToken.Parse(json));
#endif
            return null;
        }

#if NEWTOTSOFT_JSON_SUPPORTED
        private static object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
#endif

        public static string GetString(this Dictionary<string, object> dictionary)
        {
            string result = "";
            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                string value = "";
                if (pair.Value == null)
                {
                    value = "NULL";
                }
                else
                {
                    if (pair.Value.GetType() == typeof(Dictionary<string, object>))
                    {
                        value = ((Dictionary<string, object>)pair.Value).GetString();
                    }
                    else
                    {
                        value = pair.Value.ToString();
                    }
                }
                result += pair.Key + ": " + value + "; ";
            }
            return result;
        }
    }
}
