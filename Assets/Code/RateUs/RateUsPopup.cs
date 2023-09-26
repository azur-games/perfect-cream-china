using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RateUsPopup : MonoBehaviour
{
    #region Nested types

    [Serializable]
    struct StarButton
    {
        public Button button;
        public Animator bounceAnimator;
        public Image fillStar;
    }


    [Serializable]
    struct StateButton
    {
        public Button activeButton;
        public GameObject inactiveButton;

        public void SetActive(bool isActive)
        {
            activeButton.gameObject.SetActive(isActive);
            inactiveButton.SetActive(!isActive);
        }
    }

    #endregion



    #region Fields
    
    private const int           CountRateStarsToFeedback = 5;

    [SerializeField] 
    private Button              closePopupButton = default;


    [Header("Rate")]

    [SerializeField] 
    private StateButton         rateStateButton = default;
    [SerializeField] 
    private List<StarButton>    starButtons = default;
    
    private int                 currentRateStars;
    private string              previousText;
    event Action                onClosePopup;

    #endregion



    #region Unity lifecycle

    protected virtual void OnEnable()
    {
        for (int i = 0; i < starButtons.Count; i++)
        {
            int indexStarButton = i;
            starButtons[i].button.onClick.AddListener(() => StarButton_OnClick(indexStarButton));
        }

        closePopupButton.onClick.AddListener(ClosePopupButton_OnClick);
        rateStateButton.activeButton.onClick.AddListener(RateButton_OnClick);
    }


    protected virtual void OnDisable()
    {
        for (int i = 0; i < starButtons.Count; i++)
        {
            starButtons[i].button.onClick.RemoveAllListeners();
        }
        
        closePopupButton.onClick.RemoveListener(ClosePopupButton_OnClick);
        rateStateButton.activeButton.onClick.RemoveListener(RateButton_OnClick);
    }


    protected void Start()
    {
        currentRateStars = 0;
        SetCountStars(currentRateStars - 1);
        rateStateButton.SetActive(currentRateStars != 0);
    }

    #endregion



    #region Methods

    public static void Show(Transform transform, Action onClose)
    {
        #if UNITY_IOS
            RateUsService.Instance.TryToShowNativeRatePopupIOS();
            onClose?.Invoke();
        #elif UNITY_ANDROID || UNITY_EDITOR
            RateUsPopup instance = Instantiate(Resources.Load<RateUsPopup>("Prefabs/" + typeof(RateUsPopup).ToString()), transform);
            instance.onClosePopup = onClose;    
        #endif
    }
    
    
    public virtual void ClosePopup()
    {
        Destroy(gameObject);
        onClosePopup?.Invoke();
    }


    private void SetCountStars(int indexStarButton)
    {
        for (int i = starButtons.Count - 1; i >= 0; i--)
        {
            starButtons[i].fillStar.gameObject.SetActive(i <= indexStarButton);
        }
    }


    private void DisableStarBouncing()
    {
        for (int i = 0; i < starButtons.Count; i++)
        {
            starButtons[i].bounceAnimator.enabled = false;
        }
    }

    #endregion



    #region Events handlers

    private void StarButton_OnClick(int indexButtonSender)
    {
        if (currentRateStars == 0)
        {
            DisableStarBouncing();
        }

        SetCountStars(indexButtonSender);
        currentRateStars = indexButtonSender + 1;
        rateStateButton.SetActive(currentRateStars != 0);
    }


    private void ClosePopupButton_OnClick()
    {
        ClosePopup();
        SendRate(0);
    }


    private void RateButton_OnClick()
    {
        Debug.Log("currentRateStars = " + currentRateStars + "CountRateStarsToFeedback = " + CountRateStarsToFeedback);
        if (currentRateStars >= CountRateStarsToFeedback)
        {
            Debug.Log("chek true");
#if UNITY_ANDROID
            RateUsService.Instance.RateApp();
            #endif
        }

        ClosePopup();
        SendRate(currentRateStars);
    }

    private void SendRate(int rate)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["show_reason"] = "new_player";
        data["rate_result"] = rate;
    }

    #endregion
}