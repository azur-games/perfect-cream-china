using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlayColorized : UIOverlayBackground
{
    protected override void OnAwake()
    {
        base.OnAwake();

    }
    protected override void OnColorChanged()
    {
        ownImage.color = CurrentColor;
    }

    protected override void UpdateSelf(float deltaTime)
    {
        base.UpdateSelf(deltaTime);
    }
}
