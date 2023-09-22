using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewItemsDetectionHelper
{
    private const string PREFS_KEY = "NewItemsDetection";
    private const char PREFS_KEY_SEPARATOR = '\n';

    private HashSet<string> knownValues = new HashSet<string>();

    public NewItemsDetectionHelper()
    {
        ReadKnownKeys();
    }

    public bool IsHasUnknowns(List<string> values)
    {
        return IsHasUnknowns(values.ToArray());
    }

    public bool IsHasUnknowns(params string[] values)
    {
        foreach (string value in values)
        {
            if (!knownValues.Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    private void ReadKnownKeys()
    {
        if (!PlayerPrefs.HasKey(PREFS_KEY)) return;

        string sourceStr = PlayerPrefs.GetString(PREFS_KEY);
        string[] values = sourceStr.Split(PREFS_KEY_SEPARATOR);
        foreach (string value in values)
        {
            if (string.IsNullOrEmpty(value)) continue;
            knownValues.Add(value);
        }
    }

    public bool AddKnown(List<string> values)
    {
        return AddKnown(values.ToArray());
    }

    public bool AddKnown(params string[] values)
    {
        bool findNew = false;

        foreach (string value in values)
        {
            findNew |= knownValues.Add(value);
        }

        return findNew;
    }

    public void SaveKnownKeys()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (string str in knownValues)
        {
            sb.Append(str + PREFS_KEY_SEPARATOR);
        }

        PlayerPrefs.SetString(PREFS_KEY, sb.ToString());
        PlayerPrefs.Save();
    }
}
