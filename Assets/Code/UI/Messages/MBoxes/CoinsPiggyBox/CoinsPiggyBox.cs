using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Modules.Advertising;
using Modules.Analytics;
using Modules.General.Abstraction;
using System.Collections.Generic;
using TMPro;
using BoGD;

[Serializable]
public class CoinsPiggyBox : UIMessageBox
{
    #region Fields

    [SerializeField] Button getButton;
    [SerializeField] TextMeshProUGUI getButtonLabel;
    [SerializeField] VideoButton videoButton;
    [SerializeField] TextMeshProUGUI videoButtonLabel;
    [SerializeField] Button closeButton;
    [SerializeField] Image piggyIcon;
    [SerializeField] Image progressBar;
    [SerializeField] TextMeshProUGUI progressLabel;
    [SerializeField] Transform coinsRoot;
    [SerializeField] ParticleSystem confetti;
    [SerializeField] ParticleSystem chillConfetti;
    [SerializeField] ParticleSystem[] bigConfetti;
    [SerializeField] [Range(0.0f, 1.0f)] float progressBarMax = 1.0f;

    [Space]
    [Header("Common dynamics settings")]
    [SerializeField] [Min(0.0f)] float closingDelay = 0.5f;

    [Space]
    [Header("Progress bar animation settings")]
    [SerializeField] AnimationCurve progressCurve;
    [SerializeField] [Min(0.0f)] float progressDuration = 0.5f;

    [Space]
    [Header("Confetti dynamics settings")]
    [SerializeField] [Min(0.0f)] float bigConfettiDelay = 0.2f;
    [SerializeField] [Min(0.0f)] float afterBigConfettiDelay = 0.3f;

    [Space]
    [Header("Piggy idle animation settings")]
    [SerializeField] AnimationCurve idleBounceCurve;
    [SerializeField] [Min(0.0f)] float idleBounceDuration = 1.0f;
    [SerializeField] [Min(0.0f)] float idleIntensiveBounceDuration = 1.5f;
    [SerializeField] float idleBounceTarget = 1.1f;

    [Space]
    [Header("Piggy coins reaction settings")]
    [SerializeField] [Min(0.0f)] float piggyCoinsBounceDuration = 0.2f;
    [SerializeField] float piggyCoinsBounceTarget = 1.2f;

    [Space]
    [Header("Shines")]
    [SerializeField] Transform weakShine;
    [SerializeField] Transform strongShine;

    private Action onHide;

    #endregion



    #region Properties

    int VideoRewardMultiplier
    {
        get
        {
            if (BalanceDataProvider.Instance.IsCoinsBoxRewardOnlyForVideo)
                return 1;

            return BalanceDataProvider.Instance.CoinsBoxVideoMultiplier;
        }
    }

    bool IsCoinsBoxFull => Env.Instance.Inventory.BucksBox >= BalanceDataProvider.Instance.CoinsBoxMaxAmount;

    #endregion



    #region Initialization

    public void Init(Action onHide = null)
    {
        this.onHide = onHide;

        videoButton.Init(AdModule.RewardedVideo, "coins_piggy_box", VideoButton_OnClick);

        bool isRewardOnlyForVideo = BalanceDataProvider.Instance.IsCoinsBoxRewardOnlyForVideo;
        bool isVideoButtonAvailable = (VideoRewardMultiplier > 1 || isRewardOnlyForVideo) && IsCoinsBoxFull;
        videoButton.gameObject.SetActive(isVideoButtonAvailable);

        if (isRewardOnlyForVideo)
        {
            getButtonLabel.text = "OK";
            videoButtonLabel.text = "GET";
        }
        else
        {
            getButtonLabel.text = "GET";
            videoButtonLabel.text = "GET X" + VideoRewardMultiplier;

            getButton.interactable = IsCoinsBoxFull;
        }

        UpdateProgress();

        chillConfetti.gameObject.SetActive(IsCoinsBoxFull);

        if (IsCoinsBoxFull)
        {
            weakShine.gameObject.SetActive(false);
            strongShine.gameObject.SetActive(true);

            PiggyIdleBounceAnimation(idleIntensiveBounceDuration);
        }
        else
        {
            weakShine.gameObject.SetActive(true);
            strongShine.gameObject.SetActive(false);

            PiggyIdleBounceAnimation(idleBounceDuration);
        }

        UserActivityChecker.Instance.UpdateCoinsBoxOpenTime();

        Env.Instance.SendWindow("piggy_bank");
    }

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        UpdateProgress();

        closeButton.onClick.AddListener(CloseButton_OnClick);
        getButton.onClick.AddListener(GetButton_OnClick);
    }


    void OnDisable()
    {
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
        getButton.onClick.RemoveListener(GetButton_OnClick);
    }


    void OnDestroy()
    {
        DOTween.Kill(this, true);
        DOTween.Kill(piggyIcon, true);
    }

    #endregion



    #region Progress bar

    void UpdateProgress()
    {
        int maxCoins = BalanceDataProvider.Instance.CoinsBoxMaxAmount;
        int currentCoins = Env.Instance.Inventory.BucksBox;
        float progress = ((float)currentCoins) / maxCoins;

        progressBar.fillAmount = progressBarMax * progress;
        progressLabel.text = currentCoins.ToString() + "/" + maxCoins.ToString();
    }

    #endregion



    #region Closing handling

    void CloseSelfWithDelay(float delay)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delay);
        sequence.AppendCallback(() => CloseSelf());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        sequence.Play();
    }


    void CloseSelf()
    {
        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            overlay.Close();

            Close();
            onHide?.Invoke();
        });
    }

    #endregion



    #region Animations

    Sequence RewardAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, PiggyCoinsReactionAnimation());
        sequence.Insert(0.0f, ProgressBarAnimation());
        sequence.Insert(bigConfettiDelay, BigConfetti());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tweener ProgressBarAnimation()
    {
        float duration = (progressBar.fillAmount / progressBarMax) * progressDuration;
        int maxCoins = BalanceDataProvider.Instance.CoinsBoxMaxAmount;
        return DOTween.To(() => progressBar.fillAmount, (val) => 
               {
                   float parameter = val / progressBarMax;
                   int coinsProgress = Mathf.RoundToInt(parameter * maxCoins);

                   progressBar.fillAmount = val;
                   progressLabel.text = coinsProgress.ToString() + "/" + maxCoins.ToString();
               }, 0.0f, duration).SetEase(progressCurve).SetTarget(this).SetAutoKill(true);
    }


    Sequence BigConfetti()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => 
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

            for (int i = 0; i < bigConfetti.Length; i++)
            {
                bigConfetti[i].Play();
            }
        });
        sequence.AppendInterval(afterBigConfettiDelay);

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tweener PiggyIdleBounceAnimation(float duration)
    {
        return piggyIcon.transform.DOScale(idleBounceTarget, 0.5f * duration).SetEase(idleBounceCurve)
                                                                             .SetLoops(-1, LoopType.Yoyo)
                                                                             .SetTarget(piggyIcon)
                                                                             .SetAutoKill(true);
    }


    Tweener PiggyCoinsReactionAnimation()
    {
        DOTween.Kill(piggyIcon, true);
        return piggyIcon.transform.DOScale(piggyCoinsBounceTarget, 0.5f * piggyCoinsBounceDuration).SetEase(idleBounceCurve)
                                                                                                   .SetLoops(2, LoopType.Yoyo)
                                                                                                   .SetTarget(piggyIcon)
                                                                                                   .SetAutoKill(true);
    }

    #endregion



    #region Events handling

    void GetButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        if (BalanceDataProvider.Instance.IsCoinsBoxRewardOnlyForVideo)
        {
            CloseSelf();
        } 
        else if (Env.Instance.Inventory.TryRecieveBucksBoxValue(1, coinsRoot))
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);
            confetti.Play();

            closeButton.interactable = false;
            getButton.interactable = false;

            RewardAnimation().OnComplete(() => 
            {
                CloseSelfWithDelay(closingDelay);
            }).Play();


            var data = new Dictionary<string, object>();
            data["action"] = "get";
            BoGD.MonoBehaviourBase.Analytics.SendEvent("piggy_bank", data);
        }
    }


    void VideoButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        closeButton.interactable = false;
        getButton.interactable = false;

        Env.Instance.Sound.StopMusic();
        AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, "coins_piggy_box", RewardedVideo_OnShown);
    }


    void RewardedVideo_OnShown(AdActionResultType result)
    {
        Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);
        
        if (result == AdActionResultType.Success)
        {
            if (Env.Instance.Inventory.TryRecieveBucksBoxValue(VideoRewardMultiplier, coinsRoot))
            {
                RewardAnimation().OnComplete(() => 
                {
                    CloseSelfWithDelay(closingDelay);
                }).Play();
                var data = new Dictionary<string, object>();
                data["action"] = "get_x2";
                BoGD.MonoBehaviourBase.Analytics.SendEvent("piggy_bank", data);
            }
        }
        else
        {
            closeButton.interactable = true;
            getButton.interactable = true;

            videoButton.Reset();

            if (result == AdActionResultType.NoInternet)
            {
                Env.Instance.UI.Messages.ShowInfoBox("label_no_video".Translate(), () => { });
            }
        }
    }


    void CloseButton_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        CloseSelf();
    }

    #endregion
}
