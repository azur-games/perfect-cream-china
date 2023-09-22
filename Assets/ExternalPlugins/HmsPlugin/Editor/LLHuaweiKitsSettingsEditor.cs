using Modules.General.Abstraction;
using Modules.HmsPlugin.Settings;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Modules.HmsPlugin.Editor
{
    [CustomEditor(typeof(LLHuaweiKitsSettings)), CanEditMultipleObjects]
    public class LLHuaweiKitsSettingsEditor : UnityEditor.Editor
    {
        #region Fields

        private ReorderableList modulesList;

        private LLHuaweiKitsSettings settings;

        #endregion



        #region Methods

        public void OnEnable()
        {
            settings = target as LLHuaweiKitsSettings;
            modulesList = new ReorderableList(serializedObject,serializedObject.FindProperty("adModulesInfos"), true, true, true, true);
            modulesList.drawElementCallback = (rect, index, isActive, isFocused) => 
            {
                SerializedProperty element = modulesList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, 50, EditorGUIUtility.singleLineHeight), "Module");
                EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, 150, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("module"), GUIContent.none);
                
                EditorGUI.LabelField(new Rect(rect.x + 220, rect.y, 20, EditorGUIUtility.singleLineHeight), "Id");
                EditorGUI.PropertyField(new Rect(rect.x + 250, rect.y, 300, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("moduleId"), GUIContent.none);
            };
            modulesList.drawHeaderCallback = (rect) => 
            {
                EditorGUI.LabelField(rect, "Ad modules ids");
            };
        }

        
        public override void OnInspectorGUI() 
        {
            base.OnInspectorGUI();
            if (!settings.IsAdsKitEnabled)
            {
                return;
            }
            serializedObject.Update();
                
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Advertising settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            modulesList.DoLayoutList();
                
            if (!string.IsNullOrEmpty(settings.GetModuleId(AdModule.Banner)))
            {
                EditorGUILayout.LabelField("Banner settings", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bannerPosition"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bannerSize"));
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}