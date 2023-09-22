using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameplayButton : MonoBehaviour
{
    private const string PRESSED_ANIMATION_NAME = "Pressed";
    private const string JUMP_ANIMATION_NAME = "Jump";

    [SerializeField] private float jumpPeriod = 3.0f;

    public CustomButton ButtonStartCommonLevel;
    private Animator animatorCommon;
    private bool commonButtonPressed = false;

    public CustomButton ButtonStartExtraLevel;
    private Animator animatorExtra;
    private bool extraButtonPressed = false;

    private int currentButton = -1;

    private System.DateTime lastJumpTime;
    private System.Action onClick;

    public bool JumpsEnable { get; set; } = true;

    private void Awake()
    {
        animatorCommon = ButtonStartCommonLevel.GetComponent<Animator>();
        animatorExtra = ButtonStartExtraLevel.GetComponent<Animator>();

        ButtonStartCommonLevel.PointerDown += OnButtonStartCommonLevelPressed;
        ButtonStartCommonLevel.PointerUp += OnButtonStartCommonLevelReleased;

        ButtonStartExtraLevel.PointerDown += OnButtonStartExtraLevelPressed;
        ButtonStartExtraLevel.PointerUp += OnButtonStartExtraLevelReleased;

        ButtonStartCommonLevel.onClick.AddListener(OnSomeButtonClick);
        ButtonStartExtraLevel.onClick.AddListener(OnSomeButtonClick);

        lastJumpTime = System.DateTime.Now;
    }

    private void OnSomeButtonClick()
    {
        onClick?.Invoke();
    }

    private void OnButtonStartCommonLevelPressed()
    {
        commonButtonPressed = true;
        animatorCommon.SetBool(PRESSED_ANIMATION_NAME, commonButtonPressed);
    }

    private void OnButtonStartCommonLevelReleased()
    {
        commonButtonPressed = false;
        animatorCommon.SetBool(PRESSED_ANIMATION_NAME, commonButtonPressed);
    }

    private void OnButtonStartExtraLevelPressed()
    {
        extraButtonPressed = true;
        animatorExtra.SetBool(PRESSED_ANIMATION_NAME, extraButtonPressed);
    }

    private void OnButtonStartExtraLevelReleased()
    {
        extraButtonPressed = false;
        animatorExtra.SetBool(PRESSED_ANIMATION_NAME, extraButtonPressed);
    }

    private void SetButtons(int but)
    {
        if (currentButton == but) return;
        currentButton = but;

        ButtonStartCommonLevel.gameObject.SetActive(currentButton == 0);
        ButtonStartExtraLevel.gameObject.SetActive(currentButton == 1);
    }

    public void Init(bool needCommonButton, System.Action onClick)
    {
        SetButtons(needCommonButton ? 0 : 1);
        this.onClick = onClick;
    }

    private bool buttonCommonJumpCached = false;
    private bool buttonExtraJumpCached = false;

    private void Update()
    {
        System.DateTime now = System.DateTime.Now;

        bool needToJump = false;
        if ((now - lastJumpTime).TotalSeconds > jumpPeriod)
        {
            lastJumpTime = now;
            needToJump = true;
        }

        bool buttonCommonJump = false;
        bool buttonExtraJump = false;
        if (needToJump && JumpsEnable)
        {
            if (ButtonStartCommonLevel.gameObject.activeSelf && !commonButtonPressed)
            {
                buttonCommonJump = true;
            }

            if (ButtonStartExtraLevel.gameObject.activeSelf && !extraButtonPressed)
            {
                buttonExtraJump = true;
            }
        }

        if (buttonCommonJumpCached != buttonCommonJump)
        {
            buttonCommonJumpCached = buttonCommonJump;
            animatorCommon.SetBool(JUMP_ANIMATION_NAME, buttonCommonJump);
        }

        if (buttonExtraJumpCached != buttonExtraJump)
        {
            buttonExtraJumpCached = buttonExtraJump;
            animatorExtra.SetBool(JUMP_ANIMATION_NAME, buttonExtraJump);
        }
    }
}
