using UnityEngine;


public class AnimationBool : MonoBehaviour
{
    #region Fields

    [SerializeField] private string triggerName;
    [SerializeField] public bool TrueOnStart;

    Animator animator;

    #endregion



    #region Unity lifecycle

    private bool cachedValue = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (TrueOnStart)
        {
            Set(true);
        }

        cachedValue = animator.GetBool(triggerName);
    }

    public void Set(bool value)
    {
        if (animator == null) return;

        if (cachedValue == value) return;
        cachedValue = value;

        animator.SetBool(triggerName, value);
    }

    #endregion
}
