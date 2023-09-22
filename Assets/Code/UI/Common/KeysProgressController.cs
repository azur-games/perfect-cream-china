using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class KeysProgressController : MonoBehaviour
{
    #region Fields

    [SerializeField] Camera canvasCamera;
    [SerializeField] List<Transform> keys;

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



    #region Properties

    Transform CurrentKey
    {
        get
        {
            int keysInventory = Env.Instance.Inventory.Keys;
            return (keysInventory > 0 && keysInventory <= keys.Count) ? keys[keysInventory - 1] : null;
        }
    }

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        UpdateProgress();
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



    #region Update progress

    void UpdateProgress()
    {
        int availableKeys = keys.Count;

        for (int i = 0; i < availableKeys; i++)
        {
            keys[i].gameObject.SetActive(i < Env.Instance.Inventory.Keys);
        }
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
        Vector3 counterViewportPosition = canvasCamera.WorldToViewportPoint(CurrentKey.position);
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

        return CurrentKey.DOScale(animationScale, 0.5f * bounceDuration).SetEase(bounceCurve)
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
                                                                            else if (loopsCounter == 2)
                                                                            {
                                                                                onComplete?.Invoke();
                                                                            }
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
                UpdateProgress();
                
                BounceAnimation(() => 
                {
                    callback?.Invoke();
                });
            });
        }
        else
        {
            UpdateProgress();
            callback?.Invoke();
        }
    }

    #endregion
}