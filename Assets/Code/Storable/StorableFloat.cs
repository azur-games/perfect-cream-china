using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorableFloat
{
    private float val;
    private readonly string name;

    private readonly bool rangeSetted;
    private readonly float minValue;
    private readonly float maxValue;

    public StorableFloat(string name, float defValue)
    {
        this.name = name;

        minValue = 0.0f;
        maxValue = 0.0f;
        rangeSetted = false;

        ReadFromPrefs(defValue);
    }

    public StorableFloat(string name, float defValue, float minValue, float maxValue)
    {
        this.name = name;

        this.minValue = minValue;
        this.maxValue = maxValue;
        rangeSetted = true;

        ReadFromPrefs(defValue);
    }

    private void ReadFromPrefs(float defValue)
    {
        val = PlayerPrefs.GetFloat(name, defValue);
    }

    private void WriteToPrefs()
    {
        PlayerPrefs.SetFloat(name, val);
    }

    public float Value
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
