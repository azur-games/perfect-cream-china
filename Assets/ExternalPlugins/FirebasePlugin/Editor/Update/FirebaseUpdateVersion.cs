using Modules.Hive.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;


namespace Modules.Firebase.Editor
{
    public class FirebaseUpdateVersion
    {
        internal static string PackagePath =>
            (FirebasePluginHierarchy.Instance.PackageInfo == null)
                ? UnityPath.Combine(UnityPath.ProjectPath, FirebasePluginHierarchy.Instance.RootAssetPath)
                : FirebasePluginHierarchy.Instance.PackageInfo.resolvedPath;


        [MenuItem("Modules/Firebase/Update version")]
        private static void Initialize()
        {
            string newFirebaseVersion = ModulesConstants.FirebaseVersion;

            FirebasePackagesManifest packagesManifest = FirebasePackagesManifest.Open();
            // Made in case the dependencies will be updated not in general, but separately.
            var updatePackages =
                packagesManifest.Dependencies.Where(dependency => dependency.Value != newFirebaseVersion);

            List<FirebaseModule> allFirebaseModules =
                new List<FirebaseModule>(FirebaseConfigurator.ModulesInfo.Select(moduleEntry => moduleEntry.module));

            allFirebaseModules.Add(FirebaseConfigurator.CoreModule);

            List<FirebaseModule> updateFirebaseModules =
                allFirebaseModules.Where(wh => updatePackages.Any(dependency => dependency.Key == wh.PackageName))
                                  .ToList();

            foreach (FirebaseModule module in updateFirebaseModules)
            {
                string oldVersion =
                    packagesManifest.Dependencies.First(dependency => dependency.Key == module.PackageName).Value;

                // delete old
                packagesManifest.RemoveDependency(module.PackageName);

                FileSystemUtilities.Delete(
                    string.Format(module.PackageManagerDependencyWithoutVersionFormat, oldVersion));

                // append new
                FileSystemUtilities.ExtractArchive(
                    string.Format(module.PackageManagerDependencyTGZFileFormat, PackagePath),
                    module.PackageManagerDependencyFormat);

                if (Directory.Exists(module.PackageManagerPackageFormat))
                {
                    FileSystemUtilities.Move(
                        module.PackageManagerPackageFormat,
                        module.PackageManagerDependencyFormat,
                        FileSystemOperationOptions.Override
                        );

                    FileSystemUtilities.Delete(module.PackageManagerPackageFormat);
                }

                packagesManifest.AddDependency(module.PackageName, module.Version);
            }

            packagesManifest.Save();
        }
    }
}