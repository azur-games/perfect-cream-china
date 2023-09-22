using Modules.Hive.Editor;
using System.Collections.Generic;
using UnityEditor;


namespace Modules.Firebase.ClearData
{
    public class FirebaseClearData
    {
        [MenuItem("Modules/Firebase/Clear old data")]
        private static void Initialize()
        {
            UnityPackagesManifest packagesManifest = UnityPackagesManifest.Open();
            List<string> dependencies = packagesManifest.GetDependencies();

            foreach (var item in dependencies)
            {
                if (item.ToLower().StartsWith("com.google.firebase"))
                {
                    packagesManifest.RemoveDependency(item);
                }
            }

            packagesManifest.Save();

            List<string> preprocessorDirectivesName = new List<string> {
                "FIREBASE_CORE",
                "FIREBASE_ANALYTICS",
                "FIREBASE_AUTHENTICATION",
                "FIREBASE_CRASHLYTICS",
                "FIREBASE_DATABASE",
                "FIREBASE_REMOTE_CONFIG",
                "FIREBASE_INSTALLATIONS"
            };

            foreach (var preprocessorDirectiveName in preprocessorDirectivesName)
            {
                PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(
                    BuildTargetGroup.Android,
                    preprocessorDirectiveName);
                PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(
                    BuildTargetGroup.iOS,
                    preprocessorDirectiveName);
                PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(
                    BuildTargetGroup.Standalone,
                    preprocessorDirectiveName);
            }
        }
    }
}
