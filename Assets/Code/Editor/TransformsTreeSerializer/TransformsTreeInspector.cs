using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TransformsTreeSerializerComponent))]
public class TransformsTreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        TransformsTreeSerializerComponent comp = target as TransformsTreeSerializerComponent;
        
        Rect availRect = EditorGUILayout.GetControlRect();

        if (comp.Variants == null)
        {
            comp.Variants = new List<TransformsTreeSerializer.TransformVariant>();
            EditorUtility.SetDirty(comp.gameObject);
            EditorUtility.SetDirty(comp);
        }

        EditorGUI.BeginChangeCheck();
        TransformsTreeSerializer.TransformVariant variantForDeleting = null;

        foreach (TransformsTreeSerializer.TransformVariant variant in comp.Variants)
        {
            EditorGUILayout.BeginHorizontal();
            variant.Name = EditorGUILayout.TextField("Name: ", variant.Name);
            if (comp.CurrentVariantName != variant.Name)
            {
                if (GUILayout.Button("Activate"))
                {
                    comp.ApplyVariant(variant);
                    comp.CurrentVariantName = variant.Name;
                }
            }

            if (GUILayout.Button("Save State"))
            {
                comp.FillVariantByCurrentState(variant, comp.transform);
            }

            if (GUILayout.Button("Delete"))
            {
                variantForDeleting = variant;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (null != variantForDeleting)
        {
            comp.Variants.Remove(variantForDeleting);
            if (comp.CurrentVariantName == variantForDeleting.Name) comp.CurrentVariantName = null;
        }

        if (GUILayout.Button("Add"))
        {
            comp.Variants.Add(new TransformsTreeSerializer.TransformVariant());
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(comp.gameObject);
            EditorUtility.SetDirty(comp);
            Debug.Log("WAS DIRTY");
        }
    }
}
