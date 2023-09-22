using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class GoogleAdManagerAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:google-ad-manager-adapter:[20.6.0.6]",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationGoogleAdManagerAdapter",
                Version = "9.11.0.4",
                Source = ""
            }
        };
    }
}
