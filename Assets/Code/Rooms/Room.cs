using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public List<GameObject> prefabInstances = null;

    public Room()
    {
        //Debug.Log("Room " + this.GetType().ToString() + " created [" + Config.SomeInfo + "]");
    }

    private RoomConfig config = null;
    public RoomConfig Config
    {
        get
        {
            if (null == config)
            {
                string roomName = this.GetType().ToString();
                string configPath = Env.Instance.Config.GetRoomConfigResourcePath(roomName);
                config = Resources.Load<RoomConfig>(configPath);
            }

            return config;
        }
    }

    public void CallOnEnter(RoomContext context)
    {
        OnEnter(context);
    }

    protected virtual void OnEnter(RoomContext context)
    {
        LoadScenes();

        LoadPrefabs();

        Env.Instance.Rooms.OnRoomEntered(this);
    }

    protected virtual void OnLeave()
    {
        Env.Instance.Rooms.BeforeLeaveRoom(this);
    }

    public virtual void Leave(bool destroyPrefabs, System.Action onFinish)
    {
        OnLeave();

        if (destroyPrefabs)
        {
            DestroyPrefabs();
        }

        onFinish();
    }

    private void LoadScenes()
    {
        if (null == Config.Scenes) return;

        foreach (string sceneName in Config.Scenes)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }

    private void LoadPrefabs()
    {
        if (null == Config.Prefabs) return;

        prefabInstances = new List<GameObject>();

        foreach (GameObject prefab in Config.Prefabs)
        {
            GameObject prefabInstance = GameObject.Instantiate(prefab);
            prefabInstances.Add(prefabInstance);
        }
    }

    private void DestroyPrefabs()
    {
        foreach (GameObject go in prefabInstances)
        {
            GameObject.Destroy(go);
        }

        prefabInstances.Clear();
        prefabInstances.Capacity = 0;
        prefabInstances = null;
    }
}
