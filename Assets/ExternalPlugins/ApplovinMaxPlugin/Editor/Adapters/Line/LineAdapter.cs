using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class LineAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:line-adapter:2021.10.29.1",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationLineAdapter",
                Version = "2.4.20211028.2",
                Source = ""
            }
        };
    }
}
