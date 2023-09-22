using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{

    [System.Serializable]
    public class DataIntPrefs
    {
        [SerializeField]
        private string  key = "";
        [SerializeField]
        private int     defaultValue = 0;

        public int Value
        {
            get => PlayerPrefs.GetInt(key, defaultValue);
            set => PlayerPrefs.SetInt(key, value);
        }


        public void Increment(int delta)
        {
            Value += delta;
        }

        public DataIntPrefs()
        {

        }

        public DataIntPrefs(string key)
        {
            this.key = key;
        }

        public DataIntPrefs(string key, int defaultValue)
        {
            this.key = key;
            this.defaultValue = defaultValue;
        }
    }

    [System.Serializable]
    public class DataFloatPrefs
    {
        [SerializeField]
        private string      key = "";
        [SerializeField]
        private float       defaultValue = 0;

        public string Key
        {
            get => key;
            set => key = value;
        }

        public float Value
        {
            get => PlayerPrefs.GetFloat(key, defaultValue);
            set => PlayerPrefs.SetFloat(key, value);
        }


        public void Increment(int delta)
        {
            Value += delta;
        }

        public DataFloatPrefs()
        {

        }

        public DataFloatPrefs(string key)
        {
            this.key = key;
        }
    }

    [System.Serializable]
    public class RequirementTimeResource
    {
        [SerializeField]
        private DataFloatPrefs   timerData = null;
        [SerializeField]
        private float           delay;

        public float Delay
        {
            get => delay;
            set => delay = value;
        }

        public RequirementTimeResource(string key, float delay)
        {
            timerData = new DataFloatPrefs(key);
            this.delay = delay;
        }

        public bool Check()
        {
            return ExtensionsBase.CurrentTime() + delay > timerData.Value;
        }

        public void Increment(double delta)
        {
            timerData.Value = (float)(ExtensionsBase.CurrentTime() + delta);
        }

        public void Update()
        {
            Increment(0);
        }
    }

    public class DataStringPrefs
    {
        [SerializeField]
        private string      key = "";
        [SerializeField]
        private string      defaultValue;

        public string Key
        {
            get => key;
            set => key = value;
        }

        public string Value
        {
            get => PlayerPrefs.GetString(key, defaultValue);
            set => PlayerPrefs.SetString(key, value);
        }


        public void Increment(object delta)
        {
            Value += delta;
        }

        public DataStringPrefs()
        {

        }

        public DataStringPrefs(string key)
        {
            this.key = key;
        }
    }
}