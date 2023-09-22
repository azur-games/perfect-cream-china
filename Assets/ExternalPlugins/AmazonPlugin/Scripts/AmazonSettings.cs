using Modules.General.HelperClasses;
using UnityEngine;

namespace Amazon.Scripts
{
    [CreateAssetMenu(fileName = "AmazonSettings")]
    public class AmazonSettings : ScriptableSingleton<AmazonSettings>
    {
       public string AppId;
       public string BannerSlotId;
       public string InterstitialSlotId;
       public string RewardedVideoSlotId;
       public int RewardedVideoWidth = 320;
       public int RewardedVideoHeight = 480;
    }
}
