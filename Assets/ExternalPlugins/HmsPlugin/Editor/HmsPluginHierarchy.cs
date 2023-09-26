using Modules.Hive.Editor;

namespace Modules.HmsPlugin.Editor
{
    public class HmsPluginHierarchy
    {
        #region Fields

        private static HmsPluginHierarchy instance;

        #endregion



        #region Properties

        public static HmsPluginHierarchy Instance =>
            instance ?? (instance = new HmsPluginHierarchy("Modules.HmsPlugin"));

        
        public string HmsPath => "";


        public string AarPath => "";
        
        
        public string DllPath => "";

        #endregion



        #region Class lifecycle

        public HmsPluginHierarchy(string mainAssemblyName) : base() { }

        #endregion
    }
}