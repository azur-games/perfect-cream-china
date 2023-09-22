using Modules.Advertising;
using Modules.Analytics;
using Modules.AppsFlyer;
using Modules.Firebase;
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
using Modules.Max;
using UnityEngine;
using System.Collections;
using Amazon.Scripts;
using Code;

public class Starter : MonoBehaviour
{
    #region Fields

    [SerializeField] private LoaderScreen   loaderScreen;
    [SerializeField] private Camera         starterCamera;
    [SerializeField] private PopupManager   popupManagerPrefab = default;
    [SerializeField] private BalanceData    balanceData;
    private bool isFirstStart = false;
    private GlobalConfig _globalConfig;

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

        CreateGameTimer();
        
        if (Env.Instance == null)
        {
            Env.SendTechnical(1, "starter_awake_begin");
            isFirstStart = !CustomPlayerPrefs.HasKey("inventory");
            Env.SendTechnical(2, "loader_screen_show");
            loaderScreen.Show();
            _globalConfig = GlobalConfig.LoadSelf();
            Env.Create(_globalConfig);
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

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        var go = new GameObject("MaxDebuggerEnabler");
        DontDestroyOnLoad(go);
        go.AddComponent<MaxDebuggerEnabler>();
#endif  
    }

    #endregion



    #region Loading

    private void Initialize()
    {
        AnalyticsManagerSettings analyticsSettings = new AnalyticsManagerSettings
        {
            Services = new IAnalyticsService[]
            {
                new AppsFlyerAnalyticsServiceImplementor(LLAppsFlyerSettings.Instance),
                new FirebaseAnalyticsServiceImplementor(LLFirebaseSettings.Instance)
            }
        };
        AdvertisingManagerSettings advertisingSettings = new AdvertisingManagerSettings
        {
            AdServices = new IAdvertisingService[]
            {
                #if !UNITY_EDITOR && HIVE_HUAWEI
                new HuaweiAdvertisingServiceImplementor(huaweiAdsKitAbTestData),
                #endif
                new MaxAdvertisingServiceImplementor(),
                new EditorAdvertisingServiceImplementor(AdvertisingEditorSettings.Instance)
            },

            AdvertisingInfo = advertisingNecessaryInfo,

            AbTestData = new IAdvertisingAbTestData[] { new AdvertisingAbTestData()}
        };

        IPurchaseAnalyticsParameters purchaseAnalyticsParameters = new PurchaseAnalyticsParametersImplementor();
        purchaseAnalyticsParameters.SetParameter(
            "placement",
            () => DataStateService.Instance.Get("placement", SubscriptionPurchasePlacement.Default));
        Services.CreateServiceSingleton<IPurchaseAnalyticsParameters, PurchaseAnalyticsParametersImplementor>(purchaseAnalyticsParameters);
        Services.CreateServiceSingleton<IAnalyticsManagerSettings, AnalyticsManagerSettings>(analyticsSettings);
        Services.CreateServiceSingleton<IAdvertisingManagerSettings, AdvertisingManagerSettings>(advertisingSettings);
        
        var amazonService = new AmazonServiceImplementor();
        amazonService.Initialize();

        if (InitializationQueueConfiguration.DoesInstanceExist)
        {
            InitializationQueue.Instance
                               .SetOnComplete(Initialization_OnInitialized)
                               .SetOnError(Initialization_OnError)
                               .Apply(InitializationQueueConfiguration.Instance);
        }

        starterCamera.enabled = true;
        SendTechData();
    }


    private void LoadGame()
    {
        UserActivityChecker.Instance.Initialize();

        Env.Instance.ContinueLoading();
        loaderScreen.Hide();
        CreateGadsmeService();
        Destroy(gameObject);
    }

    private void CreateGadsmeService()
    {
        var gadsmeService = Instantiate(_globalConfig.GadsmeServiceGameObject).GetComponent<GadsmeService>();
        gadsmeService.InitializeInstance();
        gadsmeService.InitializeService();
    }
    
    private void SendTechData()
    {
        if (CustomPlayerPrefs.GetBool("CustomFirstLaunch", true))
        {
            BoGD.MonoBehaviourBase.Analytics.SendTechData();
            CustomPlayerPrefs.SetBool("CustomFirstLaunch", false);
            CustomPlayerPrefs.Save();
        }
    }

    private void CreateGameTimer()
    {
        var gameTimerGameObject = new GameObject
        {
            name = "GameTimer"
        };
        
        gameTimerGameObject.AddComponent<GameTimer>();
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
            // Env.Instance.Sound.StopMusic();

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
                Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.SuccessfulSubscription);
            }
            
            PopupManager.Instance.IsStartSubscriptionShown = true;
        }
    }

    #endregion



    #region Events handlers

    private void Initialization_OnInitialized()
    {
        advertisingNecessaryInfo.InitListeners();
        advertisingNecessaryInfo.OnPersonalDataDeletingDetect += AdvertisingNecessaryInfo_OnPersonalDataDeletingDetect;

        Env.Instance.PostInitialize();
        
        Services.AdvertisingManager.CreateInactivityTimer(() => 
            !Services.AdvertisingManager.IsFullScreenAdShowing);

        TryShowSubscriptionPopups();
        

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
