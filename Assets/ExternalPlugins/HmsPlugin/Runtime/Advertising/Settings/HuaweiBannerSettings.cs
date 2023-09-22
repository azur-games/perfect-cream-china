using System;
using static HuaweiConstants.UnityBannerAdPositionCode;

namespace Modules.HmsPlugin.Advertising
{
    [Serializable]
    public class HuaweiBannerSettings
    {
        public UnityBannerAdPositionCodeType BannerPosition = UnityBannerAdPositionCodeType.POSITION_BOTTOM;

        public string BannerSize = "BANNER_SIZE_320_50";
    }
}