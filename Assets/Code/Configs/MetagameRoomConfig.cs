using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MetagameRoomConfig", menuName = "Configs/MetagameRoomConfig")]
public class MetagameRoomConfig : RoomConfig
{
    #region Fields

    [Space]
    [Header("Upgrades")]
    [SerializeField] List<int> upgradePrices;
    
    #endregion



    #region Properties

    public List<int> UpgradePrices
    {
        get
        {
            return upgradePrices;
        }
    }

    #endregion
}
