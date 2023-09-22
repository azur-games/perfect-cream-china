using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadRoom : Room
{
    protected override void OnEnter(RoomContext roomContext)
    {
        base.OnEnter(roomContext);

        GoToTheMetagame();
    }

    private void GoToTheMetagame()
    {
        Env.Instance.UI.Overlay.SetImmediately(this, Env.Instance.UI.Config.StartupOverlayColor, (overlayInstance) =>
        {
            if (0 == Env.Instance.Inventory.CurrentLevelIndex)
            {
                GameplayRoomContext context = new GameplayRoomContext();
                Env.Instance.Rooms.SwitchToRoom<GameplayRoom>(false, context, () =>
                {
                    overlayInstance.Close();
                });
            }
            else
            {
                MetagameRoomContext context = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.None);
                Env.Instance.Rooms.SwitchToRoom<MetagameRoom>(false, context, () =>
                {
                    overlayInstance.Close();
                });
            }
        });
    }
}
