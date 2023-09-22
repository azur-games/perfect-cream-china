namespace Modules.Firebase.Editor
{
    internal class CoreModule : FirebaseModule
    {
        public override string ModuleName => "Core";
        public override string PackageName => "com.google.firebase.app";
        public override string PreprocessorDirectiveName => "FIREBASE_CORE";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-app-unity:{Version}");
    }
}
