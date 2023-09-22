using Modules.Hive.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace Modules.Hive.Editor
{
    public class ProjectPluginsUtilities
    {
        #region Fields

        private static ProjectPluginsUtilities currentUtilities = null;
        
        private const string PackagesNamePrefix = "com.playgendary";
        private const string PackageJsonFileName = "package.json";
        private const string PackagesListFileName = "packagePluginVersions.json";
        private const string SubmodulesAssetPathRegex = @"^\s*path ?= ?(.*)\s*$";
        
        private List<UnityPackageInfo> packages = new List<UnityPackageInfo>();

        #endregion
      

        
        #region Instancing

        public static ProjectPluginsUtilities CurrentUtilities
        {
            get
            {
                if (currentUtilities == null)
                {
                    currentUtilities = new ProjectPluginsUtilities();
                }

                return currentUtilities;
            }
        }

        #endregion


        
        #region Methods

        public ProjectPluginsUtilities()
        {
            FindAllPlugins();
        }
        
         
        private void FindAllPlugins()
        {
            if (!Directory.Exists(UnityPath.ExternalPluginsSettingsAssetPath))
            {
                return;
            }
            
            string[] directories = Directory.GetDirectories(UnityPath.ExternalPluginsSettingsAssetPath);
            foreach (string directory in directories)
            {
                string packagePath = UnityPath.Combine(directory, PackageJsonFileName);
                if (File.Exists(packagePath))
                {
                    AddPluginToList(packagePath);
                }
            }
        }


        private void AddPluginToList(string packageJsonPath)
        {
            string trimmedPath = String.Concat(packageJsonPath.Where(c => !Char.IsWhiteSpace(c)));

            UnityPackageInfo packageInfo = UnityPackageInfo.Open(trimmedPath);

            if (packageInfo != null && packageInfo.name.StartsWith(PackagesNamePrefix))
            {
                packages.Add(packageInfo);
            }
        }

        
        public void WritePluginsToFile(string filePath)
        {
            string json = JsonConvert.SerializeObject(packages, Formatting.Indented);
            string finalFilePath = UnityPath.Combine(filePath, PackagesListFileName);

            if (File.Exists(finalFilePath))
            {
                File.Delete(finalFilePath);
            }
            
            File.WriteAllText(finalFilePath, json);
        }

        #endregion
       
    }
}
