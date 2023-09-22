using Modules.General.Editor;
using Modules.Hive.Editor;


namespace AppLovinMax.Scripts.Editor
{
    public class MaxSdkScriptsPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static MaxSdkScriptsPluginHierarchy instance;

        #endregion



        #region Instancing

        public static MaxSdkScriptsPluginHierarchy Instance =>
            instance ?? (instance = new MaxSdkScriptsPluginHierarchy("MaxSdk.Scripts"));


        public MaxSdkScriptsPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}