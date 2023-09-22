using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class CoinsPiggyRewardBox : UIMessageBox
{
    #region Fields

    [SerializeField] List<Transform> coins;
    [SerializeField] Transform glow;
    [SerializeField] Transform coinsTarget;
    [SerializeField] Image piggyIcon;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI rewardLabel;
    [SerializeField] ParticleSystem confettiEffect;
    [SerializeField] ParticleSystem[] bigConfetti;

    [Space]
    [Header("Background dynamics settings")]
    [SerializeField] [Range(0.0f, 1.0f)] float backgroundTargetAlpha = 0.75f;
    [SerializeField] AnimationCurve backgroundFadeInCurve;
    [SerializeField] [Min(0.0f)] float backgroundFadeInDuration = 0.2f;
    [SerializeField] AnimationCurve backgroundFadeOutCurve;
    [SerializeField] [Min(0.0f)] float backgroundFadeOutDuration = 0.2f;

    [Space]
    [Header("Coins dynamics settings")]
    [SerializeField] [Min(0.0f)] float coinsAppearanceDelay = 0.15f;
    [SerializeField] [Min(0.0f)] float coinsAppearanceTimeOffset = 0.1f;
    [SerializeField] AnimationCurve coinsScaleUpCurve;
    [SerializeField] [Min(0.0f)] float coinsScaleUpDuration = 0.2f;
    [SerializeField] [Min(0.0f)] float coinsFlightDuration = 0.5f;

    [Space]
    [Header("Piggy/glow dynamics settings")]
    [SerializeField] AnimationCurve piggyScaleUpCurve;
    [SerializeField] [Min(0.0f)] float piggyScaleUpDuration = 0.3f;
    [SerializeField] AnimationCurve piggyScaleDownCurve;
    [SerializeField] [Min(0.0f)] float piggyScaleDownDuration = 0.3f;
    [SerializeField] AnimationCurve piggyBounceCurve;
    [SerializeField] float piggyBounceTargetScale = 1.2f;
    [SerializeField] [Min(0.0f)] float piggyBounceDuration = 0.2f;
    
    [Space]
    [Header("Reward label animation settings")]
    [SerializeField] AnimationCurve rewardLabelScaleUpCurve;
    [SerializeField] [Min(0.0f)] float rewardLabelScaleUpDuration = 1.0f;
    [SerializeField] AnimationCurve rewardLabelScaleDownCurve;
    [SerializeField] [Min(0.0f)] float rewardLabelScaleDownDuration = 0.3f;
    
    [Space]
    [Header("Common dynamics settings")]
    [SerializeField] [Min(0.0f)] float hideDelay = 0.3f;

    Action onHide = null;

    #endregion



    #region Unity lifecycle

    void OnDestroy()
    {
        DOTween.Kill(this);
    }

    #endregion


    
    #region Reset elements

    void ResetElements()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            coins[i].gameObject.SetActive(true);
            coins[i].transform.localScale = Vector3.zero;
        }

        piggyIcon.transform.localScale = Vector3.zero;
        glow.transform.localScale = Vector3.zero;
        rewardLabel.transform.localScale = Vector3.zero;

        rewardLabel.text = "+" + BalanceDataProvider.Instance.CoinsBoxPeriodicReward;

        background.color = new Color(background.color.r, background.color.g, background.color.b, 0.0f);
    }

    #endregion



    #region Initialization

    public void Init(Action onHide = null)
    {
        this.onHide = onHide;

        ResetElements();
        ShowAnimation().OnComplete(ShowAnimation_OnComplete).Play();
    }

    #endregion



    #region Close self

    void CloseSelf()
    {
        Close();
        onHide?.Invoke();
    }

    #endregion



    #region Show/Hide animations

    Sequence ShowAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, BackgroundFadeInAnimation());
        sequence.Insert(0.0f, PiggyScaleUpAnimation());
        sequence.Insert(0.0f, GlowScaleUpAnimation());
        sequence.AppendInterval(coinsAppearanceDelay);
        sequence.Append(CoinsScaleUpAnimation());
        sequence.Append(CoinsFlightAnimation().OnComplete(CoinsFlightAnimation_OnComplete));
        sequence.Append(RewardLabelAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Sequence HideAnimation(float delay)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(delay, PiggyScaleDownAnimation());
        sequence.Insert(delay, GlowScaleDownAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    void ShowAnimation_OnComplete()
    {
        int currentCoins = Env.Instance.Inventory.BucksBox;
        int maxCoins = BalanceDataProvider.Instance.CoinsBoxMaxAmount;

        if (currentCoins >= maxCoins)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(hideDelay);
            sequence.AppendCallback(() => 
            {
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
                {
                    Env.Instance.UI.Messages.ShowCoinsPiggyBox(() => 
                    {
                        CloseSelf();
                    });

                    overlay.Close();
                });
            });

            sequence.SetTarget(this);
            sequence.SetAutoKill(true);
            sequence.Play();
        }
        else
        {
            HideAnimation(hideDelay).OnComplete(HideAnimation_OnComplete).Play();
        }
    }


    void HideAnimation_OnComplete()
    {
        CloseSelf();
    }

    #endregion



    #region Coins sound keys

    string CoinScaleUpSoundKey(int index)
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


    string CoinFlightSoundKey(int index)
    {
        switch (index)
        {
            case 0:
                return AudioKeys.UI.Star01;

            case 1:
                return AudioKeys.UI.Star02;

            case 2:
                return AudioKeys.UI.Star03;
        }

        return AudioKeys.UI.Star01;
    }

    #endregion



    #region Background animations

    Tweener BackgroundFadeInAnimation()
    {
        return background.DOFade(backgroundTargetAlpha, backgroundFadeInDuration).SetEase(backgroundFadeInCurve)
                                                                                 .SetTarget(this)
                                                                                 .SetAutoKill(true);
    }


    Tweener BackgroundFadeOutAnimation()
    {
        return background.DOFade(0.0f, backgroundFadeOutDuration).SetEase(backgroundFadeOutCurve)
                                                                 .SetTarget(this)
                                                                 .SetAutoKill(true);
    }

    #endregion



    #region Coins animations
    
    Tweener CoinScaleUpAnimation(Transform coin)
    {
        return coin.DOScale(1.0f, coinsScaleUpDuration).SetEase(coinsScaleUpCurve)
                                                       .SetTarget(this)
                                                       .SetAutoKill(true);
    }


    Sequence CoinsScaleUpAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < coins.Count; i++)
        {
            string audioKey = CoinScaleUpSoundKey(i);
            sequence.Insert(i * coinsAppearanceTimeOffset, CoinScaleUpAnimation(coins[i]).OnStart(() => 
            {
                Env.Instance.Sound.PlaySound(audioKey);
            }));
        }

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tweener CoinFlightScaleDownAnimation(Transform coin)
    {
        return coin.DOScale(0.0f, coinsFlightDuration).SetTarget(this).SetAutoKill(true);
    }


    Tweener CoinFlightMoveAnimation(Transform coin)
    {
        return coin.DOLocalMove(coinsTarget.localPosition, coinsFlightDuration).SetTarget(this)
                                                                               .SetAutoKill(true);
    }


    Sequence CoinFlightAnimation(Transform coin)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, CoinFlightMoveAnimation(coin));
        // sequence.Insert(0.0f, CoinFlightScaleDownAnimation(coin));

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }

    
    Sequence CoinsFlightAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < coins.Count; i++)
        {
            int index = i;
            Transform coin = coins[i];
            Sequence coinAnimation = CoinFlightAnimation(coin).OnComplete(() => 
            {
                Env.Instance.Sound.PlaySound(CoinFlightSoundKey(index));

                coin.gameObject.SetActive(false);

                if (index == coins.Count - 1)
                {
                    Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

                    confettiEffect.Play();
                    TryStartBigConfetti();
                }
            });

            sequence.Insert(i * coinsAppearanceTimeOffset, coinAnimation);
            sequence.Insert(i * coinsAppearanceTimeOffset + coinAnimation.Duration(), PiggyBounceAnimation());
        }

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    void TryStartBigConfetti()
    {
        int reward = BalanceDataProvider.Instance.CoinsBoxPeriodicReward;
        if (Env.Instance.Inventory.BucksBox + reward >= BalanceDataProvider.Instance.CoinsBoxMaxAmount)
        {
            for (int i = 0; i < bigConfetti.Length; i++)
            {
                bigConfetti[i].Play();
            }
        }
    }


    void CoinsFlightAnimation_OnComplete()
    {
        int reward = BalanceDataProvider.Instance.CoinsBoxPeriodicReward;
        Env.Instance.Inventory.IncreaseBucksBoxValue(reward);
        UserActivityChecker.Instance.UpdateCoinsBoxPeriodicRewardTime();
    }

    #endregion



    #region Piggy animations

    Tweener PiggyScaleUpAnimation()
    {
        return piggyIcon.transform.DOScale(1.0f, piggyScaleUpDuration).SetEase(piggyScaleUpCurve)
                                                                      .SetTarget(this)
                                                                      .SetAutoKill(true);
    }


    Tweener PiggyScaleDownAnimation()
    {
        return piggyIcon.transform.DOScale(0.0f, piggyScaleDownDuration).SetEase(piggyScaleDownCurve)
                                                                        .SetTarget(this)
                                                                        .SetAutoKill(true);
    }


    Tweener PiggyBounceAnimation()
    {
        return piggyIcon.transform.DOScale(piggyBounceTargetScale, 0.5f * piggyBounceDuration).SetEase(piggyBounceCurve)
                                                                                              .SetLoops(2, LoopType.Yoyo)
                                                                                              .SetTarget(this)
                                                                                              .SetAutoKill(true);
    }

    #endregion



    #region Glow animations

    Tweener GlowScaleUpAnimation()
    {
        return glow.transform.DOScale(1.0f, piggyScaleUpDuration).SetEase(piggyScaleUpCurve)
                                                                 .SetTarget(this)
                                                                 .SetAutoKill(true);
    }


    Tweener GlowScaleDownAnimation()
    {
        return glow.transform.DOScale(0.0f, piggyScaleDownDuration).SetEase(piggyScaleDownCurve)
                                                                   .SetTarget(this)
                                                                   .SetAutoKill(true);
    }

    #endregion



    #region Reward label animations

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
}