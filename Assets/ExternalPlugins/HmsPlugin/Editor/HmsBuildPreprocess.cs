using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using System.IO;


namespace Modules.HmsPlugin.Editor
{
    internal class HmsBuildPreprocess : IBuildPreprocessor<IAndroidBuildPreprocessorContext>, IBuildPreprocessor<IIosBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            #if HIVE_HUAWEI
                void RemoveDirectory(string path)
                {
                    if (Directory.Exists(path))
                    {
                        ProjectSnapshot.CurrentSnapshot.SaveDirectoryStructure(path);
                        Directory.Delete(path, true);
                    }
                }

                RemoveDirectory(UnityPath.Combine(HuaweiMobileServicesPluginHierarchy.Instance.RootPath, "Demos"));
                RemoveDirectory(UnityPath.Combine(HuaweiMobileServicesPluginHierarchy.Instance.RootPath, "Resources"));
            #else
                PluginDisabler.CurrentDisabler.PreparePluginForRemoval(HmsPluginHierarchy.Instance);
            #endif
        }
        
        
        public void OnPreprocessBuild(IIosBuildPreprocessorContext context)
        {
            PluginDisabler.CurrentDisabler.PreparePluginForRemoval(HmsPluginHierarchy.Instance);
        }
    }
}