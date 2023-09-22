using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class KeysCounterController : MonoBehaviour
{
    #region Fields

    [SerializeField] Text text;
    [SerializeField] Camera canvasCamera;

    [Space]
    [Header("Flying key animation settings")]
    [SerializeField] AnimationCurve flyingMoveCurve;
    [SerializeField] AnimationCurve flyingScaleCurve;
    [SerializeField] float targetScale = 0.1f;
    [SerializeField] float flyingDuration = 1.0f;

    [Space]
    [Header("Bounce animation settings")]
    [SerializeField] AnimationCurve bounceCurve;
    [SerializeField] float targetBounceScale = 1.1f;
    [SerializeField] float bounceDuration = 1.0f;

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        UpdateCounter();
        Env.Instance.Inventory.OnKeysCountUpdated += Inventory_OnKeysCountUpdated;
    }


    void OnDisable()
    {
        Env.Instance.Inventory.OnKeysCountUpdated -= Inventory_OnKeysCountUpdated;
    }


    void OnDestroy()
    {
        DOTween.Kill(this, true);
    }

    #endregion



    #region Update counter

    void UpdateCounter()
    {
        text.text = Env.Instance.Inventory.Keys.ToString();
    }

    #endregion



    #region Animations

    void PlayFlyingAnimation(Transform objectRoot, Action callback = null)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, MoveAnimation(objectRoot));
        sequence.Insert(0.0f, ScaleAnimation(objectRoot));

        sequence.SetTarget(this)
                .SetAutoKill(true)
                .OnComplete(() => { callback?.Invoke(); })
                .Play();
    }


    Tween MoveAnimation(Transform objectRoot)
    {
        Vector3 counterViewportPosition = canvasCamera.WorldToViewportPoint(transform.position);
        Vector3 keyViewportPosition = Env.Instance.Rooms.GameplayRoom.Controller.Camera.WorldToViewportPoint(objectRoot.position);
        Vector3 targetWorldPosition = Env.Instance.Rooms.GameplayRoom.Controller.Camera.ViewportToWorldPoint(new Vector3(counterViewportPosition.x, 
                                                                                                                         counterViewportPosition.y, 
                                                                                                                         keyViewportPosition.z));

        return objectRoot.DOMove(targetWorldPosition, flyingDuration).SetEase(flyingMoveCurve)
                                                                     .SetTarget(this)
                                                                     .SetAutoKill(true);
    }


    Tween ScaleAnimation(Transform objectRoot)
    {
        return objectRoot.DOScale(targetScale * objectRoot.localScale, flyingDuration).SetEase(flyingScaleCurve)
                                                                                      .SetTarget(this)
                                                                                      .SetAutoKill(true);
    }


   Tween BounceAnimation(Action onPeak = null, Action onComplete = null)
    {
        Vector3 animationScale = targetBounceScale * transform.localScale;
        int loopsCounter = 0;

        DOTween.Kill(this, true);

        return transform.DOScale(animationScale, 0.5f * bounceDuration).SetEase(bounceCurve)
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

    void Inventory_OnKeysCountUpdated(int amount, Transform animationRoot, Action callback)
    {
        if (animationRoot != null)
        {
            PlayFlyingAnimation(animationRoot, () => 
            {
                BounceAnimation(() => 
                {
                    UpdateCounter();
                    callback?.Invoke();
                });
            });
        }
        else
        {
            UpdateCounter();
            callback?.Invoke();
        }
    }

    #endregion
} 