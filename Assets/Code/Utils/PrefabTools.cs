using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabTools
{
    public static T Instantiate<T>(Transform parent, T original, Vector3? position = null) where T : MonoBehaviour
    {
        T newT = GameObject.Instantiate<T>(original);

        newT.transform.parent = parent;
        newT.transform.localRotation = Quaternion.identity;
        newT.transform.localPosition = position ?? Vector3.zero;
        
        return newT;
    }
}
