public class GameplayRoomContext : RoomContext
{
    public bool IsExtraLevel { get; private set; }

    public GameplayRoomContext()
    {
        IsExtraLevel = false;
    }

    public LevelAsset GetLevelAssetByDefaultFlow(GameplayRoomConfig gameplayRoomConfig)
    {
        string levelName = "TutorialLevel";
        if (0 < Env.Instance.Inventory.CurrentLevelIndex) levelName = "CommonLevel";

        int extraLevelPeriod = BalanceDataProvider.Instance.ExtraLevelsPeriod;
        IsExtraLevel = false;

        if (Env.Instance.Inventory.IsNextExtraLevel() && (0 < gameplayRoomConfig.ExtraLevels.Count))
        {
            int extraLevelVariant = ((Env.Instance.Inventory.CurrentLevelIndex + 1) / extraLevelPeriod) % gameplayRoomConfig.ExtraLevels.Count;
            levelName = gameplayRoomConfig.ExtraLevels[extraLevelVariant];
            IsExtraLevel = true;
        }

        ContentItem levelItem = Env.Instance.Content.LoadContentItem(ContentAsset.AssetType.Level, itemInfo => itemInfo.Name == levelName);

        return levelItem.Asset as LevelAsset;
    }
}
