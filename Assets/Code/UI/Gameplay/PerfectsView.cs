using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PerfectsView : MonoBehaviour
{
    #region Nested types

    enum PerfectState
    {
        None = 0,
        Showing,
        Idling,
        Hiding
    }

    #endregion



    #region Field

    [SerializeField] Image perfectImage;

    [Space]
    [Header("Perfects groups")]
    [SerializeField] List<Sprite> positivePerfects = new List<Sprite>();
    [SerializeField] List<Sprite> negativePrefects = new List<Sprite>();

    [Space]
    [Header("Appear / Disappear settings")]
    [SerializeField] AnimationCurve appearCurve;
    [SerializeField] float appearDuration = 0.3f;
    [SerializeField] AnimationCurve disappearCurve;
    [SerializeField] float disappearDuration = 0.3f;
    [SerializeField] float disappearDelay = 1.0f;


    GameplayController controller = null;
    PerfectsController.ChunkResultType currentType = PerfectsController.ChunkResultType.None;
    PerfectState currentState = PerfectState.None;
    Sequence currentSequence = null;

    #endregion



    #region Unity lifecycle

    void Awake()
    {
        SetupPerfect(PerfectsController.ChunkResultType.None);
    }


    void OnEnable()
    {
        Env.Instance.Rooms.GameplayRoom.Controller.PerfectsController.OnChunkFinishedAction += GameplayController_OnChunkFinished;
        Env.Instance.Rooms.GameplayRoom.Controller.OnCreamLost += GameplayController_OnCreamLost;
    }


    void OnDisable()
    {
        Env.Instance.Rooms.GameplayRoom.Controller.PerfectsController.OnChunkFinishedAction -= GameplayController_OnChunkFinished;
        Env.Instance.Rooms.GameplayRoom.Controller.OnCreamLost -= GameplayController_OnCreamLost;

        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;

            currentType = PerfectsController.ChunkResultType.None;
            currentState = PerfectState.None;
        }
    }

    #endregion



    #region Perfects logic

    void ShowPerfect(PerfectsController.ChunkResultType type)
    {
        if (currentType != type)
        {
            if (currentSequence != null && currentState != PerfectState.Hiding && 
                                           currentState != PerfectState.None)
            {
                currentSequence.Kill();
                currentSequence = null;

                Tween hideAnimation = HideAnimation();
                hideAnimation.OnComplete(() => 
                {
                    SetupPerfect(type);
                    PlayPerfectAnimation();
                });
                hideAnimation.Play();
            }
            else if (currentState == PerfectState.None)
            {
                SetupPerfect(type);
                PlayPerfectAnimation();
            }
        }
    }


    void SetupPerfect(PerfectsController.ChunkResultType type)
    {
        currentType = type;

        perfectImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        perfectImage.sprite = RandomPerfect(type);

        perfectImage.transform.localScale = Vector3.zero;
    }


    Sprite RandomPerfect(PerfectsController.ChunkResultType type)
    {
        List<Sprite> perfectsForType = null;

        switch(type)
        {
            case PerfectsController.ChunkResultType.Positive:
                perfectsForType = positivePerfects;
                break;

            case PerfectsController.ChunkResultType.Negative:
                perfectsForType = negativePrefects;
                break;

            default:
                break;
        }

        return (perfectsForType != null) ? perfectsForType.RandomObject() : null;
    }

    #endregion



    #region Animations

    void PlayPerfectAnimation()
    {
        currentSequence = DOTween.Sequence();

        currentSequence.Append(ShowAnimation());
        currentSequence.AppendCallback(Sequence_OnIdleStart);
        currentSequence.AppendInterval(disappearDelay);
        currentSequence.Append(HideAnimation());

        currentSequence.SetTarget(this)
                       .SetAutoKill(true)
                       .OnComplete(Sequence_OnComplete)
                       .Play();
    }


    Tween ShowAnimation()
    {
        return perfectImage.transform.DOScale(1.0f, appearDuration).SetEase(appearCurve)
                                                                   .SetAutoKill(true)
                                                                   .SetTarget(this)
                                                                   .OnStart(ShowAnimation_OnStart);
    }


    Tween HideAnimation()
    {
        return perfectImage.transform.DOScale(0.0f, disappearDuration).SetEase(disappearCurve)
                                                                      .SetAutoKill(true)
                                                                      .SetTarget(this)
                                                                      .OnStart(HideAnimation_OnStart);
    }

    #endregion



    #region Events handling

    void ShowAnimation_OnStart()
    {
        currentState = PerfectState.Showing;
    }


    void HideAnimation_OnStart()
    {
        currentState = PerfectState.Hiding;
    }


    void Sequence_OnIdleStart()
    {
        currentState = PerfectState.Idling;
    }


    void Sequence_OnComplete()
    {
        currentType = PerfectsController.ChunkResultType.None;
        currentState = PerfectState.None;
        currentSequence = null;
    }


    void GameplayController_OnChunkFinished(PerfectsController.ChunkResultType result)
    {
        ShowPerfect(result);
    }


    void GameplayController_OnCreamLost()
    {
        ShowPerfect(PerfectsController.ChunkResultType.Negative);
    }

    #endregion
}