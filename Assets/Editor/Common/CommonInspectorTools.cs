using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CommonInspectorTools
{
    public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
        fi.SetValue(obj, val);
    }

    public static void DrawInspectorField(SerializedObject serializedObject, string description, string fieldName, SceneAsset oldScene)
    {        
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        var newScene = EditorGUILayout.ObjectField(description, oldScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            SetObjectField(serializedObject, fieldName, newPath);
        }
    }

    public static void DrawInspectorField(SerializedObject serializedObject, string description, string fieldName, float oldValue)
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        float newValue = EditorGUILayout.FloatField(description, oldValue);

        if (EditorGUI.EndChangeCheck())
        {
            SetObjectField(serializedObject, fieldName, newValue);
        }
    }

    public static void DrawInspectorField(SerializedObject serializedObject, string description, string fieldName, List<GameObject> oldValue)
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        var property = serializedObject.FindProperty(fieldName);
        var field = new UnityEditor.UIElements.PropertyField(property);
        
        //EditorGUILayout.ObjectField(oldValue, typeof(List<GameObject>));

        //float newValue = EditorGUILayout.FloatField(description, oldValue);

        if (EditorGUI.EndChangeCheck())
        {
            //SetObjectField(serializedObject, fieldName, newValue);
        }
    }

    public static void DrawInspectorField(SerializedObject serializedObject, string description, string fieldName, string oldValue)
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        string newValue = EditorGUILayout.TextField(description, oldValue);

        if (EditorGUI.EndChangeCheck())
        {
            SetObjectField(serializedObject, fieldName, newValue);
        }
    }

    public static void DrawInspectorField(SerializedObject serializedObject, string description, string fieldName, int oldValue)
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        int newValue = EditorGUILayout.IntField(description, oldValue);

        if (EditorGUI.EndChangeCheck())
        {
            SetObjectField(serializedObject, fieldName, newValue);
        }
    }

    public static void DrawInspectorField(SerializedObject serializedObject, string description, string fieldName, bool oldValue)
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        bool newValue = EditorGUILayout.Toggle(description, oldValue);

        if (EditorGUI.EndChangeCheck())
        {
            SetObjectField(serializedObject, fieldName, newValue);
        }
    }

    public static void SetObjectField(SerializedObject serializedObject, string fieldName, string value)
    {
        var property = serializedObject.FindProperty(fieldName);
        property.stringValue = value;
        serializedObject.ApplyModifiedProperties();
    }

    public static void SetObjectField(SerializedObject serializedObject, string fieldName, float value)
    {
        var property = serializedObject.FindProperty(fieldName);
        property.floatValue = value;
        serializedObject.ApplyModifiedProperties();
    }

    public static void SetObjectField(SerializedObject serializedObject, string fieldName, int value)
    {
        var property = serializedObject.FindProperty(fieldName);
        property.intValue = value;
        serializedObject.ApplyModifiedProperties();
    }

    public static void SetObjectField(SerializedObject serializedObject, string fieldName, bool value)
    {
        var property = serializedObject.FindProperty(fieldName);
        property.boolValue = value;
        serializedObject.ApplyModifiedProperties();
    }
}
