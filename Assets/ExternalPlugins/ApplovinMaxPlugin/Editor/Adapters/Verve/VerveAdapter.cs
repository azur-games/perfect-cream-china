using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class VerveAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:verve-adapter:2.12.1.0",
                Repository = "https://verve.jfrog.io/artifactory/verve-gradle-release"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationVerveAdapter",
                Version = "2.12.1.0",
                Source = ""
            }
        };
    }
}
