namespace Modules.Firebase.Editor
{
    internal class InstallationsModule : FirebaseModule
    {
        public override string ModuleName => "Installations";
        public override string PackageName => "com.google.firebase.installations";
        public override string PreprocessorDirectiveName => "FIREBASE_INSTALLATIONS";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-installations-unity:{Version}");
    }
}
