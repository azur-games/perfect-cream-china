using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public UIConfig Config { get; private set; }

    public UIOverlay Overlay { get; private set; }
    public UIMessages Messages { get; private set; }

    public UIManager()
    {
        string configPath = Env.Instance.Config.GetUIConfigResourcePath();
        Config = Resources.Load<UIConfig>(configPath);

        Messages = new UIMessages();
        Overlay = new UIOverlay();
    }

    public void CloseAll()
    {
        Overlay.CloseAll();
        Messages.CloseAll();
    }
}
