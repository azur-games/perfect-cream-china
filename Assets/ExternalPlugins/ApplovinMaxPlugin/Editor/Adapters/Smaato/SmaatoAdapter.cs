using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class SmaatoAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:smaato-adapter:21.8.1.2",
                Repository = "https://s3.amazonaws.com/smaato-sdk-releases/"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationSmaatoAdapter",
                Version = "21.7.4.1",
                Source = ""
            }
        };
    }
}
