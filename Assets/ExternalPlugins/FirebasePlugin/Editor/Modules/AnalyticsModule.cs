namespace Modules.Firebase.Editor
{
    internal class AnalyticsModule : FirebaseModule
    {
        public override string ModuleName => "Analytics";
        public override string PackageName => "com.google.firebase.analytics";
        public override string PreprocessorDirectiveName => "FIREBASE_ANALYTICS";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-analytics-unity:{Version}");
    }
}
