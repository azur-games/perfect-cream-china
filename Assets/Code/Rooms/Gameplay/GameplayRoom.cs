using System.Collections.Generic;
using BoGD;

public class GameplayRoom : Room
{
    public static int CurrentLevelIndex { get; private set; }
    public GameplayRoomContext Context { get; private set; }

    public GameplayController Controller { get; private set; }
    public GameplayRoomUI UI { get; private set; }

    protected override void OnEnter(RoomContext context)
    {
        base.OnEnter(context);

        Context = context as GameplayRoomContext;

        LevelAsset levelAsset = Context.GetLevelAssetByDefaultFlow(Config as GameplayRoomConfig);
        Controller.CreateScene(levelAsset);     
    }

    protected override void OnLeave()
    {
        base.OnLeave();
        //CustomDebug.LogError("ON LEAVE ROOM");
        //Dictionary<string, object> data = new Dictionary<string, object>();
        //MonoBehaviourBase.Analytics.SendEvent("level_finish", data);
    }

    public void OnGameplayLoaded(GameplayController gameplayController)
    {
        this.Controller = gameplayController;
    }

    public void OnUILoaded(GameplayRoomUI gameplayUI)
    {
        this.UI = gameplayUI;
    }
}
