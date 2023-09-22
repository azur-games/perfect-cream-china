using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ContentSource))]
public sealed class ContentSourceInspector : Editor
{
    public override void OnInspectorGUI()
    {
        ContentSource contentSource = (target as ContentSource);

        bool hasChanges = false;

        // TARGET DIRECTORY =>
        EditorGUILayout.BeginHorizontal();

        string path = contentSource.TargetDirectory;
        if (string.IsNullOrEmpty(path)) path = "<<EMPTY>>";
        path += " => ";
        EditorGUILayout.LabelField("Destination : " + path);

        EditorGUI.BeginChangeCheck();
        DefaultAsset targetDirectoryAsset = (DefaultAsset)EditorGUILayout.ObjectField("", null, typeof(DefaultAsset), false, GUILayout.Width(100.0f));
        if (EditorGUI.EndChangeCheck())
        {
            path = (null == targetDirectoryAsset) ? null : AssetDatabase.GetAssetPath(targetDirectoryAsset);
            contentSource.TargetDirectory = path;
            hasChanges = true;
        }

        EditorGUILayout.EndHorizontal();
        // <= TARGET DIRECTORY

        if (GUILayout.Button("Prepare content items"))
        {
            if (ContentPreparer.PrepareContentItems(contentSource, out string errorString))
            {
                EditorUtility.DisplayDialog("Complete!", "Content items prepared!", " Ok");
            }
            else
            {
                EditorUtility.DisplayDialog("Error!", "Something went wrong : \n" + errorString, " Ok");
            }
        }

        if (hasChanges)
        {
            EditorUtility.SetDirty(contentSource);
        }
    }
}
