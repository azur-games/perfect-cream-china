using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformsTreeSerializerComponent : TransformsTreeSerializer
{
    public string CurrentVariantName;

    private void Start()
    {
        ApplyVariant(Env.Instance.Rules.CameraView.Value);
    }

    public void ApplyVariant(string variantName)
    {
        ApplyVariant(FindByName(variantName));
    }

    public void ApplyVariant(TransformsTreeSerializer.TransformVariant variant)
    {
        if (null == variant)
        {
            variant = Variants.First();
            if (null == variant) return;
        }

        CurrentVariantName = variant.Name;

        foreach (TransformsTreeSerializer.TransformInfo info in variant.Data)
        {
            info.Transform.localPosition = info.LocalPosition;
            info.Transform.localRotation = info.LocalRotation;
            info.Transform.localScale = info.LocalScale;
        }

        SceneVisual sceneVisual = this.gameObject.GetComponent<SceneVisual>();
        if (null != sceneVisual)
        {
            sceneVisual.UpdateBackSize();
        }

        Env.Instance.Rules.CameraView.Value = variant.Name;
    }

    public void ApplyNextVariant()
    {
        if (string.IsNullOrEmpty(CurrentVariantName))
        {
            CurrentVariantName = Variants.First().Name;
        }
        else
        {
            TransformsTreeSerializer.TransformVariant currentVariant = FindByName(CurrentVariantName);
            int index = Variants.IndexOf(currentVariant);
            index = (index + 1) % Variants.Count;
            CurrentVariantName = Variants[index].Name;
        }

        ApplyVariant(CurrentVariantName);
    }
}
