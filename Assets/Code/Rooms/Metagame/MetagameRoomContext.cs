using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetagameRoomContext : RoomContext
{
    public enum GameplaySessionResult
    {
        None,
        Completed,
        CompletedExtraLevel,
        Failed
    }

    public LevelAsset.DeliveredObjectType TypeOfDeliveredObject { get; set; } = LevelAsset.DeliveredObjectType.None;
    public Color DeliveredObjectColor1 { get; set; }
    public Color DeliveredObjectColor2 { get; set; }

    public GameplaySessionResult GameplayResult { get; private set; }
    public bool NewShapeRecieved { get; set; }
    public ContentItemInfo LastItemReceived { get; set; }

    public MetagameRoomContext(GameplaySessionResult gameplayResult, ContentItemInfo lastItemReceived = null)
    {
        GameplayResult = gameplayResult;
        LastItemReceived = lastItemReceived;
    }
}
