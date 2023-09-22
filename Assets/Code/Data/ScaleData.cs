using DG.Tweening;
using UnityEngine;


[CreateAssetMenu(fileName = "Data_Scale", menuName = "Data/Scales/Scale")]
public sealed class ScaleData : ScriptableObject
{
    public AnimationCurve ease;
    [Range(0f, 5f)] public float duration;
    public Vector3 scale;
}
