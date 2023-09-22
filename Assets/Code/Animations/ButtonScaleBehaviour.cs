using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Button))]
public sealed class ButtonScaleBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Nested types

    enum ButtonScaleState
    {
        None = 0,
        AnimatingPointerDown,
        AnimatingPointerUp,
        AnimatingIdle
    }

    #endregion



    #region Fields

    [SerializeField] bool disableIfNotInteractable = true;

    [Space]
    [Header("Tap settings")]
    [SerializeField] ScaleData pointerDownScaleData;
    [SerializeField] ScaleData pointerUpScaleData;

    [Space]
    [Header("Idle settings")]
    [SerializeField] ScaleData scaleUpIdleData;
    [SerializeField] ScaleData scaleDownIdleData;
    [SerializeField] bool enableIdleAnimation = false;


    Button button = null;
    ButtonScaleState currentState = ButtonScaleState.None;
    bool lastInteractableCheck = false;
    bool isIdleBlocked = false;
    bool isEnabled = true;

    #endregion



    #region Properties

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                UpdateIdleAnimation();
            }
        }
    }


    public bool IsIdleBlocked
    {
        get => isIdleBlocked;
        set
        {
            if (isIdleBlocked != value)
            {
                isIdleBlocked = value;
                UpdateIdleAnimation();
            }
        }
    }


    bool InteractionActive => (button != null && IsEnabled && (button.interactable || !disableIfNotInteractable));

    #endregion



    #region Unity lifecycle

    void Awake()
    {
        button = gameObject.GetComponent<Button>();
    }


    void OnEnable()
    {
        UpdateIdleAnimation();
    }


    void Update()
    {
        if (button != null && lastInteractableCheck != button.interactable)
        {
            UpdateIdleAnimation();
        }
    }


    void OnDisable()
    {
        DOTween.Complete(button);
        DOTween.Kill(button);

        currentState = ButtonScaleState.None;
    }


    void OnDestroy()
    {
        DOTween.Complete(button);
        DOTween.Kill(button);
    }

    #endregion



    #region IPointerDownHandler

    public void OnPointerDown(PointerEventData eventData)
    {
        DOTween.Complete(button);
        DOTween.Kill(button);
        currentState = ButtonScaleState.None;

        ScaleData data = pointerDownScaleData;
        if (data != null && InteractionActive)
        {
            currentState = ButtonScaleState.AnimatingPointerDown;

            transform.DOScale(data.scale, data.duration).SetEase(data.ease)
                                                        .SetTarget(button)
                                                        .SetAutoKill(true)
                                                        .OnComplete(() => 
                                                        { 
                                                            currentState = ButtonScaleState.None; 
                                                        });
        }
    }

    #endregion



    #region IPointerUpHandler

    public void OnPointerUp(PointerEventData eventData)
    {
        DOTween.Complete(button);
        DOTween.Kill(button);
        currentState = ButtonScaleState.None;

        ScaleData data = pointerUpScaleData;
        if (data != null && InteractionActive)
        {
            currentState = ButtonScaleState.AnimatingPointerUp;

            transform.DOScale(data.scale, data.duration).SetEase(data.ease)
                                                        .SetTarget(button)
                                                        .SetAutoKill(true)
                                                        .OnComplete(() => 
                                                        { 
                                                            currentState = ButtonScaleState.None;

                                                            UpdateIdleAnimation(); 
                                                        });
        }
    }

    #endregion



    #region IPointerExitHandler

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Complete(button);
        DOTween.Kill(button);
        currentState = ButtonScaleState.None;

        ScaleData data = pointerUpScaleData;
        if (data != null && InteractionActive)
        {
            currentState = ButtonScaleState.AnimatingPointerUp;
            
            transform.DOScale(data.scale, data.duration).SetEase(data.ease)
                                                        .SetTarget(button)
                                                        .SetAutoKill(true)
                                                        .OnComplete(() => 
                                                        { 
                                                            currentState = ButtonScaleState.None;

                                                            UpdateIdleAnimation(); 
                                                        });
        }
    }

    #endregion



    #region Update animation

    void UpdateIdleAnimation()
    {
        lastInteractableCheck = (button != null) ? button.interactable : lastInteractableCheck;

        if (InteractionActive && enableIdleAnimation && !IsIdleBlocked &&
            currentState != ButtonScaleState.AnimatingPointerDown && currentState != ButtonScaleState.AnimatingPointerUp)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(button.transform.DOScale(scaleUpIdleData.scale, scaleUpIdleData.duration).SetEase(scaleUpIdleData.ease)
                                                                                                     .SetTarget(button));


            sequence.Append(button.transform.DOScale(scaleDownIdleData.scale, scaleDownIdleData.duration).SetEase(scaleDownIdleData.ease)
                                                                                                         .SetTarget(button));

            sequence.SetTarget(button);
            sequence.SetLoops(-1, LoopType.Restart);

            currentState = ButtonScaleState.AnimatingIdle;
        }
        else if (button != null && (!IsEnabled || disableIfNotInteractable || IsIdleBlocked) && 
                 currentState != ButtonScaleState.AnimatingPointerDown && currentState != ButtonScaleState.AnimatingPointerUp)
        {
            DOTween.Complete(button);
            DOTween.Kill(button);

            currentState = ButtonScaleState.None;
        }
    }

    #endregion
}