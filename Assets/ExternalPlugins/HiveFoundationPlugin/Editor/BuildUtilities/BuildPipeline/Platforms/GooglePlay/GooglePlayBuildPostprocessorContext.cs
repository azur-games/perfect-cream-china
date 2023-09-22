﻿using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class GooglePlayBuildPostprocessorContext : AndroidBuildPostprocessorContext, IGooglePlayBuildPostprocessorContext
    {
        public GooglePlayBuildPostprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            HiveBuildSummary buildProjectSummary) : 
            base(preprocessorDirectives, buildOptions, buildSettings, buildProjectSummary) { }
    }
}
