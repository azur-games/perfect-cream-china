using System;
using HmsPlugin;
using System.IO;
using Modules.HmsPlugin.Settings;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

namespace Modules.HmsPlugin.Editor
{
    public class HMSGradleFixer : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 1;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string fileName = "agconnect-services.json";
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            string destPath = "";
            destPath = Path.Combine(Directory.GetParent(path).FullName + "//launcher", fileName);
            
            try
            {
                string hmsMainTemplatePath = Application.dataPath + "/Plugins/Android/hmsMainTemplate.gradle";
                if (File.Exists(hmsMainTemplatePath))
                {
                    FileUtil.CopyFileOrDirectory(hmsMainTemplatePath, Path.GetFullPath(path) + @"/hmsMainTemplate.gradle");
                    using (var writer = File.AppendText(Path.GetFullPath(path) + "/build.gradle"))
                        writer.WriteLine("apply from: 'hmsMainTemplate.gradle'");
                }
                else
                {
                    Debug.Log($"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] no file exists at {hmsMainTemplatePath}");
                }

            }
            catch (Exception e)
            {
                Debug.Log($"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] Error hmsMainTemplatePath : {e.Message}");
            }

            try
            {
                string launcherTemplatePath = Application.dataPath + "/Plugins/Android/hmsLauncherTemplate.gradle";
                if (File.Exists(launcherTemplatePath))
                {
                    FileUtil.CopyFileOrDirectory(launcherTemplatePath, Directory.GetParent(path).FullName + @"/launcher/hmsLauncherTemplate.gradle");
                    using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/launcher/build.gradle"))
                        writer.WriteLine("apply from: 'hmsLauncherTemplate.gradle'");
                }
                else
                {
                    Debug.Log($"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] no file exists at {launcherTemplatePath}");
                }

            }
            catch (Exception e)
            {
                Debug.Log($"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] Error launcherTemplatePath : {e.Message}");
            }

            try
            {
                string baseProjectTemplatePath = Application.dataPath + "/Plugins/Android/hmsBaseProjectTemplate.gradle";
                if (File.Exists(baseProjectTemplatePath))
                {
                    FileUtil.CopyFileOrDirectory(baseProjectTemplatePath, Directory.GetParent(path).FullName + @"/hmsBaseProjectTemplate.gradle");
                    using (var writer = File.AppendText(Directory.GetParent(path).FullName + "/build.gradle"))
                        writer.WriteLine("apply from: 'hmsBaseProjectTemplate.gradle'");
                }
                else
                {
                    Debug.Log($"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] no file exists at {baseProjectTemplatePath}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] Error baseProjectTemplatePath : {e.Message}");
            }

            try
            {
                if (File.Exists(destPath))
                {
                    FileUtil.DeleteFileOrDirectory(destPath);
                }

                if (File.Exists(filePath))
                {
                    FileUtil.CopyFileOrDirectory(filePath, destPath);
                }
            }
            catch (Exception e)
            {
                Debug.Log(
                    $"[HMSGradleFixer - OnPostGenerateGradleAndroidProject] Error {destPath} or {filePath} : {e.Message}");
            }
        }
    }
}