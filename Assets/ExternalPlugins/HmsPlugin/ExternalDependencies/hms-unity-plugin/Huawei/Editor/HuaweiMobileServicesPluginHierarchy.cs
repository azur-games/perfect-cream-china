using Modules.Hive.Editor;

namespace Modules.HmsPlugin.Editor
{
    public class HuaweiMobileServicesPluginHierarchy
    {
        #region Fields

        private static HuaweiMobileServicesPluginHierarchy instance;

        #endregion



        #region Properties
        
        public static HuaweiMobileServicesPluginHierarchy Instance =>
            instance ?? (instance = new HuaweiMobileServicesPluginHierarchy("HuaweiMobileServices.Editor"));
        
        
        public string CoreAsmdefPath => "";
        
        
        public string VersionPath => "";

        #endregion



        #region Class lifecycle

        public HuaweiMobileServicesPluginHierarchy(string mainAssemblyName) : base() { }

        #endregion



        #region Methods

        public string GetPathWithRoot(string pathInRoot) => "";

        #endregion
    }
}