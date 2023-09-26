using Modules.Hive.Editor;


namespace Modules.DoTween.Editor
{
    public class DoTweenPluginHierarchy
    {
        private static DoTweenPluginHierarchy instance;
        
        
        public static DoTweenPluginHierarchy Instance =>
            instance ?? (instance = new DoTweenPluginHierarchy("Modules.DOTween"));


        public static string DoTweenSettingsDirectoryAssetPath => "";
        
        
        private DoTweenPluginHierarchy(string mainAssemblyName) : base() { }
    }
}
