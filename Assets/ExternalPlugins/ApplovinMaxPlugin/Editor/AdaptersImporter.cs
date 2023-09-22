using Modules.General.Editor;
using Modules.Hive.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;


namespace Modules.Max.Editor
{
    public static class AdaptersImporter
    {
        private const string DependenciesFilePath = "Editor/Dependencies.xml";
        private const string TemplatePath = "Editor/AdapterTemplate.txt";
        private const string AdaptersPath = "Editor/Adapters";

        private const string AdapterNameTemplate = "{adaptername}";
        private const string AndroidPackagesTemplate = "{androidpackages}";
        private const string IosPodsTemplate = "{iospods}";
        private const string AndroidEntryTemplate = 
@"            new AndroidPackage()
            {
                Spec = ""{spec}"",
                Repository = ""{repo}""
            }";
        private const string IosEntryTemplate = 
@"            new IosPod()
            {
                Name = ""{name}"",
                Version = ""{version}"",
                Source = ""{source}""
            }";
        
        private static string TemplateData { get; set; }
        
        
        [MenuItem("Modules/AppLovin/Import Adapters")]
        private static void ImportMediations()
        {
            string mediationsPath = EditorUtility.OpenFolderPanel("Select Adapters Folder", "", "Mediation");

            TemplateData =
                File.ReadAllText(UnityPath.Combine(ApplovinMaxPluginHierarchy.Instance.RootPath, TemplatePath));

            string[] folders = Directory.GetDirectories(mediationsPath);
            foreach (string folder in folders)
            {
                ProcessAdapter(folder);
            }
            
            AssetDatabase.Refresh();
        }


        private static void ProcessAdapter(string path)
        {
            string adapterName = Path.GetFileName(path);
            Debug.Log("Importing " + adapterName);
            
            string template = TemplateData.Replace(AdapterNameTemplate, $"{adapterName}Adapter");
            
            XmlDocument doc = new XmlDocument();
            doc.Load(UnityPath.Combine(path, DependenciesFilePath));
            var androidPackages = doc.SelectNodes("dependencies/androidPackages");
            var iosPods = doc.SelectNodes("dependencies/iosPods");
            
            var androidData = "";
            if (androidPackages.Count > 0)
            {
                androidData = GetAndroidDependencies(androidPackages[0]);
            }
            template = template.Replace(AndroidPackagesTemplate, androidData);
            
            var iosData = "";
            if (iosPods.Count > 0)
            {
                iosData = GetIosDependencies(iosPods[0]);
            }
            template = template.Replace(IosPodsTemplate, iosData);

            string filePath = UnityPath.Combine(ApplovinMaxPluginHierarchy.Instance.RootPath, AdaptersPath, adapterName);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = UnityPath.Combine(filePath, $"{adapterName}Adapter.cs");

            File.WriteAllText(filePath, template);
        }


        private static string GetAndroidDependencies(XmlNode node)
        {
            var packages = node.SelectNodes("androidPackage");
            var mainRepositories = node.SelectNodes("repositories/repository");

            string mainRepository = mainRepositories.Count > 0 ? mainRepositories[0].InnerText : "";

            List<string> entries = new List<string>();
            for (int i = 0; i < packages.Count; i++)
            {
                string spec = packages[i].Attributes["spec"].Value;
                
                var repositories = packages[i].SelectNodes("repositories/repository");
                string repository = repositories.Count > 0 ? repositories[0].InnerText : mainRepository;
                
                string data = AndroidEntryTemplate
                    .Replace("{spec}", spec)
                    .Replace("{repo}", repository);
                
                entries.Add(data);
            }
            
            return string.Join("," + Environment.NewLine, entries.ToArray());
        }


        private static string GetIosDependencies(XmlNode node)
        {
            var packages = node.SelectNodes("iosPod");
            
            List<string> entries = new List<string>();
            for (int i = 0; i < packages.Count; i++)
            {
                string name = packages[i].Attributes["name"].Value;
                string version = packages[i].Attributes["version"].Value;
                
                var repositories = packages[i].SelectNodes("sources/source");
                string repository = repositories.Count > 0 ? repositories[0].InnerText : null;
                
                string data = IosEntryTemplate
                    .Replace("{name}", name)
                    .Replace("{version}", version)
                    .Replace("{source}", repository);
                
                entries.Add(data);
            }
            
            return string.Join("," + Environment.NewLine, entries.ToArray());
        }
    }
}
