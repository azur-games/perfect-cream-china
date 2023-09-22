using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class HyprMXAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:hyprmx-adapter:6.0.2.0",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationHyprMXAdapter",
                Version = "6.0.1.4",
                Source = ""
            }
        };
    }
}
