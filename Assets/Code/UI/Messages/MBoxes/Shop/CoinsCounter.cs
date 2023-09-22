using System;
using UnityEngine;
using UnityEngine.UI;

public class CoinsCounter : MonoBehaviour
{
    [SerializeField] Text _counter;
    [SerializeField] private CoinsFlightController flightController;

    private int _count;



    private void Awake()
    {
        UpdateCounter();
    }

    private void OnEnable()
    {
        Env.Instance.Inventory.OnBucksCountUpdated += Inventory_OnBucksCountUpdated;
        if(_count != Env.Instance.Inventory.Bucks)
        {
            UpdateCounter();
        }
    }

    private void OnDisable()
    {
        Env.Instance.Inventory.OnBucksCountUpdated -= Inventory_OnBucksCountUpdated;
    }

    private void UpdateCounter()
    {
        _count = Env.Instance.Inventory.Bucks;
        _counter.text = _count.ToString();
    }

    void Inventory_OnBucksCountUpdated(int newCount, Transform startTransform, Action callback)
    {
        if (newCount > 0 && startTransform)
        {
            flightController.ProcessCoinsAnimation(startTransform, () =>
            {
                callback?.Invoke();
                UpdateCounter();
            });
        }
        else
        {
            UpdateCounter();
        }
    }
}
