using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager
{
    public Dictionary<Type, Room> LoadedRooms { get; } = new Dictionary<Type, Room>();
    public Room CurrentRoom { get; private set; } = null;

    public RoomManager()
    {
    }

    private Action onSwitchingFinished = null;
    private RoomContext roomContext = null;

    public GameplayRoom GameplayRoom => (CurrentRoom as GameplayRoom);
    public MetagameRoom MetagameRoom => (CurrentRoom as MetagameRoom);

    public bool GetCurrentRoom<T>(out T room) where T : Room
    {
        room = CurrentRoom as T;
        return (room != null);
    }

    public void GetCurrentRoom<T>(Action<T> ifCurrent) where T : Room
    {
        if (CurrentRoom is T room)
        {
            ifCurrent(room);
        }
    }

    public void SwitchToInitialRoom()
    {
        SwitchToRoom(Env.Instance.Config.InitialRoomName, false, null, () => { });
    }

    public bool SwitchToRoom<T>(bool reloadScene, RoomContext roomContext, Action onFinish) where T : Room
    {
        return SwitchToRoom(typeof(T).ToString(), reloadScene, roomContext, onFinish);
    }

    public bool SwitchToRoom(string roomName, bool reloadScene, RoomContext roomContext, Action onFinish)
    {        
        if (null != onSwitchingFinished)
        {
            return false;
        }

        this.roomContext = roomContext;

        onSwitchingFinished = () =>
        {
            onSwitchingFinished = null;
            onFinish?.Invoke();
        };

        if (null == CurrentRoom)
        {
            FinishSwitching(roomName);
        }
        else
        {
            CurrentRoom.Leave(!reloadScene, () =>
            {
                if (reloadScene)
                {
                    Env.Instance.SceneManager.ReLoad(() =>
                    {
                        FinishSwitching(roomName);
                    });
                }
                else
                {
                    FinishSwitching(roomName);
                }
            });
        }

        return true;
    }

    private void FinishSwitching(string toRoomName)
    {
        CurrentRoom = GetOrCreateRoom(toRoomName);
        CurrentRoom.CallOnEnter(roomContext);

        ActionUtils.ResetAndCall(ref onSwitchingFinished, null);
    }

    private Room GetOrCreateRoom(string roomName)
    {
        Type roomType = Type.GetType(roomName);
        if (LoadedRooms.TryGetValue(roomType, out Room room))
        {
            return room;
        }

        Room newRoom = (Room)Activator.CreateInstance(roomType);
        LoadedRooms.Add(roomType, newRoom);
        
        return newRoom;
    }

    private T GetOrCreateRoom<T>() where T : Room
    {
        T room = GetRoom<T>();

        if (null == room)
        {
            room = (T)Activator.CreateInstance(typeof(T));
            LoadedRooms.Add(typeof(T), room);
        }

        return room;
    }

    private T GetRoom<T>() where T : Room
    {
        if (LoadedRooms.TryGetValue(typeof(T), out Room room))
        {
            return (T)room;
        }

        return null;
    }

    public void OnRoomEntered(Room room)
    {
        // TODO : change to call event or send message
        //Env.Instance.Debugger.Register(room, room.Config.SrDebugerOptions);
    }

    public void BeforeLeaveRoom(Room room)
    {
        // TODO : change to call event or send message
        //Env.Instance.Debugger.Unregister(room);
    }
}
