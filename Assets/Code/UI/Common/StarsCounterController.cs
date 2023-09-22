using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class StarsCounterController : MonoBehaviour
{
    #region Fields

    [SerializeField] Text text;

    [Space]
    [Header("Gain animation settings")]
    [SerializeField] AnimationCurve gainAnimationCurve;
    [SerializeField] float gainAnimationScale = 1.1f;
    [SerializeField] float gainAnimationDuration;


    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        UpdateCounter();
        Env.Instance.Inventory.OnStarsCountUpdated += Inventory_OnStarsCountUpdated;
    }


    void OnDisable()
    {
        Env.Instance.Inventory.OnStarsCountUpdated -= Inventory_OnStarsCountUpdated;
    }


    void OnDestroy()
    {
        DOTween.Kill(this, true);
    }

    #endregion



    #region Update counter

    void UpdateCounter()
    {
        text.text = Env.Instance.Inventory.Stars.ToString();
    }

    #endregion



    #region Animations

    Tween GainAnimation(Action onPeak = null, Action onComplete = null)
    {
        Vector3 animationScale = gainAnimationScale * transform.localScale;
        int loopsCounter = 0;

        DOTween.Kill(this, true);

        return transform.DOScale(animationScale, 0.5f * gainAnimationDuration).SetEase(gainAnimationCurve)
                                                                              .SetTarget(this)
                                                                              .SetAutoKill(true)
                                                                              .SetLoops(2, LoopType.Yoyo)
                                                                              .OnStepComplete(() => 
                                                                              {
                                                                                  loopsCounter++;

                                                                                  if (loopsCounter == 1)
                                                                                  {
                                                                                      onPeak?.Invoke();
                                                                                  }
                                                                              })
                                                                              .OnComplete(() => 
                                                                              {
                                                                                  if (loopsCounter < 1)
                                                                                  {
                                                                                      onPeak?.Invoke();
                                                                                  }

                                                                                  onComplete?.Invoke();
                                                                              });
    }

    #endregion



    #region Events handling

    void Inventory_OnStarsCountUpdated(int amount, Transform animationRoot, Action onCounterUpdated)
    {
        if (amount > 0)
        {
            GainAnimation(() => 
            {
                UpdateCounter();
                onCounterUpdated?.Invoke();
            });
        }
        else
        {
            UpdateCounter();
            onCounterUpdated?.Invoke();
        }
    }

    #endregion
} 