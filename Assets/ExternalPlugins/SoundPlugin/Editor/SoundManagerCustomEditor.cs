#if UNITY_EDITOR
using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SoundManager))]
class SoundManagerEditor : Editor
{
	const string AUDIO_CLIP_DIRECTORY = "Assets/Resources/Audio";
	const string RESOURCES_SOUNDS_DIRECTORY = "Assets/Resources/Sounds/Configs";
	const string RESOURCES_CONTAINERS_DIRECTORY = "Assets/Resources/Sounds/Containers";
	const string CONTAINER_NAME_PREFIX = "SoundContainer";
	const string AUDIO_KEYS_PATH = "Assets/Scripts/GameFlow/Tags/Keys/AudioKeys.cs";


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Space(40.0f);

		if (GUILayout.Button("Update " + RESOURCES_SOUNDS_DIRECTORY + " directory and fill containers"))
		{
			CreateNonExistingConfigs();
            RemoveUnusedConfigs();
            FillContainers();
		}
	}


	void CreateNonExistingConfigs()
	{
        Dictionary<string, AudioClip> audioClips = GetAudioClips();
        AssetDatabase.StartAssetEditing();

		if (audioClips != null)
		{
			Directory.CreateDirectory(RESOURCES_SOUNDS_DIRECTORY);

            IEnumerable<SoundConfig> existingConfigs = GetExistingConfigsWithPaths().Values;

			// Creating needed configs
			foreach (var clipPair in audioClips)
			{
                if (existingConfigs != null && existingConfigs.Any(config => config.Clip == clipPair.Value))
				{
					continue;
				}

				GameObject configObject = new GameObject();
				
				SoundConfig newConfig = configObject.AddComponent<SoundConfig>();

                newConfig.Init(clipPair.Value);

                string assetPath = clipPair.Key.Replace(AUDIO_CLIP_DIRECTORY, RESOURCES_SOUNDS_DIRECTORY);
                assetPath = assetPath.Remove(assetPath.LastIndexOf('.'));
                assetPath = assetPath + ".prefab";

                Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

				PrefabUtility.CreatePrefab(assetPath, configObject);

				DestroyImmediate(configObject);
			}
		}

        AssetDatabase.SaveAssets();
		AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
	}


    void FillContainers()
	{
		if (!Directory.Exists(RESOURCES_CONTAINERS_DIRECTORY))
		{
			Directory.CreateDirectory(RESOURCES_CONTAINERS_DIRECTORY);
		}

		if (File.Exists(AUDIO_KEYS_PATH))
		{
			File.Delete(AUDIO_KEYS_PATH);
		}
		
		StreamWriter outfile = new StreamWriter(AUDIO_KEYS_PATH);
		
		outfile.WriteLine("public class AudioKeys");
		outfile.WriteLine("{");
		
		foreach (string dir in Directory.GetDirectories(RESOURCES_SOUNDS_DIRECTORY))
		{
			string dirName = dir.Split('/').Last();
			string containerFullName = string.Format("{0}{1}", CONTAINER_NAME_PREFIX, dirName);
			outfile.WriteLine(string.Format("public class {0}", dirName));
			outfile.WriteLine("{");
			outfile.WriteLine("public const string NONE = \"\";", "None", "\"\"");

			string assetPath = string.Format("{0}/{1}", RESOURCES_CONTAINERS_DIRECTORY, containerFullName);
			assetPath = assetPath + ".asset";

			SoundConfigsContainer container = CreateInstance<SoundConfigsContainer>();
            container.SoundConfigsAssetsLink = new List<AssetLink>();
			
			PerformActionOnFilesRecursively(dir, fileName =>
			{
				Debug.Log(fileName);

				SoundConfig config = AssetDatabase.LoadAssetAtPath<SoundConfig>(fileName);
				
				if (config != null)
				{
					AssetLink containedLink = container.SoundConfigsAssetsLink.Find((link) => link.Name.Equals(config.name));

					if (containedLink == null)
					{
						AssetLink configLink = new AssetLink(config);
						string fileCapitalName = configLink.Name.ToUpper();
						fileCapitalName.Replace('-', '_');
						string soundName = string.Format("\"{0}\"", configLink.Name);

						string resultString = string.Format("public const string {0} = {1};", fileCapitalName, soundName);

						outfile.WriteLine(resultString);
						container.SoundConfigsAssetsLink.Add(configLink);
					}                  
				}               
			});
			outfile.WriteLine("}");

			
			AssetDatabase.CreateAsset(container, assetPath);	
		}
		outfile.WriteLine("}");

		outfile.Close();
		AssetDatabase.Refresh();
	}



	void RemoveUnusedConfigs()
	{
        Dictionary<string, SoundConfig> existingConfigsWithPaths = GetExistingConfigsWithPaths();

		if (existingConfigsWithPaths != null)
		{
            Dictionary<string, AudioClip> audioClips = GetAudioClips();

			foreach (var configWithPath in existingConfigsWithPaths)
			{
                if (audioClips == null || !audioClips.Any(clip => clip.Value == configWithPath.Value.Clip))
				{
					AssetDatabase.DeleteAsset(configWithPath.Key);
 				}
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}


    Dictionary<string, AudioClip> GetAudioClips()
	{
        Dictionary<string, AudioClip> clips = null;

		if (Directory.Exists(AUDIO_CLIP_DIRECTORY))
		{
            clips = new Dictionary<string, AudioClip>();

            PerformActionOnFilesRecursively(AUDIO_CLIP_DIRECTORY, fileName =>
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(fileName);

                if (clip != null)
                {
                    clips.Add(fileName, clip);
                }
            });
		}

        return clips;
	}


    Dictionary<string, SoundConfig> GetExistingConfigsWithPaths()
	{
        Dictionary<string, SoundConfig> existingConfigs = null;

		if (Directory.Exists(RESOURCES_SOUNDS_DIRECTORY))
		{
            existingConfigs = new Dictionary<string, SoundConfig>();
            PerformActionOnFilesRecursively(RESOURCES_SOUNDS_DIRECTORY, configFileName =>
            {
                SoundConfig boundAsset = AssetDatabase.LoadAssetAtPath<SoundConfig>(configFileName);
                if (boundAsset != null)
                {
                    existingConfigs.Add(configFileName, boundAsset);
                }
            });
		}

        return existingConfigs;
	}


    void PerformActionOnFilesRecursively(string rootDir, Action<string> action)
    {
        foreach (string dir in Directory.GetDirectories(rootDir))
        {
            PerformActionOnFilesRecursively(dir, action);
        }

        foreach (string file in Directory.GetFiles(rootDir))
        {
            action(file);
        }
    }
}

#endif
