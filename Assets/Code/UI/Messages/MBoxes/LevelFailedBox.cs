using Modules.Advertising;
using Modules.Analytics;
using Modules.General.Abstraction;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BoGD;

[System.Serializable]
public class LevelFailedBox : UIMessageBox
{
    #region Fields

    [Space]
    [Header("Main elements")]
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] Image counterImage;
    [SerializeField] Button continueButton;
    [SerializeField] Button noThanksButton;
    [SerializeField] CanvasGroup noThanksGroup;

    [Space]
    [Header("Counter settings")]
    [SerializeField] List<Sprite> counterSprites = new List<Sprite>();
    [SerializeField] AnimationCurve counterBounceCurve;
    [SerializeField] float counterBounceDuration = 0.75f;

    [Space]
    [Header("Dynamics settings")]
    [SerializeField] float noThanksDelay = 1.0f;
    [SerializeField] float noThanksFadeDuration = 0.3f;


    Action onContinue = null;
    Action onAbort = null;
    float currentTimer = 0.0f; 
    int currentCounter = -1;
    bool isTimerEnabled = true;
    private Color completionFontColor = Color.white;

    #endregion



    #region Unity lifecycle

    protected override void Awake()
    {
        base.Awake();
        UpdateFontColors(completionFontColor);

        continueButton.onClick.AddListener(() =>
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            isTimerEnabled = false;
            continueButton.interactable = false;

            AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, 
                "fail_continue",
                AdvertisingManager_OnAdShowed);
        });

        noThanksButton.onClick.AddListener(() => 
        {
            Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

            AbortGame();
        });
    }


    void Update()
    {
        if (!isTimerEnabled)
            return;

        if (currentTimer > 0.0f)
        {
            currentTimer -= Time.deltaTime;

            UpdateCounter(currentTimer);

            if (currentTimer <= 0.0f)
            {
                AbortGame();
            }
        }
    }


    private void OnDestroy()
    {
        DOTween.Kill(this);

        //Scheduler.Instance.UnscheduleMethod(this, NoThanksButtonAppearance);
    }

    #endregion


    

    #region Initialization

    public void Init(int score, int bestScore, Action onContinue = null, Action onAbort = null)
    {
        this.onContinue = onContinue;
        this.onAbort = onAbort;

        currentTimer = counterSprites.Count;

        InitNoThanksButtonAppearance();
    }


    public override void Close()
    {
        base.Close();

        Env.Instance.SendResultWindow("lose", "close", 0, 0);
    }

    #endregion



    #region Timer & Counter handling

    void UpdateCounter(float time)
    {
        int currentSeconds = (int)Mathf.Clamp(time, 0.0f, counterSprites.Count - 1);

        if (currentCounter != currentSeconds)
        {
            Sprite sprite = counterSprites[currentSeconds];

            counterImage.sprite = sprite;

            DOTween.Kill(counterImage.transform, false);
            counterImage.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
            counterImage.transform.DOScaleY(1.0f, counterBounceDuration).SetEase(counterBounceCurve).SetAutoKill(true);

            currentCounter = currentSeconds;
        }
    }

    #endregion



    #region Continue & Abort the game

    void ContinueGame()
    {
        onContinue?.Invoke();
        Close();

        Env.continues++;
    }


    void AbortGame()
    {
        onAbort?.Invoke();
        Close();
    }

    #endregion



    #region Fonts handling

    private void UpdateFontColors(Color color)
    {
        foreach (var t in texts)
            t.color = color;
    }

    #endregion



    #region Elements dynamics

    void InitNoThanksButtonAppearance()
    {
        noThanksButton.gameObject.SetActive(false);
        noThanksGroup.alpha = 0.0f;
        //Scheduler.Instance.CallMethodWithDelay(this, NoThanksButtonAppearance, noThanksDelay);
    }


    void NoThanksButtonAppearance()
    {
        noThanksButton.gameObject.SetActive(true);
        noThanksGroup.DOFade(1.0f, noThanksFadeDuration).SetTarget(this).SetAutoKill(true);
    }

    #endregion



    #region Events handling

    void AdvertisingManager_OnAdShowed(AdActionResultType result)
    {
        switch (result)
        {
            case AdActionResultType.Success:
                
                ContinueGame();
                break;

            case AdActionResultType.NoInternet:
                Env.Instance.UI.Messages.ShowInfoBox("label_no_video".Translate(), () =>
                {
                    AbortGame();
                });
                break;

            default:
                AbortGame();
                break;
        }
    }

    #endregion
}
