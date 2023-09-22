using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class DontLooseCreamController : MonoBehaviour
{
    #region Fields

    [SerializeField] Transform dontLooseCream;
    [SerializeField] AnimationCurve dontLooseAppearCurve;
    [SerializeField] float dontLooseAppearDuration = 0.3f;
    [SerializeField] AnimationCurve dontLooseDisappearCurve;
    [SerializeField] float dontLooseDisappearDuration = 0.3f;
    [SerializeField] AnimationCurve idleAnimationFirstHalfCurve;
    [SerializeField] AnimationCurve idleAnimationSecondHalfCurve;
    [SerializeField] float idleAnimationScale = 1.05f;
    [SerializeField] float idleAnimationDelay = 0.1f;
    [SerializeField] float dontLooseIdleAnimationDuration = 0.3f;
    [SerializeField] float dontLooseMinShowTime = 1.0f;


    GameplayController controller = null;

    DateTime creamLostDate = DateTime.MinValue;
    DateTime showDate = DateTime.MinValue;

    bool isShowing = false;
    bool isHiding = false;

    #endregion



    #region Properties

    double SecondsFromLostCream
    {
        get
        {
            return (DateTime.Now - creamLostDate).TotalSeconds;
        }
    }


    double SecondsFromShow
    {
        get
        {
            return (DateTime.Now - showDate).TotalSeconds;
        }
    }

    #endregion



    #region Unity lifecycle

    void Awake()
    {
        dontLooseCream.gameObject.SetActive(false);
    }


    void OnEnable()
    {
        Env.Instance.Rooms.GameplayRoom.Controller.OnCreamLost += GameplayController_OnCreamLost;
    }


    void OnDisable()
    {
        Env.Instance.Rooms.GameplayRoom.Controller.OnCreamLost -= GameplayController_OnCreamLost;

        DOTween.Kill(dontLooseCream, false);

        dontLooseCream.gameObject.SetActive(false);

        creamLostDate = DateTime.MinValue;
        showDate = DateTime.MinValue;

        isShowing = false;
        isHiding = false;
    }


    void Update()
    {
        if (((creamLostDate != DateTime.MinValue) && 
            (showDate != DateTime.MinValue) && 
            (SecondsFromShow >= dontLooseMinShowTime) && 
            (SecondsFromLostCream >= dontLooseMinShowTime)) ||
            Env.Instance.Rooms.GameplayRoom.Controller.IsTubeInFeverZone)
        {
            HideDontLooseCream();
        }
    }

    #endregion



    #region Dynamics

    void ShowDontLooseCream()
    {
        if (!isShowing && !dontLooseCream.gameObject.activeInHierarchy)
        {
            isShowing = true;

            dontLooseCream.gameObject.SetActive(true);
            dontLooseCream.localScale = Vector3.zero;

            DOTween.Kill(dontLooseCream, true);

            dontLooseCream.DOScale(1.0f, dontLooseAppearDuration).SetEase(dontLooseAppearCurve)
                                                                 .SetTarget(dontLooseCream)
                                                                 .SetAutoKill(true)
                                                                 .OnComplete(DontLooseCream_OnShow);
        }
    }


    void RunIdleAnimation()
    {
        if (dontLooseCream.gameObject.activeInHierarchy)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(dontLooseCream.DOScale(idleAnimationScale, 0.5f * dontLooseIdleAnimationDuration).SetEase(idleAnimationFirstHalfCurve));
            sequence.Append(dontLooseCream.DOScale(1.0f, 0.5f * dontLooseIdleAnimationDuration).SetEase(idleAnimationSecondHalfCurve));

            sequence.SetDelay(idleAnimationDelay).SetTarget(dontLooseCream).SetAutoKill(true).SetLoops(-1);
        }
    }


    void HideDontLooseCream()
    {
        if (!isHiding && dontLooseCream.gameObject.activeInHierarchy)
        {
            isHiding = true;

            DOTween.Kill(dontLooseCream, true);

            dontLooseCream.DOScale(0.0f, dontLooseDisappearDuration).SetEase(dontLooseDisappearCurve)
                                                                    .SetTarget(dontLooseCream)
                                                                    .SetAutoKill(true)
                                                                    .OnComplete(DontLooseCream_OnHide);
        }
    }

    #endregion



    #region Events handling

    void GameplayController_OnCreamLost()
    {
        if (Env.Instance.Rooms.GameplayRoom.Controller.IsTubeInFeverZone) return;

        ShowDontLooseCream();

        creamLostDate = DateTime.Now;
    }


    void DontLooseCream_OnShow()
    {
        RunIdleAnimation();

        isShowing = false;

        showDate = DateTime.Now;
    }


    void DontLooseCream_OnHide()
    {
        dontLooseCream.gameObject.SetActive(false);

        isHiding = false;

        creamLostDate = DateTime.MinValue;
        showDate = DateTime.MinValue;
    }

    #endregion
}