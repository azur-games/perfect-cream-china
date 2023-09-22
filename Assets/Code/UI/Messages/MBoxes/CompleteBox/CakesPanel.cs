using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CakesPanel : MonoBehaviour
{
    #region Fields

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image[] cakesIcons;
    [SerializeField] private ParticleSystem[] cakesEffects;

    [Space]
    [Header("Cakes scale animations settings")]
    [SerializeField] private AnimationCurve cakeScaleUpCurve;
    [SerializeField] [Min(0.0f)] private float cakeScaleUpDuration = 0.3f;
    [SerializeField] private AnimationCurve cakeScaleDownCurve;
    [SerializeField] [Min(0.0f)] private float cakeScaleDownDuration = 0.3f;

    [Space]
    [Header("Show/Hide animations settings")]
    [SerializeField] [Min(0.0f)] private float cakesTimeOffset = 0.1f;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] [Min(0.0f)] private float fadeInDuration = 0.15f;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] [Min(0.0f)] private float fadeOutDuration = 0.15f;


    private int activeCakesCount = 0;

    #endregion



    #region Unity lifecycle

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    #endregion



    #region Show/Hide

    public void Show(Action callback = null)
    {
        activeCakesCount = 0;

        Reset();
        ShowAnimation().OnComplete(() => 
        {
            callback?.Invoke();
        }).Play();
    }


    public void Hide(Action callback = null)
    {
        HideAnimation().OnComplete(() => 
        {
            Reset();
            callback?.Invoke();
        }).Play();
    }


    void Reset()
    {
        canvasGroup.alpha = 0.0f;
        ResetCakes();
    }

    #endregion



    #region Cakes handling

    public bool TrySpendCake()
    {
        if (Env.Instance.Inventory.TrySpendKeys(1))
        {
            TryHideLastCake();
            return true;
        }
        return false;
    }


    void ResetCakes()
    {
        for (int i = 0; i < cakesIcons.Length; i++)
        {
            cakesIcons[i].gameObject.SetActive(true);
            cakesIcons[i].transform.localScale = Vector3.zero;
        }
    }


    bool TryHideLastCake()
    {
        if (activeCakesCount < 1)
            return false;

        activeCakesCount--;
        CakeScaleDownAnimation(cakesIcons[activeCakesCount].transform).Play();

        return true;
    }

    #endregion



    #region Cakes sound keys

    string CakeScaleUpSoundKey(int index)
    {
        switch (index)
        {
            case 0:
                return AudioKeys.UI.StarMini1;

            case 1:
                return AudioKeys.UI.StarMini2;

            case 2:
                return AudioKeys.UI.StarMini3;
        }

        return AudioKeys.UI.StarMini1;
    }

    #endregion



    #region Show/Hide animations

    Sequence ShowAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(FadeInAnimation());
        sequence.Append(CakesShowingAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Sequence HideAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(CakesHidingAnimation());
        sequence.Append(FadeOutAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }

    #endregion



    #region Cake animations

    Sequence CakesShowingAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        activeCakesCount = Mathf.Min(Env.Instance.Inventory.Keys, cakesIcons.Length);
        for (int i = 0; i < activeCakesCount; i++)
        {
            int cakeIndex = i;
            sequence.Insert(i * cakesTimeOffset, CakeScaleUpAnimation(cakesIcons[i].transform).OnComplete(() => 
            {
                if (cakeIndex < cakesEffects.Length)
                {
                    Env.Instance.Sound.PlaySound(CakeScaleUpSoundKey(cakeIndex));
                    cakesEffects[cakeIndex].Play();
                }
            }));
        }

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Sequence CakesHidingAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < activeCakesCount; i--)
        {
            sequence.Insert(i * cakesTimeOffset, CakeScaleDownAnimation(cakesIcons[activeCakesCount - i - 1].transform));
        }

        activeCakesCount = 0;

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tweener CakeScaleUpAnimation(Transform cake)
    {
        return cake.DOScale(1.0f, cakeScaleUpDuration).SetEase(cakeScaleUpCurve)
                                                      .SetTarget(this)
                                                      .SetAutoKill(true);
    }


    Tweener CakeScaleDownAnimation(Transform cake)
    {
        return cake.DOScale(0.0f, cakeScaleDownDuration).SetEase(cakeScaleDownCurve)
                                                        .SetTarget(this)
                                                        .SetAutoKill(true);
    }

    #endregion



    #region Fade In/Out animations

    Tweener FadeInAnimation()
    {
        return canvasGroup.DOFade(1.0f, fadeInDuration).SetEase(fadeInCurve)
                                                       .SetTarget(this)
                                                       .SetAutoKill(true);
    }


    Tweener FadeOutAnimation()
    {
        return canvasGroup.DOFade(0.0f, fadeOutDuration).SetEase(fadeOutCurve)
                                                        .SetTarget(this)
                                                        .SetAutoKill(true);
    }

    #endregion
}
