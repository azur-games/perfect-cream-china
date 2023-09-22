using Modules.General;
using System;
using UnityEngine;
using TMPro;
using Spine.Unity;
using Modules.General.Abstraction.InAppPurchase;
using BoGD;


public class SubscriptionPopupCream : SubscriptionPopup
{
    #region Fields

    [Space]
    [Header("Custom settings")]
    [SerializeField] 
    private TextMeshProUGUI rewardLabel;

    [Space]
    [Header("Spine animations")]
    [SerializeField] 
    private SkeletonGraphic itemAnimation;
    [SerializeField] 
    private string          itemAnimationName;
    [SerializeField] 
    private bool            isItemAnimationLooped;

    private string          placement;

    private const float LineSpaceForTurkishLanguage = -30f;
    #endregion



    #region Override

    protected override void UpdateButtonState(SubscriptionButtonSettings buttonSettings)
    {
        if (buttonSettings != null)
        {
            if (buttonSettings.priceLabel != null)
            {
                var price = buttonSettings.storeItem.LocalizedPrice.IsNullOrEmpty() ?
                    "$"+ buttonSettings.storeItem.TierPrice.ToString("F2") :
                    buttonSettings.storeItem.LocalizedPrice;

                buttonSettings.priceLabel.text = string.Format("label_price_sub".Translate(), price);
            }

            if (buttonSettings.trialPriceLabel != null)
            {
                var price =
                    (string.IsNullOrEmpty(buttonSettings.storeItem.LocalizedPrice)) ?
                        "$" + buttonSettings.storeItem.TierPrice.ToString("F2") :
                        (buttonSettings.storeItem.LocalizedPrice);
                buttonSettings.trialPriceLabel.text = string.Format("label_price_trial_sub".Translate(), price);
            }

            TryUpdateDescriptionState(buttonSettings);
        }
    }

    protected override void TryUpdateDescriptionState(SubscriptionButtonSettings settings)
    {
        if (settings.subscriptionType == SubscriptionType.Weekly)
        {
            var price = settings.storeItem.LocalizedPrice.IsNullOrEmpty() ?
                    "$"+ settings.storeItem.TierPrice.ToString("F2") :
                    settings.storeItem.LocalizedPrice;
            var translate = "label_description_sub".Translate();

            var label = string.Format(translate, price, AccountType, StoreType);
            descriptionLabel.text = label;
        }
    }

    public override void Show(bool isStartSubscription, Action<SubscriptionPopupResult> callback, string placement = null)
    {
        base.Show(isStartSubscription, callback, placement);
        this.placement = placement;
        rewardLabel.text = string.Format("label_every_day".Translate(), BalanceDataProvider.Instance.SubscriptionCoinsReward);
        var reason = isStartSubscription ? "first_launch" : placement.IsNullOrEmpty()? "click_vip_item" : placement;
        Env.Instance.SendPopup("subscription", reason , "show");
        if (isStartSubscription)
        {
            Env.SendTechnical(5, "show_start_subscription");
        }

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Turkish:
                descriptionLabel.lineSpacing = LineSpaceForTurkishLanguage;
                break;
        }
    }

    public override void RestoreButton_OnClick()
    {
        if (CheckIfPopupNeeded(out string message))
        {
            PopupManager.Instance.ShowMessagePopup(message: message, messageHandler: transform);
            return;
        }

        PurchaseProcess(true);
        storeManager.RestorePurchases(result =>
        {
            PurchaseProcess(false);

            PopupManager.Instance.ShowMessagePopup((callback) =>
            {
                if (result.IsSucceeded && storeManager.HasAnyActiveSubscription)
                {
                    Hide(SubscriptionPopupResult.SubscriptionRestored);
                }
            }, ((result.IsSucceeded) ? ("label_purchases_restored".Translate()) : ("label_cannot_connect").Translate()), messageHandler: transform);
        });
    }

    public override void SubscriptionHiden()
    {
        base.SubscriptionHiden();
        var reason = isStartSubscription ? "first_launch" : placement.IsNullOrEmpty()? "click_vip_item" : placement;
        Env.Instance.SendPopup("subscription", reason, "close");

        if (isStartSubscription)
        {
            Env.SendTechnical(6, "hide_start_subscription");
        }
    }


    protected override bool CheckIfPopupNeeded(out string message)
    {
        message = null;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return true;
        }
        if (!Services.StoreManager.IsInitialized)
        {
            message = "label_store_is_not_initialized".Translate();
            return true;
        }
        return false;
    }

    public override void SubscriptionButton_OnClick(IStoreItem storeItem)
    {
        base.SubscriptionButton_OnClick(storeItem);
        var reason = isStartSubscription ? "first_launch" : placement.IsNullOrEmpty()? "click_vip_item" : placement;
        Env.Instance.SendPopup("subscription", reason, "click_buy");
    }

    #endregion



    #region Animations events handling

    public override void SubscriptionShown()
    {
        base.SubscriptionShown();

        if (itemAnimation != null)
        {
            itemAnimation.AnimationState.SetAnimation(0, itemAnimationName, isItemAnimationLooped);
        }
    }

    #endregion
}