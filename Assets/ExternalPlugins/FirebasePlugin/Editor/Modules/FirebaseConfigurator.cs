using Modules.Hive.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace Modules.Firebase.Editor
{
    internal class FirebaseConfigurator : EditorWindow
    {
        #region Nested types

        internal class ModuleEntry
        {
            public FirebaseModule module;
            public bool wasEnabled;
            public bool isEnabled;
            public FirebaseModule[] dependencies;


            public ModuleEntry(
                FirebaseModule firebaseModule,
                bool isModuleEnabled,
                params FirebaseModule[] moduleDependencies)
            {
                module = firebaseModule;
                wasEnabled = isModuleEnabled;
                isEnabled = isModuleEnabled;
                dependencies = moduleDependencies;
            }
        }

        #endregion



        #region Fields

        internal static readonly FirebaseModule CoreModule = new CoreModule();

        internal static readonly List<ModuleEntry> ModulesInfo = new List<ModuleEntry>()
        {
            new ModuleEntry(new AnalyticsModule(), false),
            new ModuleEntry(new AuthenticationModule(), false),
            new ModuleEntry(new CrashlyticsModule(), false),
            new ModuleEntry(new DatabaseModule(), false, new AuthenticationModule()),
            new ModuleEntry(new RemoteConfigModule(), false),
            new ModuleEntry(new InstallationsModule(), false)
        };

        internal static string PackagePath =>
            (FirebasePluginHierarchy.Instance.PackageInfo == null)
                ? UnityPath.Combine(UnityPath.ProjectPath, FirebasePluginHierarchy.Instance.RootAssetPath)
                : FirebasePluginHierarchy.Instance.PackageInfo.resolvedPath;

        private static bool modulesIsApplied = false;

        #endregion



        #region Unity lifecycle

        private void OnFocus()
        {
            RefreshModulesAvailability();
        }


        private void OnGUI()
        {
            foreach (ModuleEntry entry in ModulesInfo)
            {
                EditorGUILayout.BeginHorizontal();

                entry.isEnabled = EditorGUILayout.ToggleLeft(entry.module.ModuleName, entry.isEnabled);

                EditorGUILayout.EndHorizontal();
            }

            bool isChangeDetected = ModulesInfo.Exists(entry => entry.wasEnabled != entry.isEnabled);

            EditorGUI.BeginDisabledGroup(!isChangeDetected);
            if (GUILayout.Button("Apply"))
            {
                ApplyModulesAvailability();
            }
            EditorGUI.EndDisabledGroup();
        }

        #endregion



        #region Methods

        [MenuItem("Modules/Firebase/Configurator")]
        private static void Initialize()
        {
            FirebaseConfigurator window = (FirebaseConfigurator)GetWindow(typeof(FirebaseConfigurator));
            window.Show();

            RefreshModulesAvailability();
        }


        private static void RefreshModulesAvailability()
        {
            if (modulesIsApplied)
            {
                return;
            }

            var firebasePackagesManifest = FirebasePackagesManifest.Open();
            List<string> dependencies = firebasePackagesManifest.GetDependencies();

            foreach (ModuleEntry moduleEntry in ModulesInfo)
            {
                moduleEntry.isEnabled = dependencies.Exists(
                    dependency => dependency.Equals(moduleEntry.module.PackageName));

                moduleEntry.wasEnabled = moduleEntry.isEnabled;
            }
        }


        private static void ApplyModulesAvailability()
        {
            modulesIsApplied = true;

            bool isAnyModuleAvailable = false;
            bool isModifyModule = false;

            var firebasePackagesManifest = FirebasePackagesManifest.Open();

            foreach (ModuleEntry entry in ModulesInfo)
            {
                isAnyModuleAvailable |= entry.isEnabled;

                if (entry.wasEnabled != entry.isEnabled)
                {
                    isModifyModule |= WasModifyModule(entry.module, entry.isEnabled);
                    entry.wasEnabled = entry.isEnabled;
                }
            }

            WasModifyModule(CoreModule, isAnyModuleAvailable);

            firebasePackagesManifest.Save();

            modulesIsApplied = false;

            bool WasModifyModule(FirebaseModule module, bool isEnabled)
            {
                ModuleEntry entry =
                    ModulesInfo.FirstOrDefault(moduleEntry => moduleEntry.module.PackageName == module.PackageName);

                string preprocessorDirectiveName = module.PreprocessorDirectiveName;

                if (isEnabled)
                {
                    if (entry?.dependencies != null)
                    {
                        foreach (var dependence in entry.dependencies)
                        {
                            WasModifyModule(dependence, true);
                        }
                    }

                    if (firebasePackagesManifest.Dependencies.Any(dependency => dependency.Key == module.PackageName) &&
                        Directory.Exists(module.PackageManagerDependencyFormat))
                    {
                        return false;
                    }

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

                    firebasePackagesManifest.AddDependency(module.PackageName, module.Version);

                    PreprocessorDirectivesUtilities.AddPreprocessorDirectives(
                        BuildTargetGroup.Android,
                        preprocessorDirectiveName);
                    PreprocessorDirectivesUtilities.AddPreprocessorDirectives(
                        BuildTargetGroup.iOS,
                        preprocessorDirectiveName);
                    PreprocessorDirectivesUtilities.AddPreprocessorDirectives(
                        BuildTargetGroup.Standalone,
                        preprocessorDirectiveName);

                    return true;
                }
                else
                {
                    List<ModuleEntry> modulesUsed =
                        ModulesInfo.Where(moduleEntry => moduleEntry.isEnabled &&
                                                         moduleEntry.dependencies != null &&
                                                         moduleEntry.dependencies.Any(firebaseModule =>
                                                                firebaseModule.PackageName == module.PackageName))
                                   .ToList();

                    if (modulesUsed.Count > 0)
                    {
                        StringBuilder str = new StringBuilder();
                        modulesUsed.ForEach(fe => str.Append(fe.module.ModuleName + ", "));

                        string modulesList = str.ToString().TrimEnd(' ', ',');

                        EditorUtility.DisplayDialog(
                            "ERROR!",
                            $"The {module.ModuleName} module is used in:\n{modulesList}",
                            "OK");

                        return false;
                    }


                    if (entry?.dependencies != null)
                    {
                        foreach (var dependence in entry.dependencies)
                        {
                            bool isModuleInfo =
                                ModulesInfo.Any(moduleInfo => moduleInfo.module.PackageName == dependence.PackageName);

                            if (isModuleInfo)
                            {
                                continue;
                            }

                            bool isDependenceUsed =
                                ModulesInfo.Where(moduleEntry => moduleEntry.isEnabled)
                                           .SelectMany(moduleEntry => moduleEntry.dependencies)
                                           .Any(firebaseModule => firebaseModule.PackageName == dependence.PackageName);

                            if (isDependenceUsed)
                            {
                                continue;
                            }

                            firebasePackagesManifest.RemoveDependency(dependence.PackageName);

                            FileSystemUtilities.Delete(module.PackageManagerDependencyFormat);
                        }
                    }

                    firebasePackagesManifest.RemoveDependency(module.PackageName);

                    FileSystemUtilities.Delete(module.PackageManagerDependencyFormat);

                    PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(
                        BuildTargetGroup.Android,
                        preprocessorDirectiveName);
                    PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(
                        BuildTargetGroup.iOS,
                        preprocessorDirectiveName);
                    PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(
                        BuildTargetGroup.Standalone,
                        preprocessorDirectiveName);

                    return true;
                }
            }
        }

        #endregion
    }
}
