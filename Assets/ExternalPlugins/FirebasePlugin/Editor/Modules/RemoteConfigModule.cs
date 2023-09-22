namespace Modules.Firebase.Editor
{
    internal class RemoteConfigModule : FirebaseModule
    {
        public override string ModuleName => "Remote Config";
        public override string PackageName => "com.google.firebase.remote-config";
        public override string PreprocessorDirectiveName => "FIREBASE_REMOTE_CONFIG";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-config-unity:{Version}");
    }
}
