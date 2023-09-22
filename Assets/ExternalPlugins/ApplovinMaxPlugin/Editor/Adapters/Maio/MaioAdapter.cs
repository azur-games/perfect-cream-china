using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class MaioAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:maio-adapter:1.1.16.1",
                Repository = "https://imobile-maio.github.io/maven"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationMaioAdapter",
                Version = "1.6.1.0",
                Source = ""
            }
        };
    }
}
