using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class YandexAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:yandex-adapter:4.5.0.3",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationYandexAdapter",
                Version = "5.2.1.0",
                Source = ""
            }
        };
    }
}
