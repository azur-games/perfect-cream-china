using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableBool
{
    private bool val;
    private readonly string name;

    public StorableBool(string name, bool defValue)
    {
        this.name = name;
        ReadFromPrefs(defValue);
    }

    private void ReadFromPrefs(bool defValue)
    {
        int defInt = defValue ? 1 : 0;
        int storedInt = PlayerPrefs.GetInt(name, defInt);
        val = (1 == storedInt);
    }

    private void WriteToPrefs()
    {
        PlayerPrefs.SetInt(name, val ? 1 : 0);
    }

    public bool Value
    {
        get
        {
            return val;
        }

        set
        {
            if (val == value) return;
            val = value;

            WriteToPrefs();
        }
    }
}
