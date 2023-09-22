using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentItemIconRef : ScriptableObject
{
    public const string FILE_NAME_POSTFIX = "_iconRef";

    #region Fields

    public Sprite Icon;
    public Sprite AlternativeIcon;
    public Sprite AlternativeIcon2;

    #endregion
}
