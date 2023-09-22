﻿using Modules.General.HelperClasses;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
#if HIVE
using Modules.Hive.Editor.BuildUtilities;
#endif


namespace Modules.BuildProcess
{
    public class TargetSettingsBuildProcess
    {
        #if HIVE

        public class PreprocessBuild : IBuildPreprocessor<IBuildPreprocessorContext>
        {
            public void OnPreprocessBuild(IBuildPreprocessorContext context)
            {
                foreach (var pair in ProjectSettingsPathsByTarget)
                {
                    if (pair.Key == context.BuildTarget)
                    {
                        ProjectSnapshot.CurrentSnapshot?.CopyAssetToStash($"Assets/{pair.Value}");
                    }
                    else
                    {
                        ProjectSnapshot.CurrentSnapshot?.MoveAssetToStash($"Assets/{pair.Value}");
                    }
                }
            }
        }
    
        #else
        
        public class PreprocessBuild : IPreprocessBuildWithReport
        {
            public int callbackOrder => -31000;
                
                
            public void OnPreprocessBuild(BuildReport report)
            {
                foreach (BuildTarget key in ProjectSettingsPathsByTarget.Keys)
                {
                    if (key != report.summary.platform)
                    {
                        ReplaceDirectory(key);
                    }
                }
                
                
                void ReplaceDirectory(BuildTarget target)
                {
                    string path = Application.dataPath.AppendPathComponent(ProjectSettingsPathsByTarget[target]);
                    string metaPath = path + ".meta";
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                    }
                }
            }
        }
        
        #endif
        
        
        #region Fields
        
        private static readonly Dictionary<BuildTarget, string> ProjectSettingsPathsByTarget = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.Android, "TargetSettings/Android" },
            { BuildTarget.iOS, "TargetSettings/iOS" }
        };
        
        #endregion
    }
}
