using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BoGD;

[System.Serializable]
public class InfoBox : UIMessageBox
{
    #region Fields

    [SerializeField] Text text;

    #endregion


    public void Init(string description)
    {
        text.text = description.Translate();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Close();
        }
    }
}
