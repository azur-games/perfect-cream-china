using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class FacebookAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:facebook-adapter:[6.12.0.1]",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationFacebookAdapter",
                Version = "6.10.0.1",
                Source = ""
            }
        };
    }
}
