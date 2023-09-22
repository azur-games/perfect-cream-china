using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class CoinsBoxProgressController : MonoBehaviour
{
    #region Fields

    [SerializeField] Camera canvasCamera;
    [SerializeField] Transform mainRoot;
    [SerializeField] Image progressBar;
    [SerializeField] Image newIcon;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CoinsFlightController flightController;
    [SerializeField] Transform simpleCoinsOrigin;
    [SerializeField] ParticleSystem sparks;
    [SerializeField] TextMeshProUGUI rewardLabel;
    [SerializeField] bool shouldHightlightFull = false;

    [Space]
    [Header("Flying coin animation settings")]
    [SerializeField] AnimationCurve flyingMoveCurve;
    [SerializeField] AnimationCurve flyingScaleCurve;
    [SerializeField] float targetScale = 0.1f;
    [SerializeField] float flyingDuration = 1.0f;

    [Space]
    [Header("Bounce animation settings")]
    [SerializeField] AnimationCurve bounceCurve;
    [SerializeField] float targetBounceScale = 1.1f;
    [SerializeField] float bounceDuration = 1.0f;

    [Space]
    [Header("Showing animation settings")]
    [SerializeField] bool isDynamicShowingEnabled = false;
    [SerializeField] AnimationCurve fadeCurve;
    [SerializeField] float fadeInDuration = 0.2f;
    [SerializeField] float fadeOutDuration = 0.2f;
    [SerializeField] float fadeOutDelay = 0.2f;

    [Space]
    [Header("Reward label animation settings")]
    [SerializeField] AnimationCurve rewardLabelScaleUpCurve;
    [SerializeField] float rewardLabelScaleUpDuration = 0.2f;
    [SerializeField] AnimationCurve rewardLabelScaleDownCurve;
    [SerializeField] float rewardLabelScaleDownDuration = 0.2f;

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        UpdateProgress();
        Env.Instance.Inventory.OnBucksBoxValueUpdated += Inventory_OnBucksBoxValueUpdated;

        canvasGroup.alpha = (isDynamicShowingEnabled) ? 0.0f : 1.0f;
        rewardLabel.gameObject.SetActive(false);
    }


    void OnDisable()
    {
        Env.Instance.Inventory.OnBucksBoxValueUpdated -= Inventory_OnBucksBoxValueUpdated;
    }


    void OnDestroy()
    {
        DOTween.Kill(this, true);
    }

    #endregion



    #region Update progress

    void UpdateProgress()
    {
        int coinsBoxMax = BalanceDataProvider.Instance.CoinsBoxMaxAmount;
        int currentCoinsAmount = Env.Instance.Inventory.BucksBox;
        progressBar.fillAmount = ((float) currentCoinsAmount) / coinsBoxMax;

        if (shouldHightlightFull && currentCoinsAmount >= coinsBoxMax)
        {
            newIcon.gameObject.SetActive(true);
        }
        else
        {
            newIcon.gameObject.SetActive(false);
        }
    }

    #endregion



    #region Animations

    Sequence FullAnimation(Transform objectRoot, Action callback = null)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(FlyingAnimation(objectRoot));
        sequence.AppendCallback(() => UpdateProgress());
        sequence.Append(BounceAnimation(onPeak: callback));

        if (isDynamicShowingEnabled)
        {
            sequence.Insert(0.0f, FadeInAnimation());
            sequence.AppendInterval(fadeOutDelay);
            sequence.Append(FadeOutAnimation());
        }

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Sequence SimpleShowAnimation(Action callback = null)
    {
        Sequence sequence = DOTween.Sequence();

        rewardLabel.gameObject.SetActive(true);
        rewardLabel.transform.localScale = Vector3.zero;
        rewardLabel.transform.position = simpleCoinsOrigin.position;

        sequence.AppendCallback(() => 
        {
            flightController.ProcessCoinsAnimation(simpleCoinsOrigin, () => 
            {
                Sequence reactionSequence = DOTween.Sequence();

                reactionSequence.Append(BounceAnimation(onPeak: callback));
                reactionSequence.Append(FadeOutAnimation());

                reactionSequence.SetTarget(this);
                reactionSequence.SetAutoKill(true);
            });
        });

        sequence.Insert(0.0f, FadeInAnimation());
        sequence.Insert(0.0f, RewardLabelAnimation().OnComplete(() => rewardLabel.gameObject.SetActive(false)));

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Sequence FlyingAnimation(Transform objectRoot, Action callback = null)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, MoveAnimation(objectRoot));
        sequence.Insert(0.0f, ScaleAnimation(objectRoot));

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);
        sequence.OnComplete(() => { callback?.Invoke(); });

        return sequence;
    }


    Tweener MoveAnimation(Transform objectRoot)
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


    Tweener ScaleAnimation(Transform objectRoot)
    {
        return objectRoot.DOScale(targetScale * objectRoot.localScale, flyingDuration).SetEase(flyingScaleCurve)
                                                                                      .SetTarget(this)
                                                                                      .SetAutoKill(true);
    }


   Tweener BounceAnimation(Action onPeak = null, Action onComplete = null)
    {
        Vector3 animationScale = targetBounceScale * mainRoot.localScale;
        int loopsCounter = 0;

        return mainRoot.DOScale(animationScale, 0.5f * bounceDuration).SetEase(bounceCurve)
                                                                      .SetTarget(this)
                                                                      .SetAutoKill(true)
                                                                      .SetLoops(2, LoopType.Yoyo)
                                                                      .OnStepComplete(() => 
                                                                      {
                                                                          loopsCounter++;

                                                                          if (loopsCounter == 1)
                                                                          {
                                                                              sparks.Play();
                                                                              onPeak?.Invoke();
                                                                          }
                                                                          else if (loopsCounter == 2)
                                                                          {
                                                                              onComplete?.Invoke();
                                                                          }
                                                                      });
    }


    Tweener FadeInAnimation()
    {
        return canvasGroup.DOFade(1.0f, fadeInDuration).SetEase(fadeCurve)
                                                       .SetTarget(this)
                                                       .SetAutoKill(true);
    }


    Tweener FadeOutAnimation()
    {
        return canvasGroup.DOFade(0.0f, fadeOutDuration).SetEase(fadeCurve)
                                                        .SetTarget(this)
                                                        .SetAutoKill(true);
    }


    Sequence RewardLabelAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(RewardLabelScaleUpAnimation());
        sequence.Append(RewardLabelScaleDownAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tweener RewardLabelScaleUpAnimation()
    {
        return rewardLabel.transform.DOScale(1.0f, rewardLabelScaleUpDuration).SetEase(rewardLabelScaleUpCurve)
                                                                              .SetTarget(this)
                                                                              .SetAutoKill(true);
    }


    Tweener RewardLabelScaleDownAnimation()
    {
        return rewardLabel.transform.DOScale(0.0f, rewardLabelScaleDownDuration).SetEase(rewardLabelScaleDownCurve)
                                                                                .SetTarget(this)
                                                                                .SetAutoKill(true);
    }

    #endregion



    #region Events handling

    void Inventory_OnBucksBoxValueUpdated(int amount, Transform animationRoot, Action callback)
    {
        if (animationRoot != null && amount >= 0)
        {
            FullAnimation(animationRoot, callback).Play();
        }
        else if (simpleCoinsOrigin != null && amount >= 0)
        {
            rewardLabel.text = "+" + amount;
            SimpleShowAnimation(() => 
            {
                UpdateProgress();
                callback?.Invoke();
            }).Play();
        }
        else
        {
            UpdateProgress();
            callback?.Invoke();
        }
    }

    #endregion
}