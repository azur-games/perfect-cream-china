#define SRDEBUGGER_ENABLED

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SrDebuggerHelper
{
    [System.Flags]
    public enum OptionType
    {
        Prefs = 1,
        Common = 2,
        Metagame = 4,
        Gameplay = 8,
    }

    #region options
    public class Prefs
    {
        [System.ComponentModel.Category("Prefs options")]
        public void ClearPlayerPrefs()
        {
            Debug.Log("Clearing PlayerPrefs");
            PlayerPrefs.DeleteAll();
        }

        [System.ComponentModel.Category("Prefs options")]
        public void SavePlayerPrefs()
        {
            Debug.Log("Saving PlayerPrefs");
            PlayerPrefs.Save();
        }
    }

    private static List<byte[]> memoryLeaksTestReservedMemory = new List<byte[]>(1000);

    public class Common
    {
        [System.ComponentModel.Category("Common options")]
        public void ReloadInitialScene()
        {
            Env.Instance.Rooms.SwitchToInitialRoom();
        }

        [System.ComponentModel.Category("Common options")]
        public void Restart()
        {
            Env.Instance.FullRestart();
        }

        [System.ComponentModel.Category("Reserve 10M")]
        public void Reserve10M()
        {
            byte[] bytes = new byte[10 * 1024 * 1024];
            memoryLeaksTestReservedMemory.Add(bytes);
            Debug.Log("RESERVED: " + (10 * memoryLeaksTestReservedMemory.Count).ToString() + " MB");
        }
    }

    public class Metagame
    {
        [System.ComponentModel.Category("Metagame options")]
        public void GoToGameplay()
        {
            Env.Instance.Rooms.SwitchToRoom<GameplayRoom>(true, null, () => { });
        }

        [System.ComponentModel.Category("Metagame options")]
        public void SomeFuncrtion()
        {
            Debug.Log("Do somethins...");
        }
    }

    public class Gameplay
    {
        [System.ComponentModel.Category("Gameplay options")]
        public void GoMetagame()
        {
            Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(true, null, () => { });
        }
    }

    #endregion

    public bool DebuggingEnabled { get; private set; }

    // Map<Owner, List<Option>>
    private Dictionary<object, List<object>> registeredOptions = new Dictionary<object, List<object>>();

    public SrDebuggerHelper(bool enabled)
    {
        DebuggingEnabled = enabled;

        if (DebuggingEnabled)
        {
#if SRDEBUGGER_ENABLED
            SRDebug.Init();
#endif
        }
    }

    public void Register(object owner, OptionType optionTypeFlags)
    {
        if (!DebuggingEnabled) return;

        if (!registeredOptions.TryGetValue(owner, out List<object> optionObjectsList))
        {
            optionObjectsList = new List<object>();
            registeredOptions.Add(owner, optionObjectsList);
        }

        foreach (OptionType optionType in System.Enum.GetValues(typeof(OptionType)))
        {
            if (!optionTypeFlags.HasFlag(optionType)) continue;

            string optionClassName = this.GetType().ToString() + "+" + optionType.ToString();
            System.Type tp = System.Type.GetType(optionClassName);
            object optionObject = System.Activator.CreateInstance(tp);

            optionObjectsList.Add(optionObject);
#if SRDEBUGGER_ENABLED
            SRDebug.Instance.AddOptionContainer(optionObject);
#endif
        }
    }

    public void UnregisterAll()
    {
        if (!DebuggingEnabled) return;

        foreach (object owner in registeredOptions.Keys)
        {
            Unregister(owner);
        }

        registeredOptions.Clear();
    }

    public void Unregister(object owner)
    {
        if (!DebuggingEnabled) return;

        if (!registeredOptions.TryGetValue(owner, out List<object> optionObjectsList)) return;

        foreach (object optionObject in optionObjectsList)
        {
#if SRDEBUGGER_ENABLED
            SRDebug.Instance.RemoveOptionContainer(optionObject);
#endif
        }

        registeredOptions[owner].Clear();
    }
}
