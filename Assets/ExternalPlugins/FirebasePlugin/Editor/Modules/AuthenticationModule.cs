namespace Modules.Firebase.Editor
{
    internal class AuthenticationModule : FirebaseModule
    {
        public override string ModuleName => "Authentication";
        public override string PackageName => "com.google.firebase.auth";
        public override string PreprocessorDirectiveName => "FIREBASE_AUTHENTICATION";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-auth-unity:{Version}");
    }
}
