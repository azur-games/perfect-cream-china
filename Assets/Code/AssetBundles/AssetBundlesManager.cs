using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundlesManager
{
    public AssetBundlesManager()
    {
        Init();
    }

    private void Init()
    {

    }

    public void CheckForUpdates(System.Action onFinish)
    {
        onFinish?.Invoke();
    }

    public void LoadUpdatesFromServer(System.Action onFinish)
    {
        onFinish?.Invoke();
    }
}
