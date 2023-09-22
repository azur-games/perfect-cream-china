using System.Collections.Generic;
using System.IO;
using HmsPlugin;
using Modules.HmsPlugin.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Modules.HmsPlugin.Editor
{
    /// <summary>
    /// Переделанный ExternalDependencies/hms-unity-plugin/Huawei/Editor/Utils/HMSGradleWorker.cs, работает вместо него
    /// </summary>
    public class HMSGradleWorker : HMSEditorSingleton<HMSGradleWorker>, IPreprocessBuildWithReport
    {
        #region Fields
        
        private Dictionary<string, string[]> gradleSettings;
        public int callbackOrder => 0;

        private string gradleTemplatesPath = EditorApplication.applicationContentsPath + @"\PlaybackEngines\AndroidPlayer\Tools\GradleTemplates";

        private List<string> kitsGradleSettings;

        #endregion



        #region Properties

        private string[] CoreGradles => new[] { "com.huawei.hms:base:5.2.0.300", "com.android.support:appcompat-v7:28.0.0", "com.huawei.agconnect:agconnect-core:1.4.1.300" };

        #endregion



        #region IPreprocessBuildWithReport

        public void OnPreprocessBuild(BuildReport report)
        {
            Application.logMessageReceived += OnBuildError;
            bool isAndroid = report.summary.platform == BuildTarget.Android;
            
            var huaweiMobileServicesDLL = AssetImporter.GetAtPath(HmsPluginHierarchy.Instance.DllPath) as PluginImporter;
            var appDebugAar = AssetImporter.GetAtPath(HmsPluginHierarchy.Instance.AarPath) as PluginImporter;

            if (isAndroid)
            {
                PrepareGradleFile();
            }

            if (huaweiMobileServicesDLL != null)
            {
                huaweiMobileServicesDLL.SetCompatibleWithPlatform(BuildTarget.Android, isAndroid);
            }
            
            if (appDebugAar != null)
            {
                appDebugAar.SetCompatibleWithPlatform(BuildTarget.Android, isAndroid);
            }

            HMSEditorUtils.HandleAssemblyDefinitions(isAndroid);
        }

        #endregion



        #region Methods

        private void CreateGradleFiles(string[] gradleConfigs)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Plugins"))
            {
                AssetDatabase.CreateFolder("Assets", "Plugins");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Plugins/Android"))
            {
                AssetDatabase.CreateFolder("Assets/Plugins", "Android");
            }
#if UNITY_2019_3_OR_NEWER || UNITY_2020
            CreateMainGradleFile(gradleConfigs);
            CreateLauncherGradleFile(gradleConfigs);
            BaseProjectGradleFile();

#elif UNITY_2018_1_OR_NEWER
            CreateMainGradleFile(gradleConfigs);
#endif
            AssetDatabase.Refresh();
        }

        private void CreateMainGradleFile(string[] gradleConfigs)
        {
#if UNITY_2019_3_OR_NEWER || UNITY_2020
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsMainTemplate.gradle"))
            {
                file.Write("dependencies {\n\t");
                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }
                file.Write("}\n");
            }

#elif UNITY_2018_1_OR_NEWER
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsMainTemplate.gradle"))
            {
                file.Write("buildscript {\n\t");
                file.Write("repositories {\n\t\t");
                file.Write("google()\n\t\t");
                file.Write("jcenter()\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n\n\t");
                file.Write("dependencies {\n\t\t");
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.4.2.300"));
                file.Write("\t}\n}\n\n");
                file.Write("allprojects {\n\t");
                file.Write("repositories {\n\t\t");
                file.Write("google()\n\t\t");
                file.Write("jcenter()\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n}\n\n");

                file.WriteLine("apply plugin: 'com.huawei.agconnect'\n");

                file.Write("dependencies {\n\t");
                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }
                file.Write("}\n\n");
            }
#endif
        }

        private void CreateLauncherGradleFile(string[] gradleConfigs)
        {
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsLauncherTemplate.gradle"))
            {
                file.Write("apply plugin: 'com.huawei.agconnect'\n\n");
                file.Write("dependencies {\n\t");

                for (int i = 0; i < gradleConfigs.Length; i++)
                {
                    file.Write(AddDependency(gradleConfigs[i]));
                }

                file.Write("\n}\n");
            }
        }

        private void BaseProjectGradleFile()
        {
            using (var file = File.CreateText(Application.dataPath + "/Plugins/Android/hmsBaseProjectTemplate.gradle"))
            {
                file.Write("allprojects {\n\t");
                file.Write("buildscript {\n\t\t");
                file.Write("repositories {\n\t\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t\t}\n\n\t\t");
                file.Write("dependencies {\n\t\t\t");
                file.Write(AddClasspath("com.huawei.agconnect:agcp:1.4.2.300"));
                file.Write("\n\t\t}\n\t}\n\n\t");
                file.Write("repositories {\n\t\t");
                file.Write("maven { url 'https://developer.huawei.com/repo/' }\n\t}\n}\n\n");
            }
        }

        private string AddDependency(string name)
        {
            return $"implementation '{name}'\n\t";
        }

        private string AddClasspath(string name)
        {
            return $"classpath '{name}'\n\t\t\t";
        }

        private void PrepareGradleFile()
        {
            if (!LLHuaweiKitsSettings.DoesInstanceExist)
            {
                return;
            }
            List<string> gradle = new List<string>(CoreGradles);
            if (LLHuaweiKitsSettings.Instance.IsAdsKitEnabled)
            {
                // AccountKit
                gradle.Add("com.huawei.hms:hwid:5.3.0.302" ); 
                // AdsKit
                gradle.Add("com.huawei.hms:ads-lite:13.4.39.302");
                gradle.Add("com.huawei.hms:ads-consent:3.4.39.302");
                gradle.Add("com.huawei.hms:ads-identifier:3.4.39.302");
            }
            if (LLHuaweiKitsSettings.Instance.IsIAPKitEnabled)
            {
                // IAPKit
                gradle.Add("com.huawei.hms:iap:5.3.0.300");
            }
            CreateGradleFiles(gradle.ToArray());
        }



        private void OnBuildError(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                Application.logMessageReceived -= OnBuildError;
                HMSEditorUtils.HandleAssemblyDefinitions(true);
            }
        }

        #endregion
    }
}

