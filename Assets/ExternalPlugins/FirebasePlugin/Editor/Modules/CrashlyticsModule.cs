namespace Modules.Firebase.Editor
{
    internal class CrashlyticsModule : FirebaseModule
    {
        public override string ModuleName => "Crashlytics";
        public override string PackageName => "com.google.firebase.crashlytics";
        public override string PreprocessorDirectiveName => "FIREBASE_CRASHLYTICS";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-crashlytics-unity:{Version}");
    }
}
