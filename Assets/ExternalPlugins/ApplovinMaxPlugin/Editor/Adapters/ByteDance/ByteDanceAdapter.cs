using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class ByteDanceAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:bytedance-adapter:4.3.0.7.5",
                Repository = "https://artifact.bytedance.com/repository/pangle"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationByteDanceAdapter",
                Version = "4.3.0.5.2",
                Source = ""
            }
        };
    }
}
