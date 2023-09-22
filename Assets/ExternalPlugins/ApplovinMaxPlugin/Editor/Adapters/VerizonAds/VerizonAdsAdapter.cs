using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class VerizonAdsAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:verizonads-adapter:1.14.0.8",
                Repository = "https://artifactory.verizonmedia.com/artifactory/maven/"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationVerizonAdsAdapter",
                Version = "1.14.2.6",
                Source = ""
            }
        };
    }
}
