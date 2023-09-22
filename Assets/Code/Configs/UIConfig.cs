using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UIConfig")]
public class UIConfig : ScriptableObject
{
    #region Fields
    [SerializeField] Color startupOverlayColor = Color.black;

    [SerializeField] Color overlayColor = Color.black;

    [SerializeField] float overlayCameraDepth = 1000.0f;

    [SerializeField] float messagesCameraDepth = 100.0f;

    [SerializeField] float messagesPopUpCameraDepth = 150.0f;

    [SerializeField] float overlayChangingSpeed = 1.0f;

    [SerializeField] UIOverlayBackground overlayBackground = null;

    [SerializeField] List<UIMessageBox> messageBoxes = null;

    [SerializeField] AnimationCurve ChestAnimatedScale = null;

    #endregion



    #region Properties
    public Color StartupOverlayColor { get { return startupOverlayColor; } }

    public Color OverlayColor { get { return overlayColor; } }

    public float OverlayCameraDepth { get { return overlayCameraDepth; } }

    public float MessagesCameraDepth { get { return messagesCameraDepth; } }

    public float MessagesPopUpCameraDepth { get { return messagesPopUpCameraDepth; } }

    public float OverlayChangingSpeed { get { return overlayChangingSpeed; } }

    public UIOverlayBackground OverlayBackground { get { return overlayBackground; } }

    public List<UIMessageBox> MessageBoxes { get { return messageBoxes; } }

    public AnimationCurve GetChestAnimatedScale { get { return ChestAnimatedScale; } }

    #endregion
}
