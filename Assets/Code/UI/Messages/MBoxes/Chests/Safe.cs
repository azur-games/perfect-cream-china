using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    [SerializeField] Animator chestAnimator;

    public void Open()
    {
        chestAnimator.SetTrigger("Open");
    }
}
