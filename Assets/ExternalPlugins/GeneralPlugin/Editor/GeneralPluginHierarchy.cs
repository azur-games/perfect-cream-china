using Modules.Hive.Editor;


namespace Modules.General.Editor
{
    public class GeneralPluginHierarchy
    {
        private static GeneralPluginHierarchy instance;
        
        public static GeneralPluginHierarchy Instance => 
            instance ?? (instance = new GeneralPluginHierarchy("Modules.General"));
        
        private GeneralPluginHierarchy(string mainAssemblyName) : base() { }
    }
}
