using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessageBox : MonoBehaviour
{
    [SerializeField] protected Camera ownCamera;

    public bool IsPopup = false;

    protected virtual void Awake()
    {
        visible = true;
        Visible = false;

        ownCamera.depth = IsPopup ? Env.Instance.UI.Config.MessagesPopUpCameraDepth : Env.Instance.UI.Config.MessagesCameraDepth;
    }

    private bool visible;
    public bool Visible
    {
        get { return visible; }
        set
        {
            if (visible.Equals(value))
            {
                return;
            }

            visible = value;

            this.gameObject.SetActive(visible);
        }
    }

    private Action onCloseDelegate = () => { };

    public void SetOnCloseDelegate(Action onCloseDelegate)
    {
        this.onCloseDelegate = onCloseDelegate;
    }

    public void AddOnCloseDelegate(Action onCloseDelegate)
    {
        this.onCloseDelegate += onCloseDelegate;
    }

    public virtual void Close()
    {
        onCloseDelegate();

        ForcedCloseWithoutCallbacks();
    }

    public void ForcedCloseWithoutCallbacks()
    {
        GameObject.Destroy(this.gameObject);
    }
}
