using Modules.Hive.Editor;


namespace Modules.Firebase.Editor
{
    public class FirebasePluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static FirebasePluginHierarchy instance;

        #endregion



        #region Properties

        public static FirebasePluginHierarchy Instance =>
            instance ?? (instance = new FirebasePluginHierarchy("Modules.Firebase"));

        #endregion



        #region Class lifecycle

        private FirebasePluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}
