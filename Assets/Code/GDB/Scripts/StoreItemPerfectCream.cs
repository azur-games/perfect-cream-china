using Modules.General.Abstraction.InAppPurchase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.InAppPurchase
{
    using BoGD;
    public class StoreItemPerfectCream : StoreItem
    {
        
        public StoreItemPerfectCream(StoreItemSettings settings, IStoreManager manager) : base(settings, manager)
        {
        }

        public override void Purchase(Func<IPurchaseItemResult, bool> callback = null)
        {
            if (BoGD.CheatController.Instance.CheatBuid && BoGD.CheatController.Instance.HasCheat(BoGD.CONSOLE.Cheat.ForcePuchase))
            {
                string message = "";
                callback?.Invoke(new PurchaseItemResult(this, PurchaseItemResultCode.Ok, message));
                return;
            }
            base.Purchase(callback);
        }

        public override void InvokePurchaseRestored(IPurchaseItemResult purchaseItemResult, ISubscriptionInfo info)
        {
            base.InvokePurchaseRestored(purchaseItemResult, info);

            if (purchaseItemResult.ResultCode == PurchaseItemResultCode.Ok)
            {
                Env.Payer = true;
            }
        }

        public override bool InvokePurchaseComplete(IPurchaseItemResult purchaseItemResult, ISubscriptionInfo info)
        {

            if (purchaseItemResult.ResultCode == PurchaseItemResultCode.Ok)
            {
                if (purchaseItemResult.TransactionState != PurchaseTransactionState.Restored)
                {
                    BoGD.InAppItem item = new BoGD.InAppItem();
                    item.ID = Identifier;
                    item.ISO = CurrencyCode;
                    item.LocalizedPrice = (decimal)Price;
                    item.Type = Identifier.Contains("noads") ? "noads" : "subscription";

                    BoGD.MonoBehaviourBase.Analytics.SendPurchase(item);
                    CustomDebug.Log("payment_succeed".Color(Color.green) );
                }
                Env.Payer = true;

                

            }

            return base.InvokePurchaseComplete(purchaseItemResult, info);
        }
    }
}
