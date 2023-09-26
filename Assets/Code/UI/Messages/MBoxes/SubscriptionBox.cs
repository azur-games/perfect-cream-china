using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using System;
using System.Collections.Generic;
using UnityEngine;
using BoGD;


public class SubscriptionBox : UIMessageBox
{
    #region Nested types

    public enum SubscriptionBoxType
    {
        None = 0,
        Subscription, 
        ExtendedSubscription,
        SuccessfulSubscription,
        Reward,
        Message
    }
    
    #endregion



    #region Fields

    [SerializeField] Transform contentRoot;

    SubscriptionBoxType type = SubscriptionBoxType.None;
    Action<bool> onCloseSelf = null;

    #endregion


    protected virtual string SubscriptionRewardText => string.Format("label_every_day".Translate(), BalanceDataProvider.Instance.SubscriptionCoinsReward);
    #region Initialization

    public void Init(SubscriptionBoxType type, Action<bool> onClose = null, string message = null, bool isStartSubscription = false, string placement = "")
    {
        this.type = type;
        this.onCloseSelf = onClose;

        AdvertisingManager.Instance.TryHideAdByModule(AdModule.Banner);
        AdvertisingManager.Instance.LockAd(AdModule.Interstitial, "Subscription");
        if (placement.IsNullOrEmpty())
        {
            placement = "default";
        }

        switch (type)
        {
            case SubscriptionBoxType.Subscription:
                var data = new Dictionary<string, object>();
                data["subscription_id"] = "vip";
                data["placement"] = placement;
                data["action"] = "show";

                PopupManager.Instance.ShowSubscription((result) =>
                {
                    if (result == SubscriptionPopupResult.SubscriptionPurchased ||
                        result == SubscriptionPopupResult.SubscriptionRestored)
                    {
                        CloseSelf(true);

                        if (result == SubscriptionPopupResult.SubscriptionPurchased)
                        {
                            var data = new Dictionary<string, object>();
                            data["subscription_id"] = "vip";
                            data["placement"] = placement;
                            data["action"] = "buy";
                        }
                    }
                    else
                    {
                        CloseSelf(false);

                        var data = new Dictionary<string, object>();
                        data["subscription_id"] = "vip";
                        data["placement"] = placement;
                        data["action"] = "close";
                    }
                }, isStartSubscription, contentRoot, placement);
                break;

            case SubscriptionBoxType.ExtendedSubscription:
                
                PopupManager.Instance.ShowExtendedSubscription((result) => 
                {
                    if (result == SubscriptionPopupResult.SubscriptionPurchased ||
                        result == SubscriptionPopupResult.SubscriptionRestored)
                    {
                        CloseSelf(true);
                    }
                    else
                    {
                        CloseSelf(false);
                    }
                }, isStartSubscription, contentRoot);
                break;

            case SubscriptionBoxType.Reward:
                break;

            case SubscriptionBoxType.SuccessfulSubscription:
                string description = SubscriptionRewardText;
                CustomDebug.Log("description = ".Color(Color.green) + description);
                PopupManager.Instance.ShowSuccessfulSubscriptionPopup(description, (result) =>
                {
                    if (result)
                    {
                        ApplyPremiumItems();
                        CloseSelf(true);
                    }
                    else
                    {
                        CloseSelf(false);
                    }
                }, contentRoot);
                break;

            case SubscriptionBoxType.Message:
                PopupManager.Instance.ShowMessagePopup((result) =>
                {
                    CloseSelf(result);
                }, message, contentRoot);
                break;

            default:
                CloseSelf(false);
                break;
        }
    }

    #endregion



    #region Premium handling

    private void ApplyPremiumItems()
    {

        ContentItemInfo premiumCream = Env.Instance.Content.GetAvailableInfos(ContentAsset.AssetType.CreamSkin, PremiumFilter).Last();
        ContentItemInfo premiumValve = Env.Instance.Content.GetAvailableInfos(ContentAsset.AssetType.Valve, PremiumFilter).Last();
        
        if (premiumCream != null)
        {
            if (Env.Instance.Rooms.GameplayRoom != null)
            {
                Env.Instance.Rooms.GameplayRoom.Controller.GetComponentInChildren<CreamCreator>(true).CreamSkinName = premiumCream.Name;
            }
            else
            {
                Env.Instance.Rules.CreamSkin.Value = premiumCream.Name;
            }
        }

        if (premiumValve != null)
        {
            if (Env.Instance.Rooms.GameplayRoom != null)
            {
                ValveAsset valve = Env.Instance.Content.LoadContentAsset<ValveAsset>(ContentAsset.AssetType.Valve, premiumValve.Name);
                Env.Instance.Rooms.GameplayRoom.Controller.Valve.UpdateSkin(valve);
            }

            Env.Instance.Inventory.SetCurrentValve(premiumValve.Name);
        }

        Env.Instance.Inventory.Save();
    }


    private bool PremiumFilter(ContentItemInfo itemInfo)
    {
        return Env.Instance.Inventory.IsPremiumItem(itemInfo);
    }

    #endregion



    #region Close self

    private void CloseSelf(bool success)
    {
        AdvertisingManager.Instance.UnlockAd(AdModule.Interstitial, "Subscription");

        onCloseSelf?.Invoke(success);
        onCloseSelf = null;

        Close();
    }

    #endregion
}
