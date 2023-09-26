using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.InAppPurchase;
using System;
using Modules.Hive.Ioc;


public class AdvertisingNecessaryInfo : IAdvertisingNecessaryInfo
{
    #region Fields

    public event Action<int> OnPlayerLevelUpdate;
    public event Action OnPersonalDataDeletingDetect;
    public event Action OnPurchasesListUpdate;

    IStoreManager storeManager;

    #endregion



    #region Properties

    public bool IsSubscriptionActive => true;

    public bool IsNoAdsActive => false;

    public bool IsPersonalDataDeleted => false;

    public int CurrentPlayerLevel => Env.Instance.Inventory.CurrentLevelIndex + 1;

    #endregion



    #region Class lifecycle

    public void InitListeners()
    {
        EventDispatcher.Subscribe<PrivacyPersonalDataDeletingDetected>(callback=> LLPrivacyManager_OnPersonalDataDeletingDetect());

        storeManager = Services.GetService<IStoreManager>();
        storeManager.RestorePurchasesComplete += StoreManager_OnRestorePurchasesComplete;
        storeManager.PurchaseComplete += StoreManager_OnPurchaseComplete;
        
        Env.Instance.Inventory.OnLevelChanged += OnLevelNumberUpdate;
    }

    #endregion



    #region Events handlers

    private void LLPrivacyManager_OnPersonalDataDeletingDetect()
    {
        OnPersonalDataDeletingDetect?.Invoke();
    }


    private bool StoreManager_OnPurchaseComplete(IPurchaseItemResult purchaseItemResult)
    {
        OnPurchasesListUpdate?.Invoke();
        return false;
    }


    private void StoreManager_OnRestorePurchasesComplete(IRestorePurchasesResult restorePurchasesResult)
    {
        OnPurchasesListUpdate?.Invoke();
    }


    private void OnLevelNumberUpdate()
    {
        OnPlayerLevelUpdate?.Invoke(CurrentPlayerLevel);
    }

    #endregion
}