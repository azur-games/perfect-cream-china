using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Ios;
using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
#if FIREBASE_ANALYTICS && UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEngine;
#endif


namespace Modules.Firebase.Editor.BuildProcess
{
    internal class FirebaseBuildProcess :
        IPreprocessorDirectivesConfigurator,
        IBuildPostprocessor<IIosBuildPostprocessorContext>,
        IBuildPreprocessor<IAndroidBuildPreprocessorContext>,
        IBuildPostprocessor<IAndroidBuildPostprocessorContext>,
        IBuildFinalizer
    {
        private static string FirebasePluginsAssetPath => UnityPath.Combine(UnityPath.PluginsAssetPath, "Firebase");


        public void OnConfigurePreprocessorDirectives(IPreprocessorDirectivesCollection directives)
        {
            List<FirebaseModule> modules = new List<FirebaseModule>(
                FirebaseConfigurator.ModulesInfo.Select(entry => entry.module));
            modules.Add(FirebaseConfigurator.CoreModule);

            List<string> dependencies = UnityPackagesManifest.Open().GetDependencies();
            foreach (FirebaseModule module in modules)
            {
                if (dependencies.Contains(module.PackageName))
                {
                    directives.Add(module.PreprocessorDirectiveName);
                }
            }
        }


        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            FirebasePackagesManifest packagesManifest = FirebasePackagesManifest.Open();

            List<FirebaseModule> allFirebaseModules =
                new List<FirebaseModule>(
                    FirebaseConfigurator.ModulesInfo.Select(moduleEntry => moduleEntry.module)
                );

            allFirebaseModules.Add(FirebaseConfigurator.CoreModule);

            List<FirebaseModule> enabledModules =
                allFirebaseModules.Where(module =>
                    packagesManifest.Dependencies.Any(dependency => dependency.Key == module.PackageName))
                .ToList();

            if (enabledModules.Count() == 0)
            {
                return;
            }

            Directory.CreateDirectory(FirebasePluginsAssetPath);

            foreach (FirebaseModule module in enabledModules)
            {
                ProjectSnapshot.CurrentSnapshot.SaveDirectoryStructure(module.PackageManagerDependencyFormat);

                List<FileInfo> aarDependenciesFiles =
                    FileSystemUtilities.EnumerateFiles(
                        UnityPath.GetFullPathFromAssetPath(module.PackageManagerDependencyFormat),
                        "*.srcaar",
                        SearchOption.AllDirectories)
                    .ToList();

                foreach (FileInfo aarFile in aarDependenciesFiles)
                {
                    FileSystemUtilities.Move(
                        aarFile.FullName,
                        Path.ChangeExtension(UnityPath.Combine(FirebasePluginsAssetPath, aarFile.Name), ".aar"),
                        FileSystemOperationOptions.Override
                    );
                }

                List<FileInfo> pomDependenciesFiles =
                    FileSystemUtilities.EnumerateFiles(
                        UnityPath.GetFullPathFromAssetPath(module.PackageManagerDependencyFormat),
                        "*.pom",
                        SearchOption.AllDirectories)
                    .ToList();

                foreach (FileInfo pomFile in pomDependenciesFiles)
                {
                    FileSystemUtilities.Move(
                        pomFile.FullName,
                        UnityPath.Combine(FirebasePluginsAssetPath, pomFile.Name),
                        FileSystemOperationOptions.Override
                    );
                }

                DirectoryInfo m2repositoryDirectory =
                    FileSystemUtilities.EnumerateDirectories(
                            UnityPath.GetFullPathFromAssetPath(module.PackageManagerDependencyFormat),
                            "m2repository",
                            SearchOption.AllDirectories)
                        .FirstOrDefault();

                if (m2repositoryDirectory != null)
                {
                    FileSystemUtilities.Delete(m2repositoryDirectory.FullName);
                }

                var xmlDependenciesFiles =
                    FileSystemUtilities.EnumerateFiles(
                        UnityPath.GetFullPathFromAssetPath(module.PackageManagerDependencyFormat),
                        "*Dependencies.xml",
                        SearchOption.AllDirectories);

                foreach (var xmlDependency in xmlDependenciesFiles)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(xmlDependency.FullName);

                    XmlNodeList androidPackages = xmlDocument.SelectNodes("dependencies/androidPackages/androidPackage");

                    for (int i = 0; i < androidPackages.Count; i++)
                    {
                        XmlNode xmlNode = androidPackages[i];

                        if (xmlNode.HasChildNodes)
                        {
                            XmlAttribute specAttribut = xmlNode.Attributes["spec"];

                            if (specAttribut != null && specAttribut.Value == module.AarFileName)
                            {
                                xmlNode.ParentNode.RemoveChild(xmlNode);
                                break;
                            }
                        }
                    }
                    xmlDocument.Save(xmlDependency.FullName);
                }
            }

            AssetDatabase.Refresh();
        }


        public void OnPostprocessBuild(IAndroidBuildPostprocessorContext context)
        {
            if (Directory.Exists(FirebasePluginsAssetPath))
            {
                Directory.Delete(FirebasePluginsAssetPath, true);
            }
        }


        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            #if FIREBASE_CRASHLYTICS
                if (context.IsCiBuild && context.BuildSettings.ContainsKey(BuildSetting.SymbolsFileNecessity))
                {
                    const string shellScript = "find \"${BUILT_PRODUCTS_DIR}\" -name \"*.dSYM\" | " +
                        "xargs -I \\{\\} \"${PROJECT_DIR}/Pods/FirebaseCrashlytics/upload-symbols\" " +
                        "-gsp \"${PROJECT_DIR}/GoogleService-Info.plist\" -p ios \\{\\}";

                    context.PbxProject.AddShellScriptBuildPhase(
                        "Crashlytics dSYM upload",
                        null,
                        shellScript,
                        PbxProjectTargetType.Main);
                }
            #endif

            #if FIREBASE_ANALYTICS && UNITY_IOS
                if (Debug.isDebugBuild)
                {
                    string schemePath = context.PbxProject.XcodeProjectPath + "/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
                    XcScheme xcscheme = new XcScheme();
                    xcscheme.ReadFromFile(schemePath);
                    xcscheme.AddArgumentPassedOnLaunch("-FIRDebugEnabled");
                    xcscheme.WriteToFile(schemePath);

                    string unityPath = FirebasePluginHierarchy.Instance.GetPlatformPath(PlatformAlias.Ios);
                    string xcodePath = context.GetDestinationPath("Firebase");

                    FileSystemUtilities.ExtractArchive(UnityPath.Combine(unityPath, "FirebaseDebugExtention.zip"), xcodePath);
                    context.PbxProject.AddItemsRecursively(xcodePath);
                }
            #endif
        }


        public void OnFinalizeBuild(BuildPipelineContext context)
        {
            #if FIREBASE_CORE
                if (context.BuildOptions.targetGroup == BuildTargetGroup.Android)
                {
                    // We should delete local maven repository, because in other case
                    // it leads to incorrect Android build (without necessary dlls and firebase settings)
                    string localMavenRepositoryDirectoryAssetPath = UnityPath.Combine(
                        ReflectionHelper.GetPropertyValue<string>(
                            typeof(GooglePlayServices.SettingsDialog),
                            null,
                            "LocalMavenRepoDir"),
                        "Firebase");
                    FileSystemUtilities.DeleteEntryAndEmptyParentsDirectories(
                        UnityPath.GetFullPathFromAssetPath(localMavenRepositoryDirectoryAssetPath));
                }
            #endif
        }
    }
}
