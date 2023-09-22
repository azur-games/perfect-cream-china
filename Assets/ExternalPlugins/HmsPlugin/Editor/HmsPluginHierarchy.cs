using Modules.Hive.Editor;

namespace Modules.HmsPlugin.Editor
{
    public class HmsPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static HmsPluginHierarchy instance;

        #endregion



        #region Properties

        public static HmsPluginHierarchy Instance =>
            instance ?? (instance = new HmsPluginHierarchy("Modules.HmsPlugin"));

        
        public string HmsPath =>
            UnityPath.Combine(RootAssetPath, "ExternalDependencies", "hms-unity-plugin");


        public string AarPath => UnityPath.Combine(HmsPath, "Plugins", "Android", "app-debug.aar");
        
        
        public string DllPath => UnityPath.Combine(HmsPath, "Huawei", "Dlls", "HuaweiMobileServices.dll");

        #endregion



        #region Class lifecycle

        public HmsPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}