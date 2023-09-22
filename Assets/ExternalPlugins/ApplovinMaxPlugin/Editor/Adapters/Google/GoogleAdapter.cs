using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class GoogleAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:google-adapter:[21.4.0.0]",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationGoogleAdapter",
                Version = "9.11.0.4",
                Source = ""
            }
        };
    }
}
