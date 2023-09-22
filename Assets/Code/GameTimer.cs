using System;
using System.Collections;
using UnityEngine;

namespace Code
{
    public class GameTimer : MonoBehaviour
    {
        private const string PlayTimeDataName = "PlayTimeData";
        public static GameTimer Instance { get; private set; }
        [field: SerializeField] public int PlayTime { get; private set; }

        private void Awake()
        {
            Instance = this;
            PlayTime = PlayerPrefs.GetInt(PlayTimeDataName);

            StartCoroutine(UpdatePlayTimeEnumerator());
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator UpdatePlayTimeEnumerator()
        {
            while (true)
            {
                PlayTime++;
                PlayerPrefs.SetInt(PlayTimeDataName, PlayTime);
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}