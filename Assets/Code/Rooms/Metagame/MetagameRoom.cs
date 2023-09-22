using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetagameRoom : Room
{
    public MetagameRoomContext Context { get; private set; }

    public MetagameController Controller;

    protected override void OnEnter(RoomContext context)
    {
        base.OnEnter(context);

        Context = context as MetagameRoomContext;
    }

    public void OnMetagameLoaded(MetagameController metagameController)
    {
        this.Controller = metagameController;
    }
}
