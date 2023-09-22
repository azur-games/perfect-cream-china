using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContentSource", menuName = "Configs/ContentSource")]
public class ContentSource : ScriptableObject
{
    [SerializeField] private string targetDirectory = null;

    public string TargetDirectory
    {
        get
        {
            return targetDirectory;
        }

        set
        {
            targetDirectory = value;
        }
    }
}
