using UnityEngine;
using Spine;
using Spine.Unity;


public class SubscriptionPopupCreamExtended : SubscriptionPopupCream
{
    #region Fields

    [SerializeField] SkeletonGraphic firstScreenItemAnimation;
    [SerializeField] string firstScreenItemAnimationName;
    [SerializeField] bool isFirstScreenItemAnimationLooped;
    [SerializeField] SkeletonGraphic secondScreenItemAnimation;
    [SerializeField] string secondScreenItemAnimationName;
    [SerializeField] string secondScreenItemIdleAnimationName;
    [SerializeField] bool isSecondScreenItemAnimationLooped;
    [SerializeField] SkeletonGraphic thirdScreenItemAnimation;
    [SerializeField] string thirdScreenItemAnimationName;
    [SerializeField] bool isThirdScreenItemAnimationLooped;

    #endregion


    #region Animation events handling

    protected virtual void FirstScreenItemShown()
    {
        if (firstScreenItemAnimation != null)
        {
            firstScreenItemAnimation.AnimationState.SetAnimation(0, firstScreenItemAnimationName, isFirstScreenItemAnimationLooped);
        }
    }


    protected virtual void SecondScreenItemShown()
    {
        if (secondScreenItemAnimation != null)
        {
            TrackEntry entry = secondScreenItemAnimation.AnimationState.SetAnimation(0, secondScreenItemAnimationName, isSecondScreenItemAnimationLooped);
            entry.Complete += (trackEntry) => 
            {
                secondScreenItemAnimation.AnimationState.SetAnimation(0, secondScreenItemIdleAnimationName, true);
            };
        }
    }


    protected virtual void ThirdScreenItemShown()
    {
        if (thirdScreenItemAnimation != null)
        {
            thirdScreenItemAnimation.AnimationState.SetAnimation(0, thirdScreenItemAnimationName, isThirdScreenItemAnimationLooped);
        }
    }

    #endregion

}