using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoGD
{
    public enum Method
    {
        None = 0,
        Awake = 1,
        Start = 2,
    }

    /// <summary>
    /// Тип синглтона
    /// </summary>
    public enum StaticType
    {
        UI                      = 0,
        Master                  = 1,
        TeamManager             = 2,
        Statistics              = 3,
        Awards                  = 4,
        Effects                 = 5,
        ScenesManager           = 6,
        RankSystem              = 7,
        Connector               = 8,
        PlayerData              = 9,
        Localization            = 10,
        ItemController          = 11,
        Empty                   = 12,
        Entities                = 13,
        Hideouts                = 14,
        Profile                 = 15,
        Options                 = 16,
        GameManager             = 17,
        Tips                    = 18,
        ItemsFactory            = 20,
        Input                   = 21,
        CameraSwitcher          = 22,
        StaticPattern           = 23,
        GameStateHandler        = 24,
        Sounds                  = 25,
        ExplosionSystem         = 26,
        CollidersContainer      = 27,
        Console                 = 28,
        GameModeContainer       = 29,
        StackController         = 30,
        SetsContainer           = 31,
        MainCamera              = 32,
        ItemShopController      = 33,
        GameOrderLogic          = 34,
        BattleRankSystem        = 35,
        BundlesLoader           = 36,
        UnitsContainer          = 37,
        CursorController        = 38,
        Exploder                = 39,
        AssetBundlesLoader      = 40,
        DevelopmentInfo         = 41,
        TaskDispatcher          = 42,
        InAppListener           = 43,
        CrashableContainer      = 44,
        NetEventsDispatcher     = 45,
        WeaponStorage           = 46,
        FracturedContainer      = 47,
        JsonParcer              = 48,
        InteractiveContainer    = 50,
        PickUpStorage           = 51,
        Ways                    = 52,
        Navigation              = 53,
        UnitsLoader             = 54,
        WeaponsLoader           = 55,
        QuestDispatcher         = 56,
        GameRules               = 57,
        AIDispatcher            = 58,
        ProfileCommon           = 59,
        ReferencesContainer     = 60,
        Files                   = 61,
        DragAndDropController   = 62,
        InGameShop              = 63,
        CaseGenerator           = 65,
        EventsQueue             = 66,
        Coroutines              = 67,
        ResourcesLoader         = 68,
        Server                  = 69,
        Music                   = 70,
        SummaryInfo             = 71,
        ImpactFactory           = 72,
        Tutorial                = 73,
        ActionsFactory          = 74,
        HitPointsContainer      = 75,
        RespawnController       = 76,
        ActionDispatcher        = 77,
        UpdateSystem            = 78,
        SpecialDispatcher       = 79,
        AutoAimerContainer      = 80,
        ScreenshotMaker         = 81,
        VibrationSystem         = 82,
        RemoteSettings          = 84,
        DailyBonus              = 85,
        Pool                    = 86,
        Pedia                   = 87,
        Buildings               = 88,
        GameInformer            = 89,
        SpawnObjectsContainer   = 90,
        CagesDispatcher         = 91,
        FriendsManager          = 92,
        EventsDispatcher        = 93,
        ColorSchemeSystem       = 94,
        ObjectsContainer        = 95,
        ItemsDispatcher         = 96,
        HintDispatcher          = 97,
        Constants               = 98,
        QueueController         = 99,

        AdsManager              = 201,
        AdsUnity                = 202,
        AdsFacebook             = 203,
        AdsAdmob                = 204,
        AdsIronSource           = 205,
        AdsAppLovin             = 206,

        NotificationsContainer  = 301,
        NotificationsUnity      = 302,

        TweenContainer          = 401,
        TweenAnimation          = 402,
        DOTween                 = 403,

        AnimationController     = 501,

        Analytics               = 601,
        AnalyticsAppsFlyer      = 602,
        AnalyticsAppMetrica     = 603,
        AnalyticsFacebook       = 604,
        AnalyticsUnity          = 605,
        AnalyticsAmplitude      = 606,
        AnalyticsFirebase       = 607,

        AuthSystem              = 701,
        AuthSystemAndroid       = 702,
        AuthSystemiOS           = 703,
        AuthSystemFacebook      = 704,
        AuthSystemGuest         = 705,

        DispatcherHPSystem      = 801,
        DispatcherInteractive   = 802,
        DispatcherAlerts        = 803,

        RestorableContainer     = 901,
        AchievementsSystem      = 902,
        CastleController        = 903,
        CheatController         = 904,
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    public enum Message
    {
        None = -1,
        /// <summary>
        /// Action Button from IInput component. ButtonKey - key, ButtonMode - mode
        /// </summary>
        Button = 0,
        /// <summary>
        /// Action Axis from IInput component. AxisKey - key, float - value
        /// </summary>
        Axis = 1,

        WeaponUsed = 2,
        Hit = 3,
        Destroy = 4,
        Movement = 5,
        Aiming = 6,
        Choose = 7,
        Use = 8,
        Targetting = 9,
        Respawn = 10,
        Debug = 11,
        Cheat = 12,
        ShellRequest = 13,
        UpdateAmmo = 14,
        WeaponPrepared = 15,
        Timer = 16,
        GamePhase = 17,
        ParameterChanged = 18,
        HeroChase = 19,
        WeaponChase = 20,
        Spawn = 21,
        /// <summary>
        /// Result window show request with parameters true/false - show/hide and ResultContext.
        /// </summary>
        Result = 22,
        Exit = 23,
        StatisticUpdate = 24,
        UsedReactor = 25,
        SetCurrentInteract = 26,
        Connected = 27,
        Disconnected = 28,
        Say = 29,
        SignIn = 30,
        ToBattle = 31,
        Annoncement = 32,
        LevelLoaded = 33,
        LevelLoading = 34,
        ToHangar = 35,
        Highlighted = 36,
        TranslateAll = 37,
        Warning = 38,
        ItemShopBuy = 39,
        ItemShopSelect = 40,
        EntitiesInited = 41,
        ItemChange = 42,
        EntityUpdated = 43,
        ItemShopClick = 44,
        TabShopSelect = 45,
        ToShop = 46,
        FromShop = 47,
        /// <summary>
        /// Comand to Add weapon to Stack in the start position and normalise stack.
        /// Require parameters: int - ItemShopID, ItemShopType - type of item type 
        /// </summary>
        AddToStackNew = 48,
        LoadLevelRequest = 49,
        SetPage = 50,
        ShopInited = 51,
        ToGameMode = 52,
        ToSettings = 53,
        UpdateProfileInfo = 54,
        SettingsGroupClick = 55,
        OptionSet = 56,
        OptionGet = 57,
        OptionSave = 59,
        SettingsInit = 60,
        InitOptions = 61,
        OptionCancel = 62,
        OptionApply = 63,
        SettingsSave = 64,
        BattleMenuToExit = 65,
        Shot = 66,
        Zoom = 67,
        OptionChanged = 68,
        OptionRefresh = 69,
        StartLogIn = 70,
        GetTip = 71,
        SendTip = 72,
        ShowTab = 73,
        SetWeapon = 74,
        ExitMenu = 75,
        ExitMenuReturn = 76,
        BattleMenuToBase = 77,
        ScrollClick = 78,
        BuyDetailedEntity = 79,
        SwitchWeapon = 80,
        Explosion = 81,
        OptionOk = 82,
        ApplyExitWindow = 83,
        ApplyWindowApply = 84,
        ApplyWindowCancel = 85,
        LockCursor = 86,
        EndPause = 87,
        ApplyExitWindowData = 88,
        MainMenuToExit = 89,
        /// <summary>
        /// the command to get all weapons in stack. if Have nothing as parameter than require send true + List<int> weaponID's
        /// </summary>
        SetStackElements = 90,
        CancelPage = 91,
        Disconnect = 92,
        GetAudioSource = 93,
        /// <summary>
        /// Answer to set Audio Source to Audio Manager.
        /// </summary>
        SetAudioSource = 94,
        /// <summary>
        /// Team score. require int - Team index, string - Team score
        /// </summary>
        SetTeamScore = 95,
        /// <summary>
        /// Respown on Result Window clicked for battle chat require
        /// </summary>
        RespawnRequest = 96,
        /// <summary>
        /// Time in battle event require string - time, GamePhase -phase for respawn button enable control
        /// </summary>
        BattleTime = 97,
        /// <summary>
        /// Respawn battle event for chat require string - Player name, int - Team index, bool - true to accept event
        /// </summary>
        RespawnMessage = 98,
        SetTeam = 99,
        /// <summary>
        /// Message send when grenade used. require int - grenadeCountAvailable, int seconds to explosion, Transform - grenade position
        /// </summary>
        GrenadesUse = 100,
        /// <summary>
        /// Message send when turret used. require int - turretsCountAvailable, int seconds to disable, Transform - turret position
        /// </summary>
        TurretUse = 101,
        ToEquipment = 102,
        ShowDropedItem = 103,
        ViewInteractiveUse = 104,
        WeaponDropped = 105,
        /// <summary>
        /// Message send when Stack checked the WeaponID for repetitions. require List<StackItem> - stack list, WeaponSlot - type of stack
        /// </summary>
        StackChecked = 106,
        /// <summary>
        /// Message send when Stack Element is changed. require WeaponSlot - slot type, int - SlotID, int - WeaponID
        /// </summary>
        StackChange = 107,
        /// <summary>
        /// Taken weapon after stack changed
        /// </summary>
        StackWeaponTaken = 108,
        ProfileWeaponChase = 109,
        InitSlots = 110,
        DeselectSlot = 111,
        RefreshDebug = 112,
        ItemShopOperation = 113,
        ItemShopOperationAply = 114,
        ItemShopOperationCancel = 115,
        AnimationRequest = 116,
        SelectItem = 117,
        EntitySell = 118,
        /// <summary>
        /// Double click to shop item element require IItemShop
        /// </summary>
        ItemDoubleClick = 119,
        RefreshShopItemStats = 120,
        BattleAchieveRequest = 121,
        Recoil = 122,
        HighlighlerCreated = 123,
        EffectRequest = 124,
        ShowWindowRequest = 125,
        RankUp = 126,
        FragUp = 127,
        SoundRequest = 128,
        AxisVector = 129,
        Crouch = 130,
        Jump = 131,
        Reload = 132,
        SpeedChanged = 133,
        WindowOpened = 134,
        WindowClosed = 135,
        ModelInstantiated = 136,
        InAppInitialized = 137,
        BattleMessage = 138,
        BattleMessageRequest = 139,
        ItemShopChase = 140,
        DestroyObjectRequest = 141,
        DestroyObjectReceive = 142,
        HighlightItem = 143,
        RequestPage = 144,//Page - page
        NextItem = 145,//int - direction     
        ItemsInited = 146,//List<ICustomizationSlot> - slots
        NextSkin = 147,//int - direction
        SkinSet = 148,
        AnimationResponse = 149,
        PickedUp = 150,
        FracturedRequest = 151,
        FracturedReceive = 152,
        BattleInformation = 153,
        NetMessageReceive = 154,//int net message, parameters[] parameters
        DestroyPlayerRequest = 155,
        ImpactApplied = 156,
        UnitInstantiated = 157,
        ItemDropped = 158,
        NextUnit = 159,
        FoundObject = 160,
        ToMatchMaker = 161,
        JoinRoom = 162,
        ProfileLoaded = 163,
        DestroyStatic = 164,
        DragEvent = 165,
        PrivatePolicyAgreed = 166,
        Rate = 167,
        SetSound = 168,
        SetMusic = 169,
        LoginChange = 170,
        MergeUnit = 171,
        PutUnitToWay = 172,
        ReturnUnit = 173,
        OpenBox = 174,
        Exchange = 175,
        OthersClick = 176,
        ZoomPercent = 177,
        Earnings = 178,
        ServerClick = 179,
        LocalClick = 180,
        LandingBox = 181,
        ShowWarning = 182,
        AutoShotTimer = 183,
        LoadingProgress = 184,
        ItemClick = 185,

        /// <summary>
        /// Command to update panel kills 
        /// parameters: 1. int: value of kills
        /// </summary>
        Kills = 186,

        /// <summary>
        /// Command to hide all battle panels 
        /// parameters: 1. bool: true-hide/ false-show
        /// </summary>
        HidePanels = 187,

        /// <summary>
        /// Command to Show Battle Informer
        /// parameters: 1. string(0) - top text
        ///             2. bool or none: true - with icon. then string(1) is icon sprite path else without icons
        ///             3. string(1 or 2) bottom text. If with icon then string(2) else string(1) or none if bottom text not require
        /// </summary>
        InformerShow                = 188,

        /// <summary>
        /// Command to Show OwnKilled panel
        /// parameters: 1. 
        /// </summary>
        OwnKilled                   = 189,

        SwitchCamera                = 190,
        SetVibration                = 191,
        RestoreTransactions         = 192,

        CreateRoom                  = 193,
        SlotClick                   = 194,

        SetBlood                    = 194,
        RequestAchievment           = 195,
        RequestLogin                = 196,
        RequestLeaderboard          = 197,
        RentSkin                    = 198,

        AddMoney                    = 199,
        TowerBuild                  = 200,
        AchieveIncrement            = 201,
        PassengerState              = 202,

        DailyNotification           = 203,
        DailyClaim                  = 204,
        SetStartPage                = 205,
        DailyNewMonth               = 206,
        TutorialStepComplete        = 207,
        NewPopup                    = 208,
        TutorialCompleted           = 209,
        AnimateResources            = 210,

        RegisterResourceElements    = 211,
        GameNotificationAdded       = 212,
        NoMoreSlots                 = 213,

        UnitTrashed                 = 214,
        LevelUpReward               = 215,
        AddResources                = 216,
        RequestADS                  = 217,
        AlertCreated                = 218,
        HideHint                    = 219,

        QuestStarted                = 220,
        EndGame                     = 221,
        AlertCreatedForBot          = 222,
        ProfileLocalLoaded          = 223,
        /// <summary>
        /// IContextObject
        /// </summary>
        RequestItemContext          = 224,
        CameraAnimation             = 225,

        ResetADSTimer               = 226,
        LoadingScreenFinalized      = 227,
        EndGameAnimation            = 228,
        SelectUnit                  = 229,
        RealMoneyPurchase           = 230,
        Back                        = 231,
        ButtonClicked               = 232,
        EventReached                = 233,
        KeysSpent                   = 234,
        CameraPointAnimation        = 235,
        ShowUI                      = 236,
        TutorialStepDelay           = 237,
        PinClick                    = 238,
        IncrementTutorialStep       = 239,
        DecrementTutorialStep       = 240,
        DeactiveHorizontalInput     = 241,
        ActivateHorizontalInput     = 242,
        AddCoin                     = 243,

        DragUnitStart               = 300,
        DragUnitEnd                 = 301,
        ShowSimplePanel             = 302,
        RandomBonusSpawn            = 303,
        SpeedUpButton               = 304,
        RandomBonusRelocate         = 305,
        TournamentEnter             = 306,
        RandomBonusBought           = 307,
        QuestsRefreshed             = 308,
        QuestSkip                   = 309,
        PanelMessages               = 310,
        FriendsAction               = 311,

        PlayerCreated               = 313,
        PlayerReady                 = 314,
        Alert                       = 315,
        Promocode                   = 316,
        GDPRInited                  = 317,


        AuthUnbind                  = 400,
        AuthBind                    = 401,
        AuthLogin                   = 402,
        ShowPanel                   = 403,
        MapLevel                    = 404,
        CreateMapPoint              = 405,
        DragItemDroped              = 406,
        GetItem                     = 407,
        LoseItem                    = 408,
        PanelHidden                 = 409,
        ClosePanel                  = 410,
        CreateTopItem               = 411,
        AddBonus                    = 412,
        UnitDeath                   = 413,
        UnitAttack                  = 414,
        SetBattleCamera             = 415,
        SetBaseCamera               = 416,
        GetRectTransform            = 420,
        SetRectTransform            = 421,       

        //UI Core
        SetInteractable             = 10000,
        SetElement                  = 10001,
        ClosePage                   = 10002,
        SetValue                    = 10003,
        SetVisible                  = 10004,
        SendCustomMessage           = 10005,
        SetState                    = 10006,
        UpdateHeader                = 10007,
        ReadyToClose                = 10008,
        UpdateMessage               = 10009,
        CreatePositionInfoBanner    = 10010,
        CreateUniversalItem         = 10011,
        ClearUniversalItems         = 10012,
        OnBaseWindow                = 10013,
        ResetADSCounter             = 10014,
        UpdateGameInfo              = 10016,
		RareChange                  = 10017,
        RequestRare                 = 10018,
        RemoveElement               = 10019,
        ShowMessage                 = 10020,
        HideMessage                 = 10021,
        HideWindow                  = 10022,

        //Rate US        
        StarClick                   = 20001,

        //ADS
        ResetByADS                  = 20002,

        //Window Counter
        CountdownStart              = 30001,
        CountdownEnd                = 30002,

        //WindowResult
        PreBonusView                = 40001,

        //Input
        Swipe                       = 50001,
        StaticInicialized = 50002,
    }

    public enum TextParameter
    {
        Color = 0,
        Size = 1,
        Style = 2,
    }

    public enum LocalizationType
    {
        All = -1,
        None = 0,
        Interface = 1,
        Task = 2,
        Vehicle = 3,
        Module = 4,
        Tutorial = 5,
        Combo = 6,
        Vote = 7,
        Tip = 8,
    }

    public enum FileType
    {
        None = 0,
        Profile = 1,
        Localization = 2,
        Debug = 3,
    }



}