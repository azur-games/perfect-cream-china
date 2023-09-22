using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBarController : MonoBehaviour
{
    #region Fields

    [SerializeField] Image icon;
    [SerializeField] Image fill;

    #endregion

    public void InitProgressBar(Sprite levelIcon, Color progressFillingColor)
    {
        icon.sprite = levelIcon;
        fill.color = progressFillingColor;
    }
}