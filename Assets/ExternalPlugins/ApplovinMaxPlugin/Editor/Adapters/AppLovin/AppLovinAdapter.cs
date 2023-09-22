using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class AppLovinAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin:applovin-sdk:11.6.1",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinSDK",
                Version = "11.6.1",
                Source = ""
            }
        };
    }
}
