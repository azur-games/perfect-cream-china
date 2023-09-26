using BoGD;
using Modules.General;
using Modules.General.Abstraction.InAppPurchase;
using System.Collections.Generic;
using UnityEngine;


public class Env
{
    #region Nested types

    public class PassLevelInfo
    {
        public int lastLevelIndex;
    }

    #endregion


    public static Env Instance { get; private set; } = null;

    public static bool IsUABuild {
        get {
#if UA_BUILD
            return true;
#else
            return Application.isEditor;
#endif
        }
    }

    public static void Create(GlobalConfig globalConfig)
    {
        if (null != Instance)
        {
            throw new System.Exception("Trying to create duplicated Env");
        }

        Instance = new Env();
        Instance.Init(globalConfig);
    }

    #region Properties

    public GlobalConfig Config { get; private set; }
    public RoomManager Rooms { get; private set; }
    public SceneManager SceneManager { get; private set; }
    public UIManager UI { get; private set; }
    //public SrDebuggerHelper Debugger { get; private set; }
    public SoundHelper Sound { get; private set; }
    //public AssetBundlesManager Bundles { get; private set; }
    public ContentManager Content { get; private set; }
    public GameplayRules Rules { get; private set; }

    public Inventory Inventory { get; private set; }

    public PassLevelInfo PassLevel { get; private set; }

    private static DataIntPrefs dataLevelCount = new DataIntPrefs("int.level.count", 0);
    private static DataIntPrefs dataTechnicalStep = new DataIntPrefs("int.technical.step", 0);


    public static bool Payer
    {
        get => payerPrefs.Value == 1;
        set => payerPrefs.Value = value ? 1 : 0;
    }

    private static DataIntPrefs payerPrefs = new DataIntPrefs("int.payer");
    #endregion


    private Dictionary<string, object>  savedAnalytics = new Dictionary<string, object>();
    private float                       timeStart;

    public static int continues = 0;

    public void FullRestart(bool tryToHardRestart = false)
    {
        if (tryToHardRestart)
        {
            if (Utils.Helpers.ApplicationFlow.RestartApplication())
            {
                return;
            }
        }

        Debug.Log("Soft restart...");

        string initialSceneName = Config.InitialSceneName;

        Deinitialize(() =>
        {
            Instance = null;
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(initialSceneName);
        });
    }


    private void Init(GlobalConfig globalConfig)
    {
        Application.targetFrameRate = 60;

        Object.DontDestroyOnLoad(new GameObject("UnityDispatcher", typeof(UnityDispatcher)));

        Config = globalConfig;
        Rules = new GameplayRules();
        Inventory = Inventory.Create();
        //Debugger = new SrDebuggerHelper(Config.SrDebuggerEnabled);

        UI = new UIManager();
        SceneManager = new SceneManager();
        Rooms = new RoomManager();

        Content = new ContentManager();

        PassLevel = new PassLevelInfo() { lastLevelIndex = -1 };
        CheckFinish();
    }


    public void PostInitialize()
    {
        Sound = new SoundHelper(Config.SoundManager);
    }


    public void ContinueLoading()
    {
        Rooms.SwitchToInitialRoom();
    }


    private void Deinitialize(System.Action onFinish)
    {
        UI.CloseAll();
        //Debugger.UnregisterAll();
        Config = null;

        onFinish();
    }


    public void CheckFinish()
    {
        var saved = PlayerPrefs.GetString("analytics", "");
        if (!saved.IsNullOrEmpty())
        {
            SendFinish("game_closed", 0, 0, 0, 0);
        }
    }

    public void SendStart()
    {
        CheckFinish();
        dataLevelCount.Increment(1);

        var levelNumber = Inventory.CurrentLevelIndex + 1;
        savedAnalytics["level_number"] = levelNumber;
        savedAnalytics["level_name"] = levelNumber + "_" + "level";
        savedAnalytics["level_type"] = Inventory.IsNextExtraLevel() ? "bonus" : "normal";
        savedAnalytics["creme_id"] = Inventory.AvailableCreams.Last().ToLower().Replace(' ', '_');
        savedAnalytics["valve_id"] = Inventory.ValveName.ToLower().Replace(' ', '_');
        savedAnalytics["confiture_id"] = Inventory.CurrentConfiture.IsNullOrEmpty() ? "none" : Inventory.CurrentConfiture.ToLower().Replace(' ', '_');
        
        PlayerPrefs.SetString("analytics", savedAnalytics.ToJSON());

        Dictionary<string, object> data = new Dictionary<string, object>();
        foreach (var pair in savedAnalytics)
        {
            data[pair.Key] = pair.Value;
        }
        timeStart = Time.time;
    }


    public void SendResultWindow(string levelResult, string action, int softReward, int starsReward)
    {
        var data = new Dictionary<string, object>();
        data["result"] = action;
        data["level_result"] = levelResult;
        data["soft_reward"] = softReward;
        //data["stars_reward"] = starsReward;
    }

    public void SendFinish(string result, int stars, int mistakes, int progress, int reward)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        if (savedAnalytics == null || savedAnalytics.Count == 0)
        {
            var saved = PlayerPrefs.GetString("analytics", "");
            savedAnalytics = saved.IsNullOrEmpty() ? new Dictionary<string, object>() : saved.FromJSON();
        }

        foreach (var pair in savedAnalytics)
        {
            data[pair.Key] = pair.Value;
        }
        data["result"] = result;
        data["time"] = (long)(Time.time - timeStart);
        data["soft_reward"] = reward;
        data["progress"] = progress;
        data["stars"] = stars;
        data["mistakes"] = mistakes;
        data["continue"] = continues;
        PlayerPrefs.SetString("analytics", "");

        continues = 0;
    }

    public static Dictionary<string, object> GetGlobalParameters()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["level_count"] = dataLevelCount.Value;
        data["payer"] = payerPrefs.Value;
        data["soft_balance"] = Instance == null ? 0 : Instance.Inventory.Bucks;
        data["subscription"] = "none"; 
        return data;
    }

    public void SendPopup(string id, string reason, string result)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["pop_up_id"] = id;
        dictionary["show_reason"] = reason;
        dictionary["result"] = result;
    }

    public void SendSettings(string action)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["action"] = action;
    }

    public void SendWindow(string id)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["window_id"] = id;
    }

    public static void SendTechnical(int step, string id)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["step_name"] = (step <= 9 ? "0" : "") + step + "_" + id;
        dictionary["first_start"] = dataTechnicalStep.Value < step;

        if (step > dataTechnicalStep.Value)
        {
            dataTechnicalStep.Value = step;
        }
    }
}
