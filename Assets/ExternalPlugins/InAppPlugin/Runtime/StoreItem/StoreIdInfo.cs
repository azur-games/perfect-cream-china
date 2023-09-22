using System;
using UnityEngine.Purchasing;


namespace Modules.InAppPurchase
{
    [Serializable]
    public class StoreIdInfo
    {
        public AppStore storeType = AppStore.NotSpecified;
        public string storeId = "";
        public string parentId = "";
        public bool isFullId = false;
    }
}
