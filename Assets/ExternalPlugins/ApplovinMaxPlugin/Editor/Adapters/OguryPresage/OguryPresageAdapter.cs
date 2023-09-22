using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class OguryPresageAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:ogury-presage-adapter:5.2.0.0",
                Repository = "https://maven.ogury.co"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationOguryPresageAdapter",
                Version = "2.6.0.1",
                Source = ""
            }
        };
    }
}
