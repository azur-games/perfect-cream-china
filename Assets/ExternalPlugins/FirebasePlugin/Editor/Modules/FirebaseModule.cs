using Modules.Hive.Editor;


namespace Modules.Firebase.Editor
{
    internal abstract class FirebaseModule
    {
        // Every firebase module should have exactly same version
        public virtual string Version => ModulesConstants.FirebaseVersion;

        public abstract string ModuleName { get; }

        public string PackageManagerDependencyTGZFileFormat =>
            UnityPath.Combine(
                "{0}/Runtime/Dependencies",
                $"{PackageName}-{Version}.tgz");

        public string PackageManagerDependencyFormat =>
            UnityPath.Combine(
                UnityPath.ProjectPath,
                "Packages",
                $"{PackageName}@{Version}");

        public string PackageManagerDependencyWithoutVersionFormat =>
            UnityPath.Combine(
                UnityPath.ProjectPath,
                "Packages",
                PackageName + "@{0}");

        public string PackageManagerPackageFormat =>
            UnityPath.Combine(
                PackageManagerDependencyFormat,
                "package");


        public abstract string PackageName { get; }

        public abstract string AarFileName { get; }

        public abstract string PreprocessorDirectiveName { get; }
    }
}
