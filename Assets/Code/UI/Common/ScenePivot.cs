using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePivot : MonoBehaviour
{
    public enum Subject
    {
        None = 0,
        MetaPersBubble,
        MetaPiggyBubble
    }

    public Subject PivotSubject;

    public static ScenePivot Find(GameObject parent, Subject subject)
    {
        ScenePivot[] pivots = parent.GetComponentsInChildren<ScenePivot>();
        foreach (ScenePivot pivot in pivots)
        {
            if (pivot.PivotSubject == subject)
            {
                return pivot;
            }
        }

        return null;
    }

    public static Vector3? GetPivotViewPoint(GameObject parent, Subject subject)
    {
        ScenePivot pivot = Find(parent, subject);

        if (null == pivot)
        {
            return null;
        }

        Vector3 worldPosition = pivot.transform.position;

        if (null != Env.Instance.Rooms.MetagameRoom)
        {
            return Env.Instance.Rooms.MetagameRoom.Controller.Camera.WorldToViewportPoint(worldPosition);
        }

        if (null != Env.Instance.Rooms.GameplayRoom)
        {
            return Env.Instance.Rooms.GameplayRoom.Controller.Camera.WorldToViewportPoint(worldPosition);
        }

        return null;
    }
}
