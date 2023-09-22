using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class TencentGDTAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {

        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationTencentGDTAdapter",
                Version = "4.12.4.3",
                Source = ""
            }
        };
    }
}
