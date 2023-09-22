using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformsTreeSerializer : MonoBehaviour
{
    [System.Serializable]
    public class TransformVariant
    {
        [SerializeField] public string Name;
        [SerializeField] public List<TransformInfo> Data;

        public TransformVariant()
        {
            Name = "noname";
            Data = new List<TransformInfo>();
        }
    }

    [System.Serializable]
    public class TransformInfo
    {
        [SerializeField] public Transform Transform;
        [SerializeField] public Vector3 LocalPosition;
        [SerializeField] public Quaternion LocalRotation;
        [SerializeField] public Vector3 LocalScale;
    }

    [SerializeField]
    public List<TransformVariant> Variants;

    public TransformVariant FindByName(string name)
    {
        foreach (TransformVariant variant in Variants)
        {
            if (variant.Name == name) return variant;
        }

        return null;
    }

    public void FillVariantByCurrentState(TransformVariant variant, Transform rootTransform)
    {
        variant.Data.Clear();
        FillVariantByCurrentState_internal(variant, rootTransform);
    }

    private TransformInfo GetTransformInfo(Transform transform)
    {
        TransformInfo info = new TransformInfo()
        {
            Transform = transform,
            LocalPosition = transform.localPosition,
            LocalRotation = transform.localRotation,
            LocalScale = transform.localScale
        };

        return info;
    }

    private void FillVariantByCurrentState_internal(TransformVariant variant, Transform transform)
    {
        variant.Data.Add(GetTransformInfo(transform));

        for (int index = 0; index < transform.childCount; index++)
        {
            FillVariantByCurrentState_internal(variant, transform.GetChild(index));
        }
    }
}
