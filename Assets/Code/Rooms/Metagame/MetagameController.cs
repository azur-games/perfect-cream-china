using System.Collections;
using System.Collections.Generic;
using Code;
using Gadsme;
using UnityEngine;

public class MetagameController : MonoBehaviour
{
    const string MODEL_PREFS_KEY = "MetagamePersModel";

    [SerializeField] private MetagameRoomConstructor roomConstructor;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject pivots;

    [SerializeField] private PersCharacter Pers;
    [SerializeField] private PersCharacter Barman;
    [SerializeField] private GadsmePlacementVideo _metagamePlacementVideo;
    [SerializeField] private GadsmePlacementBanner _metagamePlacementBanner;

    #region Unity lifecycle

    private void Awake()
    {
        if (null == Env.Instance)
        {
            Debug.Log("Project started with this scene. Main scene loading =>");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main", UnityEngine.SceneManagement.LoadSceneMode.Single);
            return;
        }

        Env.Instance.Rooms.MetagameRoom.OnMetagameLoaded(this);
    }

    #endregion

    private void Start()
    {
        roomConstructor.Construct();

        UpdateChar();
        GadsmeService.Instance.OnGamePhaseChange(camera);
        GadsmeService.Instance.PlacementVideos.Add(_metagamePlacementVideo);
    }

    public MetagameRoomConstructor GetRoomConstructor()
    {
        return roomConstructor;
    }

    public void UpdateChar()
    {
        // Applause
        MetagameRoomContext.GameplaySessionResult prevGameplaySessionResult = Env.Instance.Rooms.MetagameRoom.Context.GameplayResult;
        if ((prevGameplaySessionResult == MetagameRoomContext.GameplaySessionResult.Completed) ||
            (prevGameplaySessionResult == MetagameRoomContext.GameplaySessionResult.CompletedExtraLevel))
        {
            Pers.StartApplause(true);
            Barman.StartDelivering(
                Env.Instance.Rooms.MetagameRoom.Context.TypeOfDeliveredObject,
                Env.Instance.Rooms.MetagameRoom.Context.DeliveredObjectColor1,
                Env.Instance.Rooms.MetagameRoom.Context.DeliveredObjectColor2);
        }

        if (prevGameplaySessionResult == MetagameRoomContext.GameplaySessionResult.Failed)
        {
            Pers.StartCry(false);
        }

        // Boy-Girl model
        int modelIndex = PlayerPrefs.GetInt(MODEL_PREFS_KEY, 0);
        if (Env.Instance.Rooms.MetagameRoom.Context.LastItemReceived != null)
        {
            modelIndex = 1 - modelIndex;
            PlayerPrefs.SetInt(MODEL_PREFS_KEY, modelIndex);
        }

        Pers.SwitchModel(0 == modelIndex ? PersCharacter.Model.Boy : PersCharacter.Model.Girl);
    }

    public PersCharacter GetClientCharacter() { return Pers; }
    public PersCharacter GetBarmanCharacter() { return Barman; }

    public void SwitchPers()
    {
        int modelIndex = PlayerPrefs.GetInt(MODEL_PREFS_KEY, 0);
        modelIndex = 1 - modelIndex;
        PlayerPrefs.SetInt(MODEL_PREFS_KEY, modelIndex);
        Pers.SwitchModel(0 == modelIndex ? PersCharacter.Model.Boy : PersCharacter.Model.Girl);
    }

    public int GetMaxBarUpgradeLevel()
    {
        return roomConstructor.GetMaxGradeLevel();
    }

    public void UpInteriorToCurrentGrade(System.Action onFinish)
    {
        roomConstructor.UpToGrade(onFinish, true);

        Pers.StartUpgrade(true);
        Barman.StartUpgrade(false);
    }

    public Camera Camera => camera;
    public GameObject Pivots => pivots;
}
