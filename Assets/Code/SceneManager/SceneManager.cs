using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager
{
    private System.Action onLoadingFinished = null;

    public void OnMainSceneLoaded(Starter sceneStarter)
    {
        sceneStarter.StartCoroutine(
            ProcessGarbageCollection(
                () =>
                {
                    GameObject.Destroy(sceneStarter.gameObject);
                }));
    }

    private IEnumerator ProcessGarbageCollection(System.Action onGarbageCollectionFinished)
    {
        // sync garbage collection - can be done async if needed
        AsyncOperation async = Resources.UnloadUnusedAssets();
        yield return async;

        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();

        ActionUtils.ResetAndCall(ref onLoadingFinished, null);

        onGarbageCollectionFinished?.Invoke();
    }

    public void ReLoad(System.Action onFinish)
    {
        onLoadingFinished = onFinish;
        string sceneName = Env.Instance.Config.InitialSceneName;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }
}
