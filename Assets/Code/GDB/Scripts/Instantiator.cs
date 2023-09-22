using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BoGD
{
    /// <summary>
    /// Any objects intstantiator
    /// </summary>
    public class Instantiator : MonoBehaviour, ISender
    {
        public static Instantiator Create(GameObject gameObject, string path)
        {
            Instantiator instantiator = gameObject.AddComponent<Instantiator>();
            instantiator.path = path;
            instantiator.setParent = true;
            return instantiator;
        }

        [SerializeField]
        private string      path = "SystemInstantiator";

        [SerializeField]
        private bool        setParent = false;
        [SerializeField]
        private bool        check = true;

        [SerializeField]
        private Method      method = Method.Start;

#if UNITY_EDITOR

        public string Path
        {
            set => path = value;
        }
        public bool SetParent
        {
            set => setParent = value;
        }
        public bool Check
        {
            set => check = value;
        }
        public Method Method
        {
            set => method = value;
        }

#endif

        public GameObject[] Instantiated
        {
            get;
            set;
        }

        private Dictionary<string, GameObject> instantiatedDict = new Dictionary<string, GameObject>();

        private Object[]    resources = null;

        #region ISender
        public string Description
        {
            get
            {
                return "[Instantiator] " + name;
            }
            set
            {
                name = value;
            }
        }

        private List<ISubscriber>       subscribers = new List<ISubscriber>();

        public List<ISubscriber> Subscribers => subscribers;

        public void AddSubscriber(ISubscriber subscriber)
        {
            if (subscribers.Contains(subscriber))
            {
                return;
            }
            subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public void Event(Message type, params object[] parameters)
        {
            for (int i = 0; i < subscribers.Count; i++)
            {
                subscribers[i].Reaction(type, parameters);
            }
        }
        #endregion

        protected virtual void Awake()
        {
            if (method == Method.Awake)
            {
                Instantiate();
            }
        }

        protected virtual void Start()
        {
            if (method == Method.Start)
            {
                //Debug.Log("Instantiate from start:");
                Instantiate();
            }
        }

        public virtual void DestroyAll()
        {
            for (int i = 0; i < Instantiated.Length; i++)
            {
                Destroy(Instantiated[i]);
            }

            Resources.UnloadUnusedAssets();

            resources = null;

            Destroy(this);
        }

        public virtual T[] InstantiateAll<T>() where T : MonoBehaviour
        {
            Instantiate();
            T[] result = new T[Instantiated.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Instantiated[i].GetComponent<T>();

                if (result[i] == null)
                {
                    Debug.Log("Instantiated object '" + Instantiated[i].name + "' has not component '" + typeof(T).ToString() + "'", Instantiated[i]);
                }
            }
            return result;
        }

        public virtual void Instantiate()
        {
            resources = Resources.LoadAll(path);
            Instantiated = new GameObject[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                if (check && (GameObject.Find(resources[i].name) != null || GameObject.Find(resources[i].name + "(Clone)") != null))
                {
                    continue;
                }
                // Debug.Log("[" + name + "] " + (i+1) + " instance for " + resources[i].name);
                Instantiated[i] = (GameObject)Instantiate(resources[i], setParent ? transform : null, false);
                //Instantiated[i].name = resources[i].name;
            }
        }
    }
}
