using Modules.Hive.Editor;

namespace Modules.HmsPlugin.Editor
{
    public class HuaweiMobileServicesPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static HuaweiMobileServicesPluginHierarchy instance;

        #endregion



        #region Properties
        
        public static HuaweiMobileServicesPluginHierarchy Instance =>
            instance ?? (instance = new HuaweiMobileServicesPluginHierarchy("HuaweiMobileServices.Editor"));
        
        
        public string CoreAsmdefPath => UnityPath.Combine(RootAssetPath, "HuaweiMobileServices.Core.asmdef");
        
        
        public string VersionPath => UnityPath.Combine(RootAssetPath, "VERSION");

        #endregion



        #region Class lifecycle

        public HuaweiMobileServicesPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion



        #region Methods

        public string GetPathWithRoot(string pathInRoot) => UnityPath.Combine(RootAssetPath, pathInRoot);

        #endregion
    }
}