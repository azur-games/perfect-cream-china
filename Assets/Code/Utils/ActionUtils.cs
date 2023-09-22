using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionUtils
{
    public static void ResetAndCall(ref System.Action action, System.Action resetedAction)
    {
        System.Action actionCached = action;
        action = resetedAction;
        actionCached?.Invoke();
    }

    public static void ResetAndCall<T>(ref System.Action<T> action, T value, System.Action<T> resetedAction)
    {
        System.Action<T> actionCached = action;
        action = resetedAction;
        actionCached?.Invoke(value);
    }

    public static void ResetAndCall<T, K>(ref System.Action<T, K> action, T value1, K value2, System.Action<T, K> resetedAction)
    {
        System.Action<T, K> actionCached = action;
        action = resetedAction;
        actionCached?.Invoke(value1, value2);
    }
}
