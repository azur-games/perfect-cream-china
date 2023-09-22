namespace Modules.Firebase.Editor
{
    internal class DatabaseModule : FirebaseModule
    {
        public override string ModuleName => "Database";
        public override string PackageName => "com.google.firebase.database";
        public override string PreprocessorDirectiveName => "FIREBASE_DATABASE";
        public override string AarFileName => string.Format($"com.google.firebase:firebase-database-unity:{Version}");
    }
}
