using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPlaceholder : MonoBehaviour
{
    [SerializeField] private string subcategory;

    public string Subcategory
    {
        get
        {
            return subcategory;
        }
    }
}
