using Modules.Advertising;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
#if HIVE_HUAWEI
using Modules.HmsPlugin.Advertising;
#endif
using Modules.InAppPurchase;
using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour
{
    #region Fields

    [SerializeField] private LoaderScreen   loaderScreen;
    [SerializeField] private Camera         starterCamera;
    [SerializeField] private PopupManager   popupManagerPrefab = default;
    [SerializeField] private BalanceData    balanceData;
    private bool isFirstStart = false;

    private readonly RemoteAvailabilityAbTestData remoteAvailabilityAbTestData = new RemoteAvailabilityAbTestData();
    
    #if HIVE_HUAWEI
        private readonly HuaweiAdsKitAbTestData huaweiAdsKitAbTestData = new HuaweiAdsKitAbTestData();
    #endif
    
    private readonly AdvertisingNecessaryInfo advertisingNecessaryInfo = new AdvertisingNecessaryInfo();

    #endregion



    #region Unity lifecycle

    private void Start()
    {
        StartCoroutine(WaitForOneFrame());
    }

    private IEnumerator WaitForOneFrame()
    {
        yield return new WaitForEndOfFrame();
        Init();
    }

    private void Init()
    {
        new BalanceDataProvider(balanceData);
        if (SystemInfo.graphicsMemorySize > 1000 && SystemInfo.systemMemorySize > 3000)
        {
            QualitySettings.SetQualityLevel(1);
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
        }

        if (Env.Instance == null)
        {
            Env.SendTechnical(1, "starter_awake_begin");
            isFirstStart = !CustomPlayerPrefs.HasKey("inventory");
            Env.SendTechnical(2, "loader_screen_show");
            loaderScreen.Show();
            GlobalConfig globalConfig = GlobalConfig.LoadSelf();
            Env.Create(globalConfig);
            Env.SendTechnical(3, "env_created");
            if (isFirstStart)
            {
                Env.Instance?.Inventory.Save();
            }

            Initialize();
            Env.SendTechnical(4, "starter_initialized");
        }
        else
        {
            loaderScreen.Hide();
            Env.Instance.SceneManager.OnMainSceneLoaded(this);
        }
    }

    #endregion



    #region Loading

    private void Initialize()
    {
        AdvertisingManagerSettings advertisingSettings = new AdvertisingManagerSettings
        {
            AdServices = new IAdvertisingService[]
            {
                #if !UNITY_EDITOR && HIVE_HUAWEI
                new HuaweiAdvertisingServiceImplementor(huaweiAdsKitAbTestData),
                #endif
                new EditorAdvertisingServiceImplementor(AdvertisingEditorSettings.Instance)
            },

            AdvertisingInfo = advertisingNecessaryInfo,

            AbTestData = new IAdvertisingAbTestData[] { new AdvertisingAbTestData()}
        };
        
        // Services.CreateServiceSingleton<IPurchaseAnalyticsParameters, PurchaseAnalyticsParametersImplementor>(purchaseAnalyticsParameters);
        Services.CreateServiceSingleton<IAdvertisingManagerSettings, AdvertisingManagerSettings>(advertisingSettings);

        if (InitializationQueueConfiguration.DoesInstanceExist)
        {
            InitializationQueue.Instance
                               .SetOnComplete(Initialization_OnInitialized)
                               .SetOnError(Initialization_OnError)
                               .Apply(InitializationQueueConfiguration.Instance);
        }

        starterCamera.enabled = true;
    }


    private void LoadGame()
    {
        UserActivityChecker.Instance.Initialize();

        Env.Instance.ContinueLoading();
        loaderScreen.Hide();
        Destroy(gameObject);
    }


    private void TryShowSubscriptionPopups()
    {
        PopupManager popupManagerInstance = Instantiate(popupManagerPrefab);
        DontDestroyOnLoad(popupManagerInstance.gameObject);

        PopupManager.Instance.OnSubscriptionPopupVisibilityChange += isVisible =>
        {
            AdvertisingManager.Instance.IsSubscriptionShowing = isVisible;
        };

        IStoreManager storeManager = Services.GetService<IStoreManager>();
        if (balanceData.isSubscriptionAvailable && !PopupManager.Instance.IsStartSubscriptionShown &&
            !storeManager.HasAnyActiveSubscription)
        {
            Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicMetagame);

            Env.Instance.UI.Messages.ShowSubscriptionPopup(
                balanceData.isSubscriptionExtended
                    ? SubscriptionBox.SubscriptionBoxType.ExtendedSubscription
                    : SubscriptionBox.SubscriptionBoxType.Subscription,
                OnCloseSubscriptionPopup,
                isStartSubscription: true);
        }
        else
        {
            Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Reward);
        }

        void OnCloseSubscriptionPopup(bool isSubscriptionActivated)
        {
            if (Env.Instance.Rooms.GameplayRoom != null)
            {
                Env.Instance.Sound.PlayMusic(AudioKeys.Music.MusicGameplay);
            }

            if (isSubscriptionActivated)
            {
                // Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.SuccessfulSubscription);
            }
            
            PopupManager.Instance.IsStartSubscriptionShown = true;
        }
    }

    #endregion



    #region Events handlers

    private void Initialization_OnInitialized()
    {
        // advertisingNecessaryInfo.InitListeners();
        // advertisingNecessaryInfo.OnPersonalDataDeletingDetect += AdvertisingNecessaryInfo_OnPersonalDataDeletingDetect;

        Env.Instance.PostInitialize();
        //
        // Services.AdvertisingManager.CreateInactivityTimer(() => 
        //     !Services.AdvertisingManager.IsFullScreenAdShowing);
        //
        // TryShowSubscriptionPopups();
        

        LoadGame();
    }


    private static void Initialization_OnError(object registerable, InitializationStatus initializationStatus)
    {
        CustomDebug.LogError($"Error has occured while initialization: {initializationStatus}");
    }


    private void AdvertisingNecessaryInfo_OnPersonalDataDeletingDetect()
    {
        Env.Instance.UI.Overlay.SetImmediately(this, new Color(0.16f, 0.16f, 0.16f, 0.5f), (overlayInstance) =>
        {
            Time.timeScale = 0.0f;
        });

        if (Env.Instance.Rooms.GameplayRoom != null)
        {
            GameplayController.IsGameplayActive = false;
        }

        Env.Instance.Sound.IsSoundEnabled = false;
    }
    
    #endregion
    // 1.11.26.0
}
