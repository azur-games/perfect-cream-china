using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class NendAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:nend-adapter:8.0.1.0",
                Repository = "http://fan-adn.github.io/nendSDK-Android-lib/library"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationNendAdapter",
                Version = "7.3.0.0",
                Source = ""
            }
        };
    }
}
