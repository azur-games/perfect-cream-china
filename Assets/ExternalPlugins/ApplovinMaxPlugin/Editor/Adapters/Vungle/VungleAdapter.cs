using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class VungleAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:vungle-adapter:6.10.5.1",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationVungleAdapter",
                Version = "6.10.6.2",
                Source = ""
            }
        };
    }
}
