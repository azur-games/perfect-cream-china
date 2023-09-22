using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayRules
{
    public StorableString CreamSkin = new StorableString("CreamSkin", "WhiteStar");
    public StorableString CameraView = new StorableString("CameraView", "0");
    public StorableString CreamForm = new StorableString("CreamForm", "Straight");
    public StorableInt ScorePerTickNormalGame = new StorableInt("ScorePerTickNormalGame", 1);
    public StorableInt ScorePerTickFastGame = new StorableInt("ScorePerTickFastGame", 3);
    public StorableBool FastGame = new StorableBool("FastGame", false);
    public StorableBool FeverNumbers = new StorableBool("FeverNumbers", true);
    public StorableBool Keys = new StorableBool("Keys", true);
    public StorableBool Effects = new StorableBool("Effects", true);
}
