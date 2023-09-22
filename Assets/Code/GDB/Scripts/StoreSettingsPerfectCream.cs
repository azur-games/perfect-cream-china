using Modules.General.Abstraction.InAppPurchase;
using Modules.General.InitializationQueue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.InAppPurchase
{
    [CreateAssetMenu(fileName = "StoreSettingsPerfectCream")]
    [InitQueueService(
       -5010,
       typeof(IStoreSettings))]
    public class StoreSettingsPerfectCream : StoreSettings
    {
        public override List<IStoreItem> GetStoreItems(IStoreManager storeManager)
        {
            if (storeItems == null)
            {
                storeItems = new List<IStoreItem>(storeItemsSettings.Count);
                foreach (StoreItemSettings settings in storeItemsSettings)
                {
                    storeItems.Add(new StoreItemPerfectCream(settings, storeManager));
                }
            }

            return storeItems;
        }
    }
}