using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;
using Modules.General;
using Modules.General.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Object = UnityEngine.Object;


namespace Modules.HmsPlugin.InAppPurchase
{
    /// <summary>
    /// Переработанный ExternalDependencies/hms-unity-plugin/Huawei/Scripts/IAP/UnityPurchase/HuaweiStore.cs
    /// </summary>
    internal class HuaweiStore : IStore
    {
        #region Fields

        private const int PRODUCTS_CAPACITY = 100;
        
        private const int PURCHASE_DATA_CAPACITY = 50;
        
        private static HuaweiStore instance;
        
        private IStoreCallback storeEvents;
        
        private object locker;
        
        private List<ProductInfo>               productsList;
        
        private Dictionary<string, ProductInfo> productsByID;
        
        private Dictionary<string, InAppPurchaseData> purchasedData;
        
        private IIapClient iapClient;
        
        private bool clientInited;
        
        private ReadOnlyCollection<ProductDefinition> initProductDefinitions;

        private ProductDefinition purchasingProduct;

        #endregion
        


        #region Properties

        public static HuaweiStore Instance => instance ?? (instance = new HuaweiStore());

        #endregion



        #region IStore

        void IStore.Initialize(IStoreCallback callback)
        {
            storeEvents = callback;
            BaseInit();
            CreateClient();
        }


        void IStore.RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
            CustomDebug.Log($"[HuaweiStore - RetrieveProducts] products count : {products.Count}");
            lock(locker)
            {
                initProductDefinitions = products;
                if (clientInited)
                {
                    LoadProducts();
                }
            }
        }
        
        
        void IStore.Purchase(ProductDefinition product, string developerPayload)
        {
            CustomDebug.Log($"[HuaweiStore - Purchase] product : {product.storeSpecificId}, developerPayload : {developerPayload}");
            if (!productsByID.ContainsKey(product.storeSpecificId))
            {
                CustomDebug.Log("[HuaweiStore - Purchase] product is not in productsByID");
                storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, PurchaseFailureReason.ProductUnavailable, "UnknownProduct"));
                return;
            }

            var productInfo = productsByID[product.storeSpecificId];
            PurchaseIntentReq purchaseIntentReq = new PurchaseIntentReq
            {
                PriceType        = productInfo.PriceType,
                ProductId        = productInfo.ProductId,
                DeveloperPayload = developerPayload
            };

            OnPurchaseStart(product);
            
            var task = iapClient.CreatePurchaseIntent(purchaseIntentReq);
            task.AddOnSuccessListener(intentResult =>
            {
                OnCreatePurchaseIntentSuccess(intentResult, product);
            });
            task.AddOnFailureListener(exception =>
            {
                OnPurchaseFailed(product.storeSpecificId, PurchaseFailureReason.Unknown, exception.GetDescription());
            });
        }


        void IStore.FinishTransaction(ProductDefinition product, string transactionId)
        {
            CustomDebug.Log($"[HuaweiStore - FinishTransaction] product : {product.id}, transaction : {transactionId}");
            if (purchasedData.TryGetValue(product.storeSpecificId, out var data))
            {
                var request = new ConsumeOwnedPurchaseReq();
                request.PurchaseToken = data.PurchaseToken;
                var task = iapClient.ConsumeOwnedPurchase(request);
                task.AddOnSuccessListener(result =>
                {
                    CustomDebug.Log($"[HuaweiStore - FinishTransaction] ConsumeOwnedPurchase - success : {result.ConsumePurchaseData}");
                    purchasedData.Remove(product.storeSpecificId);
                });
                task.AddOnFailureListener(exception =>
                {
                    CustomDebug.Log($"[HuaweiStore - FinishTransaction] ConsumeOwnedPurchase - failed : {exception.GetDescription()}");
                });
            }
        }
        
        #endregion
        
        

        #region Initialization

        private void BaseInit()
        {
            locker        = new object();
            productsList  = new List<ProductInfo>(PRODUCTS_CAPACITY);
            productsByID  = new Dictionary<string, ProductInfo>(PRODUCTS_CAPACITY);
            purchasedData = new Dictionary<string, InAppPurchaseData>(PURCHASE_DATA_CAPACITY);
        }

        
        private void CreateClient()
        {
            CustomDebug.Log("[HuaweiStore - CreateClient]");
            iapClient     = Iap.GetIapClient();
            var envReadyTask = iapClient.EnvReady;
            envReadyTask.AddOnSuccessListener(OnEnvReadySuccess);
            envReadyTask.AddOnFailureListener(OnEnvReadyFailure);

            if (Debug.isDebugBuild)
            {
                CustomDebug.Log("[HuaweiStore - SandboxActivated]");
                var sanboxTask = iapClient.SandboxActivated;
                sanboxTask.AddOnSuccessListener(result =>
                {
                    CustomDebug.Log(
                        $"[HuaweiStore - SandboxActivated] Success. Market version : {result.VersionFrMarket}," +
                        $"Apk version : {result.VersionInApk}, sanbox user : {result.SandboxUser}, sanbox apk : " +
                        $"{result.SandboxApk}");
                });
                sanboxTask.AddOnFailureListener(exception =>
                {
                    CustomDebug.Log($"[HuaweiStore - SandboxActivated] Error - {exception.GetDescription()}");
                });
            }
        }
        
        
        private void OnEnvReadySuccess(EnvReadyResult result)
        {   
            CustomDebug.Log("[HuaweiStore - OnEnvReadySuccess]");
            lock (locker)
            {
                clientInited = true;
                if (initProductDefinitions != null)
                {
                    LoadProducts();
                }
            }
        }
        
        
        private void LoadProducts()
        {
            LoadInitProducts(() => LoadOwnedProducts(OnAllProductsLoaded));
        }
        
        
        private void OnEnvReadyFailure(HMSException exception)
        {
            CustomDebug.Log($"[HuaweiStore - OnEnvReadyFailure] {exception.GetDescription()}");
            switch (exception.GetErrorCode())
            {
                case HMSResultCode.ORDER_HWID_NOT_LOGIN:
                    if (HMSAccountManager.Instance == null)
                    {
                        (new GameObject("HuaweiServices")).AddComponent<HMSAccountManager>();
                    }
                    HMSAccountManager.Instance.OnSignInSuccess += HMSAccountManager_OnSignInSuccess;
                    HMSAccountManager.Instance.OnSignInFailed += HMSAccountManager_OnSignInFailed;
                    HMSAccountManager.Instance.SignIn();
                    break;
                default:
                    storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);  
                    break;
            }
        }



        #endregion



        #region Init products

        private void LoadInitProducts(Action callback = null)
        {
            LoadInitProductsOfType(ProductType.Consumable, () =>
            {
                LoadInitProductsOfType(ProductType.NonConsumable, () =>
                {
                    LoadInitProductsOfType(ProductType.Subscription, () =>
                    {
                        callback?.Invoke();
                    });
                });
            });
        }
        
        
        private void LoadInitProductsOfType(ProductType productType, Action callback)
        {
            List<string> productsIds = (from definition in initProductDefinitions where definition.type == productType select definition.storeSpecificId).ToList();
            if (productsIds == null || productsIds.Count == 0)
            {
                callback.Invoke();
                return;
            }
            var productsDataRequest = new ProductInfoReq
            {
                PriceType = productType.ToHmsPriceType(),
                ProductIds = productsIds
            };
            var task = iapClient.ObtainProductInfo(productsDataRequest);
            task.AddOnSuccessListener(result =>
            {
                ParseInitProducts(result); 
                callback.Invoke();
            });
            task.AddOnFailureListener(OnObtainProductInfoFailure);
        }


        private void OnObtainProductInfoFailure(HMSException exception)
        {   
            CustomDebug.Log("[HuaweiStore - OnObtainProductInfoFailure] " + exception.GetDescription());
            storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);
        }

        
        private void ParseInitProducts(ProductInfoResult result)
        {
            if (result == null || result.ProductInfoList.Count == 0)
            {
                CustomDebug.Log("[HuaweiStore - ParseInitProducts] no products to parse");
                return;
            }
            foreach (ProductInfo productInfo in result.ProductInfoList)
            {   
                productsList.Add(productInfo);
                productsByID.Add(productInfo.ProductId, productInfo);
            }
        }

        #endregion

        
        
        #region Owned products

        private void LoadOwnedProducts(Action callback = null)
        {
            LoadOwnedProductsOfType(ProductType.Consumable, () =>
            {
                LoadOwnedProductsOfType(ProductType.NonConsumable, () =>
                {
                    LoadOwnedProductsOfType(ProductType.Subscription, () =>
                    {
                        callback?.Invoke();
                    });
                });
            });
        }
        
        
        private void LoadOwnedProductsOfType(ProductType type, Action onSuccess)
        {
            var ownedPurchasesReq = new OwnedPurchasesReq()
            {
                PriceType = type.ToHmsPriceType(),
            };
            
            var task = iapClient.ObtainOwnedPurchases(ownedPurchasesReq);

            task.AddOnSuccessListener(result =>
            {
                ParseOwnedProducts(result, type); 
                onSuccess();
            });
            task.AddOnFailureListener(error =>
            {
                CustomDebug.Log($"[HuaweiStore - LoadOwnedProductsOfType] ObtainOwnedPurchases {type} - failure {error.GetDescription()}");
            });
        }

        
        private void ParseOwnedProducts(OwnedPurchasesResult result, ProductType type)
        {
            if (result?.InAppPurchaseDataList == null)
            {
                CustomDebug.Log($"[HuaweiStore - ParseOwnedProducts] no result or no data in result for {type}");
                return;
            }
            
            foreach (var inAppPurchaseData in result.InAppPurchaseDataList)
            {
                purchasedData[inAppPurchaseData.ProductId] = inAppPurchaseData;
            }
        }

        #endregion
        


        #region All products loaded

        private void OnAllProductsLoaded()
        {
            storeEvents.OnProductsRetrieved(productsList.Select(GetProductDescription).ToList());
        }


        private ProductDescription GetProductDescription(ProductInfo product)
        {
            float price = product.MicrosPrice * 0.000001f;
            string priceString = price < 100 ? price.ToString("0.00") : ((int) (price + 0.5f)).ToString();
            priceString =  $"{priceString} {product.Currency}";
                
            ProductMetadata prodMeta = new ProductMetadata(priceString, product.ProductName, product.ProductDesc,
                product.Currency, (decimal)price);
            
            if (purchasedData.TryGetValue(product.ProductId, out var purchaseData))
            {
                if (!CheckIfIsExpiredSubscription(purchaseData))
                {
                    return new ProductDescription(product.ProductId, prodMeta, CreateReceipt(purchaseData),
                        purchaseData.OrderID);
                }
            }

            return new ProductDescription(product.ProductId, prodMeta);
        }


        private bool CheckIfIsExpiredSubscription(InAppPurchaseData purchaseData)
        {
            bool isSubscription = Enum.IsDefined(typeof(ProductType), purchaseData.Kind) 
                                  && (ProductType)purchaseData.Kind == ProductType.Subscription;
            if (isSubscription && purchaseData.AutoRenewing)
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime expireTime = epoch.AddMilliseconds(purchaseData.ExpirationDate);
                DateTime currentTime = DateTime.UtcNow;
                bool isExpired = 0L < expireTime.Ticks && expireTime.Ticks < currentTime.Ticks;
                return isExpired;
            }
            return false;
        }
        
        
        private string CreateReceipt(InAppPurchaseData purchaseData)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("purchaseTime", purchaseData.PurchaseTime);
            payload.Add("isAutoRenewing", purchaseData.AutoRenewing);
            payload.Add("expirationDate", purchaseData.ExpirationDate);
            payload.Add("trialFlag", purchaseData.TrialFlag);
            payload.Add("purchaseToken", purchaseData.PurchaseToken);
            payload.Add("subscriptionId", purchaseData.SubscriptionId);
            payload.Add("purchaseType", purchaseData.PurchaseType);
            payload.Add("cancellationTime", purchaseData.CancellationTime);
            payload.Add("introductoryFlag", purchaseData.IntroductoryFlag);

            string receiptString = JsonConvert.SerializeObject(payload);
            CustomDebug.Log("[HuaweiStore - CreateReceipt] receipt : " + receiptString);
            return receiptString;
        }

        #endregion



        #region Purchase

        private void OnCreatePurchaseIntentSuccess(PurchaseIntentResult intentResult, ProductDefinition product)
        {
            CustomDebug.Log($"[HuaweiStore - OnCreatePurchaseIntentSuccess] {product.storeSpecificId}, {intentResult == null}");
            if (intentResult == null)
            {   
                OnPurchaseFailed(product.storeSpecificId, PurchaseFailureReason.Unknown, "IntentIsNull");
                return;
            }

            var status = intentResult.Status;
            status.StartResolutionForResult(androidIntent =>
            {
                PurchaseResultInfo resultInfo = iapClient.ParsePurchaseResultInfoFromIntent(androidIntent);

                if (resultInfo.ReturnCode == OrderStatusCode.ORDER_STATE_SUCCESS)
                {
                    purchasedData[product.storeSpecificId] = resultInfo.InAppPurchaseData;
                    var receipt = CreateReceipt(resultInfo.InAppPurchaseData);
                    storeEvents.OnPurchaseSucceeded(product.storeSpecificId, receipt, resultInfo.InAppPurchaseData.OrderID );
                }
                else
                {
                    storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(product.storeSpecificId, resultInfo.GetFailureReason(), resultInfo.ErrMsg ));  
                }
                OnPurchaseEnd();
            }, exception =>
            {
                OnPurchaseFailed(product.storeSpecificId, PurchaseFailureReason.Unknown, exception.GetDescription());
            });
        }


        private void OnPurchaseFailed(string productId, PurchaseFailureReason reason, string message)
        {
            storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(productId, reason, message));
            OnPurchaseEnd();
        }


        private void OnPurchaseStart(ProductDefinition product)
        {
            purchasingProduct = product;
            ApplicationFocusObserver.Instance.StartObserve(OnPurchaseCancel);
        }


        private void OnPurchaseEnd()
        {
            purchasingProduct = null;
            ApplicationFocusObserver.Instance.StopObserve();
        }


        private void OnPurchaseCancel()
        {
            if (purchasingProduct != null)
            {
                storeEvents.OnPurchaseFailed(new PurchaseFailureDescription(purchasingProduct.storeSpecificId, PurchaseFailureReason.UserCancelled, "Billing dialog closed"));
                OnPurchaseEnd();
            }
        }

        #endregion
        
        
        
        #region Event handlers

        private void HMSAccountManager_OnSignInSuccess(AuthAccount account)
        {
            CustomDebug.Log("[HuaweiStore - HMSAccountManager_OnSignInSuccess]");
            HMSAccountManager.Instance.OnSignInSuccess -= HMSAccountManager_OnSignInSuccess;
            CreateClient();
        }
        
        
        private void HMSAccountManager_OnSignInFailed(HMSException exception)
        {
            CustomDebug.Log($"[HuaweiStore - HMSAccountManager_OnSignInFailed] {exception.GetDescription()}");
            HMSAccountManager.Instance.OnSignInFailed -= HMSAccountManager_OnSignInFailed;
            storeEvents.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable);
        }

        #endregion
    }
}
