using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibbonController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool animationStarted = false;

    public void StartRibbonAnimation()
    {
        if (animationStarted) return;
        animator.SetTrigger("Go");
        animationStarted = true;
    }
}
