using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoGD
{
    public static class ExtensionsBase
    {
        public static DateTime              epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        public static float                 lastTimeGet;
        public static float                 lastTimeGetTimeStamp;
        public static double                lastTime;

        public static DateTime ToDateTime(this double seconds)
        {
            DateTime result = epochStart;
            result = result.AddSeconds(seconds).ToLocalTime();
            return result;
        }

        public static DateTime ToDateTime(this long seconds)
        {
            DateTime result = epochStart;
            result = result.AddSeconds(seconds).ToLocalTime();
            return result;
        }

        public static double GetTimestamp(this DateTime date)
        {
            float time = Time.time;
            if (lastTimeGetTimeStamp == time)
            {
                return lastTime;
            }
            lastTimeGetTimeStamp = time;
            lastTime = (date - epochStart).TotalSeconds;

            return lastTime;
        }

        public static double CurrentTime()
        {
            float time = Time.time;
            if (lastTimeGet == time)
            {
                return lastTime;
            }
            lastTimeGet = time;
            lastTime = (DateTime.UtcNow - epochStart).TotalSeconds;
            //DebugTools.Error(DebugSubject.Bubka, "UTC NOW: {0}: EPOCH: {1}, SECONDS: {2}", DateTime.UtcNow, epochStart, lastTime);

            return lastTime;
        }

        public static string ToSmall(this SystemLanguage lanuage)
        {
            switch (lanuage)
            {
                case SystemLanguage.Russian:
                    return "RU";

                case SystemLanguage.English:
                    return "EN";

                case SystemLanguage.French:
                    return "FR";

                case SystemLanguage.Spanish:
                    return "ES-EU";

                case SystemLanguage.Italian:
                    return "IT";

                case SystemLanguage.ChineseSimplified:
                    return "ZN";

                case SystemLanguage.German:
                    return "DE";

                case SystemLanguage.Japanese:
                    return "JA";

                case SystemLanguage.Korean:
                    return "KO";

                case SystemLanguage.Portuguese:
                    return "PT-EU";

                case SystemLanguage.Thai:
                    return "TH";

                case SystemLanguage.Turkish:
                    return "TR";

                case SystemLanguage.Unknown:
                    return "PT-BR";

                case SystemLanguage.Catalan:
                    return "ES-LATAM";
            }
            return lanuage.ToString().ToLower();
        }

        public static SystemLanguage ToLanguage(this string lanuage)
        {
            switch (lanuage)
            {
                case "RU":
                    return SystemLanguage.Russian;

                case "EN":
                    return SystemLanguage.English;

                case "FR":
                    return SystemLanguage.French;

                case "ES-EU":
                    return SystemLanguage.Spanish;

                case "IT":
                    return SystemLanguage.Italian;

                case "ZN":
                    return SystemLanguage.ChineseSimplified;

                case "DE":
                    return SystemLanguage.German;

                case "JA":
                    return SystemLanguage.Japanese;

                case "KO":
                    return SystemLanguage.Korean;

                case "PT-EU":
                    return SystemLanguage.Portuguese;

                case "TH":
                    return SystemLanguage.Thai;

                case "TR":
                    return SystemLanguage.Turkish;

                case "PT-BR":
                    return SystemLanguage.Unknown;

                case "ES-LATAM":
                    return SystemLanguage.Catalan;
            }

            try
            {
                return (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), lanuage, true);
            }

            catch
            {
                return SystemLanguage.Afrikaans;
            }
        }

        public static void Event(this StaticType staticType, Message message, params object[] parameters)
        {
            staticType.Instance().Event(message, parameters);
        }

        public static void Reaction(this StaticType staticType, Message message, params object[] parameters)
        {
            if (!staticType.Exists())
            {
                return;
            }

            staticType.Instance().Reaction(message, parameters);
        }

        public static void AddSubscriber(this StaticType staticType, ISubscriber subscriber)
        {
            if (!staticType.Exists())
            {
                return;
            }

            staticType.Instance().AddSubscriber(subscriber);
        }


        public static void RemoveSubscriber(this StaticType staticType, ISubscriber subscriber)
        {
            if (!staticType.Exists())
            {
                return;
            }

            staticType.Instance().RemoveSubscriber(subscriber);
        }

        public static bool Exists(this StaticType staticType)
        {
            IStatic iStatic = StaticContainer.Get(staticType);
            return iStatic != null && !iStatic.IsEmpty;
        }

        public static T Instance<T>(this StaticType staticType) where T : IStatic
        {
            return StaticContainer.Get<T>(staticType);
        }

        public static IStatic Instance(this StaticType staticType)
        {
            return StaticContainer.Get(staticType);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static double ExtractNumber(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary == null)
            {
                Debug.LogErrorFormat("Dictionary is null! '{0}'", key);
                return 0;
            }

            object obj = null;
            if (!dictionary.TryGetValue(key, out obj))
            {
                Debug.LogWarningFormat("Key was not found '{0}'", key);
                return 0;
            }

            if(obj.GetType() == typeof(string))
            {
                double result;
                if(double.TryParse(obj.ToString(), out result))
                {
                    return result;
                }

                return 0;
            }

            if (obj.GetType() == typeof(int))
            {
                return (int)obj;
            }

            if (obj.GetType() == typeof(long))
            {
                return (long)obj;
            }            

            return (double)obj;
        }

        /// <summary>
        /// Извлеччение данных из словаря
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Extract<T>(this Dictionary<string, object> dictionary, string key, T defaultValue = default(T))
        {
            if (dictionary == null)
            {
                Debug.LogWarningFormat("Dictionary is null! '{0}'", key);
                return defaultValue;
            }

            object obj = null;
            if (!dictionary.TryGetValue(key, out obj))
            {
                Debug.LogWarningFormat("Key was not found '{0}'", key);
                return defaultValue;
            }

            T result = defaultValue;
            if (typeof(T).IsEnum)
            {
                result = (T)System.Enum.Parse(typeof(T), (string)obj, true);
            }
            else
            {
                try
                {
                    result = (T)obj;
                }
                catch
                {
                    Debug.LogWarningFormat("Can't convert '{0}' to format '{1}', it '{2}'", key, typeof(T), obj.GetType());
                    return result;
                }

                //if (typeof(T).IsArray)
                //{ 
                //    Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)obj;
                //    result = array.ToObject<T>();
                //}
                //else
                //{
                //    result = (T)obj;
                //}
            }
            return result;
        }        

        public static Vector2 XZ(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }

        public static void SetGlobalScale(this Transform transform, Vector3 scale)
        {
            var m = transform.worldToLocalMatrix;
            m.SetColumn(0, new Vector4(m.GetColumn(0).magnitude, 0f));
            m.SetColumn(1, new Vector4(0f, m.GetColumn(1).magnitude));
            m.SetColumn(2, new Vector4(0f, 0f, m.GetColumn(2).magnitude));
            m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            transform.localScale = m.MultiplyPoint(scale);
        }
        public static void SetGlobalScale(this Transform transform, Vector3 scale, Vector3 globalscale)
        {
            var m = transform.worldToLocalMatrix;
            m.SetColumn(0, new Vector4(m.GetColumn(0).magnitude * globalscale.x, 0f));
            m.SetColumn(1, new Vector4(0f, m.GetColumn(1).magnitude * globalscale.y));
            m.SetColumn(2, new Vector4(0f, 0f, m.GetColumn(2).magnitude * globalscale.z));
            m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            transform.localScale = m.MultiplyPoint(scale);
        }

        public static string FormatString(this string need, string format)
        {
            Dictionary<TextParameter, string> dict = new Dictionary<TextParameter, string>();
            string[] lst = format.Split(new char[] { ';' });
            for (int i = 0; i < lst.Length; i++)
            {
                string[] tmp = lst[i].Split(new char[] { ':' });
                if (tmp[0].ToLower() == "color")
                {
                    dict.Add(TextParameter.Color, tmp[1]);
                }
                if (tmp[0].ToLower() == "size")
                {
                    dict.Add(TextParameter.Size, tmp[1]);
                }
                if (tmp[0].ToLower() == "style")
                {
                    dict.Add(TextParameter.Style, tmp[1]);
                }
            }

            return need.FormatString(dict);
        }

        public static string FormatString(this string need, Dictionary<TextParameter, string> parameters)
        {
            string res = need;
            if (parameters.ContainsKey(TextParameter.Color))
            {
                res = string.Format("<color={0}>{1}</color>", parameters[TextParameter.Color], res);
            }
            if (parameters.ContainsKey(TextParameter.Size))
            {
                res = string.Format("<size={0}>{1}</size>", parameters[TextParameter.Size], res);
            }
            if (parameters.ContainsKey(TextParameter.Style))
            {
                string[] tmp = parameters[TextParameter.Style].Split(new char[] { ',' });
                for (int i = 0; i < tmp.Length; i++)
                {
                    res = string.Format("<{0}>{1}</{0}>", tmp[i], res);
                }
            }
            return res;
        }

        public static T Get<T>(this object[] parameters, int index = 0)
        {
            T result = default(T);
            int currentIndex = -1;
            System.Type targetType = typeof(T);
#if NETFX_CORE
            TypeInfo targetTypeInfo = targetType.GetTypeInfo();
#else
            System.Type targetTypeInfo = targetType;
#endif
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    continue;
                }
#if NETFX_CORE
                TypeInfo type = parameters[i].GetType().GetTypeInfo();
#else
                System.Type type = parameters[i].GetType();
#endif
                if (targetTypeInfo.IsAssignableFrom(type) ||
                    type.IsAssignableFrom(targetTypeInfo) ||
                    type.IsSubclassOf(targetType))
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        result = (T)parameters[i];
                        break;
                    }
                }
            }
            return result;
        }

        public static string GetTimeString(this System.DateTime dateTime)
        {
            string time = (dateTime.Hour < 10 ? "0" : "") + dateTime.Hour + ":" + (dateTime.Minute < 10 ? "0" : "") + dateTime.Minute + ":" + (dateTime.Second < 10 ? "0" : "") + dateTime.Second;
            return time;
        }
        public static string Size(this string need, float size)
        {
            return "<size=" + size + ">" + need + "</size>";
        }

        public static string Color(this string need, Color color)
        {
            return "<color=" + color.ToHex() + ">" + need + "</color>";
        }

        public static string Color(this string need, string color)
        {
            return "<color=" + color + ">" + need + "</color>";
        }

        private static ILocalization Localization => MonoBehaviourBase.Localizator;

        private static int TranslateIndex = 1;
        public static string Translate(this string key)
        {
            if (Localization == null)
            {
                return key;
            }

            string result = key;
            if (!Localization.TryLocalize(key, out result))
            {
                if (Localization.TryLocalize(key + "." + TranslateIndex, out result))
                {
                    return result;
                }

                if (Localization.TryLocalize(key + ".1", out result))
                {
                    return result;
                }

                return key;
            }

            return result;
        }
    }
    
    /*public static class JsonHelper
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
    }*/
}