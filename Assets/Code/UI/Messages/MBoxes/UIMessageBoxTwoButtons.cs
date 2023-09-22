using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIMessageBoxTwoButtons : UIMessageBox
{
    [SerializeField] UnityEngine.UI.Text caption = null;
    [SerializeField] UnityEngine.UI.Text text = null;

    [SerializeField] UnityEngine.UI.Button button1 = null;
    [SerializeField] UnityEngine.UI.Text button1Caption = null;

    [SerializeField] UnityEngine.UI.Button button2 = null;
    [SerializeField] UnityEngine.UI.Text button2Caption = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(string caption, string text, string button1Text, Action onButton1ClickAction, string button2Text, Action onButton2ClickAction)
    {
        this.caption.text = caption;
        this.text.text = text;

        button1Caption.text = string.IsNullOrEmpty(button1Text) ? "" : button1Text;
        if (null != onButton1ClickAction)
            button1.onClick.AddListener(
                () => 
                {
                    Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

                    Close(); 
                    onButton1ClickAction(); 
                });
        else
            button1.gameObject.SetActive(false);

        button2Caption.text = string.IsNullOrEmpty(button2Text) ? "" : button2Text;
        if (null != onButton2ClickAction)
            button2.onClick.AddListener(
                () => 
                {
                    Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

                    Close(); 
                    onButton2ClickAction(); 
                });
        else
            button2.gameObject.SetActive(false);
    }
}
