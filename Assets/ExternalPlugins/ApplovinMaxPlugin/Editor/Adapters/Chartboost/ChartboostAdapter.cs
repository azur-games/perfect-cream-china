using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class ChartboostAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:chartboost-adapter:8.4.2.0",
                Repository = ""
            },
            new AndroidPackage()
            {
                Spec = "com.google.android.gms:play-services-base:16.1.0",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationChartboostAdapter",
                Version = "8.5.0.3",
                Source = ""
            }
        };
    }
}
