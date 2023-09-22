using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableString
{
    private string val;
    private readonly string name;

    public StorableString(string name, string defValue)
    {
        this.name = name;
        ReadFromPrefs(defValue);
    }

    private void ReadFromPrefs(string defValue)
    {
        val = PlayerPrefs.GetString(name, defValue);
    }

    private void WriteToPrefs()
    {
        PlayerPrefs.SetString(name, val);
    }

    public string Value
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
