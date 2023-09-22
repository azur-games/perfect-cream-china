using Modules.Hive.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;


namespace Modules.Firebase.Editor
{
    [Serializable]
    public class FirebasePackagesManifest
    {
        #region Fields

        private static string PackagesManifestPath => UnityPath.Combine(
            UnityPath.PluginsAssetPath,
            "FirebasePackagesManifest.json");

        #endregion



        #region Properties

        [JsonProperty] public Dictionary<string, string> Dependencies { get; private set; } = 
            new Dictionary<string, string>();

        #endregion



        #region Instancing

        public static FirebasePackagesManifest Open()
        {
            if (!Directory.Exists(UnityPath.PluginsAssetPath))
            {
                Directory.CreateDirectory(UnityPath.PluginsAssetPath);
            }
            
            if (!File.Exists(PackagesManifestPath))
            {
                var firebasePackagesManifest = new FirebasePackagesManifest();
                firebasePackagesManifest.Save();
            }

            return JsonConvert.DeserializeObject<FirebasePackagesManifest>(
                File.ReadAllText(PackagesManifestPath),
                GetSerializerSettings());
        }


        private FirebasePackagesManifest() { }

        #endregion



        #region Methods

        public bool AddDependency(string packageName, string packageVersion)
        {
            if (Dependencies.ContainsKey(packageName))
            {
                return false;
            }

            Dependencies[packageName] = packageVersion;
            return true;
        }


        public bool RemoveDependency(string packageName) => Dependencies.Remove(packageName);


        public List<string> GetDependencies()
        {
            return Dependencies.Keys.ToList();
        }


        public void Save()
        {
            JsonSerializer serializer = JsonSerializer.Create(GetSerializerSettings());

            using (var file = File.CreateText(PackagesManifestPath))
            using (var writer = new JsonTextWriter(file))
            {
                writer.Indentation = 2;
                writer.IndentChar = ' ';
                serializer.Serialize(writer, this);
            }

            AssetDatabase.Refresh();
        }


        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        #endregion
    }
}
