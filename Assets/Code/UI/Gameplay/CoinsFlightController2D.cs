using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class CoinsFlightController2D : MonoBehaviour
{
    [SerializeField] private GameObject coinOriginal;
    [SerializeField] private AnimationCurve flightCurve;
    [SerializeField] private float initialBoomSpeed = 1.0f;
    private List<Transform> coins = new List<Transform>();
    private List<Vector3> coinVectors = new List<Vector3>();
    private System.DateTime startTime;
    private bool inProgress = false;
    private float maxLifetime = float.MaxValue;
    private Action onFinished;
    private Vector3 targetPosition;

    private void Awake()
    {
        coinOriginal.SetActive(false);
        maxLifetime = flightCurve.keys[flightCurve.keys.Length - 1].time;
    }

    private void DestroyCoins()
    {
        foreach (Transform coin in coins)
        {
            GameObject.Destroy(coin.gameObject);
        }

        coins.Clear();
        coinVectors.Clear();
    }

    public void Play(Transform startTransform, Transform targetTransform, int coinsCount, Action onFinished)
    {
        this.onFinished = onFinished;

        DestroyCoins();

        targetPosition = targetTransform.localPosition;

        for (int i = 0; i < coinsCount; i++)
        {
            GameObject obj = Instantiate(coinOriginal, startTransform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);
            coins.Add(obj.transform);

            Vector3 moveVec = initialBoomSpeed * UnityEngine.Random.onUnitSphere;
            moveVec.z = 0.0f;
            coinVectors.Add(moveVec);
        }

        startTime = System.DateTime.Now;
        inProgress = true;

        this.gameObject.SetActive(true);

        UpdateSelf();
    }

    private void Update()
    {
        if (!inProgress) return;
        UpdateSelf();
    }

    private void UpdateSelf()
    {
        float timeSpan = (float)(System.DateTime.Now - startTime).TotalSeconds;
        float curveValue = flightCurve.Evaluate(timeSpan);

        for (int i = 0; i < coins.Count; i++)
        {
            Vector3 position = Vector3.Lerp(coinVectors[i] * timeSpan, targetPosition, curveValue);
            coins[i].localPosition = position;
        }

        if (timeSpan >= maxLifetime)
        {
            this.gameObject.SetActive(false);
            DestroyCoins();

            onFinished?.Invoke();
        }
    }
}
