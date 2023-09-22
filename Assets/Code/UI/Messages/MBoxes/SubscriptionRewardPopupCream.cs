using Modules.General;
using System;
using UnityEngine;
using BoGD;


public class SubscriptionRewardPopupCream : RewardPopup
{

    [SerializeField]
    private bool isFullLabelReward = true;

    public override void Show(string rewardAmount, Action<bool> callback)
    {
        base.Show(rewardAmount, callback);
        string reward;
        if(isFullLabelReward)
		{
            reward = string.Format("label_every_day".Translate(), BalanceDataProvider.Instance.SubscriptionCoinsReward);
        }
        else
		{
            reward = BalanceDataProvider.Instance.SubscriptionCoinsReward.ToString();
        }
        rewardDescriptionLabel.text = reward;
        Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);
    }

    public override void PopupHiden()
    {
        base.PopupHiden();
    }
}
