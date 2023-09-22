using UnityEngine;


public class AnimationTrigger : MonoBehaviour
{
    #region Fields

    [SerializeField] private string triggerName;

    Animator animator;

    #endregion



    #region Unity lifecycle

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger(triggerName);
    }


    void OnEnable()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    #endregion
}
