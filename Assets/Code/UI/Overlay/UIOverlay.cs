using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay
{
    public class OverlayInstance
    {
        public object Owner { get; private set; }
        public Color? Color { get; private set; }
        public bool Immediately { get; private set; }

        private bool alreadyReached = false;
        private readonly System.Action<OverlayInstance> onReached;
        private UIOverlay overlay;

        public OverlayInstance(UIOverlay overlay, object owner, Color? color, bool immediately, System.Action<OverlayInstance> onReached)
        {
            Owner = owner;
            Color = color;
            Immediately = immediately;
            this.overlay = overlay;
            this.onReached = onReached;
        }

        public void CallOnReachedAction()
        {
            if (alreadyReached) return;
            alreadyReached = true;
            onReached?.Invoke(this);
        }

        public void Close()
        {
            overlay.Remove(this);
        }
    }

    private UIOverlayBackground activeBackground = null;
    private List<OverlayInstance> overlays = new List<OverlayInstance>();

    public UIOverlay()
    {
        
    }

    public OverlayInstance CurrentActiveOverlay
    {
        get
        {
            return (0 == overlays.Count) ? null : overlays[overlays.Count - 1];
        }
    }

    public OverlayInstance SetImmediately(object owner, Color? color, System.Action<OverlayInstance> onReached)
    {
        return Set(owner, color, true, onReached);
    }

    public OverlayInstance Set(object owner, Color? color, System.Action<OverlayInstance> onReached)
    {
        return Set(owner, color, false, onReached);
    }

    public OverlayInstance Set(object owner, Color? color, bool immediately, System.Action<OverlayInstance> onReached)
    {
        CheckBackgroundObject();

        OverlayInstance newTarget = new OverlayInstance(this, owner, color, immediately, onReached);
        overlays.Add(newTarget);

        UpdateOverlaysStack();

        return newTarget;
    }

    private void CheckBackgroundObject()
    {
        if (null != activeBackground) return;

        activeBackground = PrefabTools.Instantiate<UIOverlayBackground>(null, Env.Instance.UI.Config.OverlayBackground);
    }

    private void DestroyBackgroundObject()
    {
        if (null == activeBackground) return;

        GameObject.Destroy(activeBackground.gameObject);
        activeBackground = null;
    }

    public void Remove(OverlayInstance target)
    {
        bool needUpdateStack = CurrentActiveOverlay == target;

        if (overlays.Remove(target))
        {
            if (needUpdateStack)
            {
                UpdateOverlaysStack();
            }
        }
    }

    public void Remove(object owner, bool all = true)
    {
        if (null == owner) return;

        OverlayInstance activeOverlay = CurrentActiveOverlay;

        if (all)
        {
            int deletedItemsCount = overlays.RemoveAll(t => t.Owner == owner);

            if ((deletedItemsCount > 0) && (activeOverlay != CurrentActiveOverlay))
            {
                UpdateOverlaysStack();
            }
        }
        else
        {
            OverlayInstance target = overlays.FindLast(t => t.Owner == owner);
            if (null != target)
            {
                overlays.Remove(target);

                if (activeOverlay != CurrentActiveOverlay)
                {
                    UpdateOverlaysStack();
                }
            }
        }
    }

    private void OnTargetReached()
    {
        if (0 < overlays.Count)
        {
            overlays[overlays.Count - 1].CallOnReachedAction();
        }
        else
        {
            DestroyBackgroundObject();
            return;
        }
    }

    private void UpdateOverlaysStack()
    {
        bool isBufferEmpty = (0 == overlays.Count);

        OverlayInstance currentTarget = isBufferEmpty ?
            new OverlayInstance(this, null, null, false, (tg) => { }) : 
            overlays[overlays.Count - 1];

        Color targetColor = currentTarget.Color ?? new Color(
                activeBackground.CurrentColor.r,
                activeBackground.CurrentColor.g,
                activeBackground.CurrentColor.b,
                0.0f);

        activeBackground.SetTarget(
            targetColor,
            currentTarget.Color.HasValue,
            currentTarget.Immediately,
            () =>
            {
                OnTargetReached();
            });
    }

    public void CloseAll()
    {
        List<OverlayInstance> overlaysBuffer = new List<OverlayInstance>(overlays);

        foreach (OverlayInstance overlayInstance in overlaysBuffer)
        {
            overlayInstance.Close();
        }

        DestroyBackgroundObject();
    }
}
