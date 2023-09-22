using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class MyTargetAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:mytarget-adapter:5.15.5.0",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationMyTargetAdapter",
                Version = "5.16.0.0",
                Source = ""
            }
        };
    }
}
