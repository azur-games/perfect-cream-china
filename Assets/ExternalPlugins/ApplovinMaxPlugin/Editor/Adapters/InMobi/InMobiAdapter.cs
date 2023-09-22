using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class InMobiAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:inmobi-adapter:10.0.5.5",
                Repository = ""
            },
            new AndroidPackage()
            {
                Spec = "com.squareup.picasso:picasso:2.71828",
                Repository = ""
            },
            new AndroidPackage()
            {
                Spec = "com.android.support:recyclerview-v7:28.+",
                Repository = ""
            },
            new AndroidPackage()
            {
                Spec = "com.android.support:customtabs:28.+",
                Repository = ""
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationInMobiAdapter",
                Version = "10.0.5.3",
                Source = ""
            }
        };
    }
}
