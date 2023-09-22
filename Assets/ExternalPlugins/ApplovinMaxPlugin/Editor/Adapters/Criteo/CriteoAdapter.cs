using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class CriteoAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:criteo-adapter:4.6.0.4",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationCriteoAdapter",
                Version = "4.5.0.3",
                Source = ""
            }
        };
    }
}
