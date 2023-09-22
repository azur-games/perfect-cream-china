using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableInt
{
    private int val;
    private readonly string name;

    private readonly bool rangeSetted;
    private readonly int minValue;
    private readonly int maxValue;

    public StorableInt(string name, int defValue)
    {
        this.name = name;

        minValue = 0;
        maxValue = 0;
        rangeSetted = false;

        ReadFromPrefs(defValue);
    }

    public StorableInt(string name, int defValue, int minValue, int maxValue)
    {
        this.name = name;

        this.minValue = minValue;
        this.maxValue = maxValue;
        rangeSetted = true;

        ReadFromPrefs(defValue);
    }

    private void ReadFromPrefs(int defValue)
    {
        val = PlayerPrefs.GetInt(name, defValue);
    }

    private void WriteToPrefs()
    {
        PlayerPrefs.SetInt(name, val);
    }

    public int Value
    {
        get
        {
            return val;
        }

        set
        {
            if (rangeSetted)
            {
                value = Mathf.Clamp(value, minValue, maxValue);
            }

            if (val == value) return;
            val = value;

            WriteToPrefs();
        }
    }
}
