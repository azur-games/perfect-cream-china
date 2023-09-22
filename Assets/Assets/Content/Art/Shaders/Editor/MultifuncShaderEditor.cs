using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


public class MultifuncShaderEditor : ShaderGUI
{
    struct FeatureDef
    {
        public string keyword;
        public string toggleFlagName;
        public List<string> properties;
    }


    readonly List<FeatureDef> K_FEATURES = new List<FeatureDef>
    {
        new FeatureDef
        {
            keyword = "F_MAIN_TEXTURE_TRANSITION",
            toggleFlagName = "_F_texture_transition",
            properties = new List<string>{ "_SecondMainTex", "_TransitionFactor" }
        },
        new FeatureDef
        {
            keyword = "F_DIFFUSE",
            toggleFlagName = "_F_diffuse",
            properties = new List<string> { "_DiffuseRampTex" }
        },
        new FeatureDef
        {
            keyword = "F_SPECULAR",
            toggleFlagName = "_F_specular",
            properties = new List<string> { "_SpecTex", "_SpecShininess", "_SpecIntensity" }
        },
        new FeatureDef
        {
            keyword = "F_GRADIENT",
            toggleFlagName = "_F_gradient",
            properties = new List<string> { "_GradientTex" }
        },
        new FeatureDef
        {
            keyword = "F_EMISSION",
            toggleFlagName = "_F_emission",
            properties = new List<string> { "_EmissionMap", "_EmissionIntensity" }
        },
        new FeatureDef
        {
            keyword = "F_RIM",
            toggleFlagName = "_F_rim",
            properties = new List<string> { "_RimColor", "_RimPower" }
        },
        new FeatureDef
        {
            keyword = "F_REFLECTION",
            toggleFlagName = "_F_reflection",
            properties = new List<string> { "_ReflectionTex", "_ReflectionCube" }
        }
    };



    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        Material targetMat = materialEditor.target as Material;

        foreach (FeatureDef featureDef in K_FEATURES)
        {
            bool isFeatureEnabled = (Array.IndexOf(targetMat.shaderKeywords, featureDef.keyword) != -1);
            if (isFeatureEnabled)
            {
                if (featureDef.properties.Count > 0)
                {
                    MaterialProperty toggleProp = Array.Find(properties, (prop) => prop.name.Equals(featureDef.toggleFlagName));
                    GUILayout.Label(string.Format("---- {0} props ----", toggleProp.displayName));
                }

                ++EditorGUI.indentLevel;
                foreach (string propName in featureDef.properties)
                {
                    MaterialProperty featureProp = Array.Find(properties, (prop) => prop.name.Equals(propName));
                    if (featureProp != null)
                    {
                        materialEditor.ShaderProperty(featureProp, featureProp.displayName);
                    }
                }
                --EditorGUI.indentLevel;
            }
        }
    }
}