using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class CoinsFlightController : MonoBehaviour
{
    [SerializeField] private Transform targetRoot;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private AnimationCurve flightOutCure;
    [SerializeField] private float flightOutTime;
    [SerializeField] private AnimationCurve flightCurve;
    [SerializeField] private float flightTime;
    [SerializeField] private float scaleTime;
    [SerializeField] private int flyingCoinsCount = 7;
    [SerializeField] private float movementRandomSide;


    public float AnimationTime => flightOutTime + flightTime;


    void OnDestroy()
    {
        DOTween.Kill(this, true);
    }


    public void ProcessCoinsAnimation(Transform startTransform, Action onFinished = null)
    {
        List<Transform> flyingCoins = new List<Transform>();
        for (int i = 0; i < flyingCoinsCount; i++)
        {
            GameObject obj = Instantiate(coinPrefab, targetRoot);
            obj.name = "Coin" + i;
            obj.transform.position = startTransform.position;
            obj.transform.localPosition = obj.transform.localPosition.SetZ(0f);
            obj.transform.localScale = Vector3.zero;
            flyingCoins.Add(obj.transform);
        }

        Sequence sequence = DOTween.Sequence();
        foreach (var coin in flyingCoins)
        {
            Vector3 movementVector = new Vector2(UnityEngine.Random.Range(-movementRandomSide, movementRandomSide), 
                                                 UnityEngine.Random.Range(-movementRandomSide, movementRandomSide));

            sequence.Insert(0f, coin.DOScale(1f, scaleTime).SetTarget(this).SetAutoKill(true));
            sequence.Insert(0f, coin.DOLocalMove(coin.transform.localPosition + movementVector, flightOutTime).SetEase(flightOutCure).SetTarget(this).SetAutoKill(true));
            sequence.Insert(flightOutTime, coin.DOLocalMove(Vector3.zero, flightTime).SetEase(flightCurve).SetTarget(this).SetAutoKill(true));
        }

        sequence.OnComplete(() =>
        {
            foreach (var coin in flyingCoins)
            {
                Destroy(coin.gameObject);
            }

            flyingCoins.Clear();
            onFinished?.Invoke();
        });

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);
    }
}
