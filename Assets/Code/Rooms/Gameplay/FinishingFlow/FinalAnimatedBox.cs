using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalAnimatedBox : MonoBehaviour
{
    public enum BoxType
    {
        Unknown = 0,
        Common = 1,
        Plastic = 2,
        Metall = 3,
        Soap = 4,
        FastFood = 5,
    }

    [System.Serializable]
    public struct BoxColor
    {
        public Color Box;
        public Color Tape;
    }

    public List<BoxColor> FinalBoxColors;
    public List<GameObject> BoxObjects;
    public List<GameObject> TapeObjects;

    public BoxType TypeOfBox;

    public BoxColor? ApplyRandomColors()
    {
        if (null == FinalBoxColors) return null;
        if (0 == FinalBoxColors.Count) return null;

        BoxColor? colorVariant = FinalBoxColors[Random.Range(0, FinalBoxColors.Count)];
        ApplyColors(BoxObjects, colorVariant.Value.Box);
        ApplyColors(TapeObjects, colorVariant.Value.Tape);
        return colorVariant;
    }

    private void ApplyColors(List<GameObject> objects, Color color)
    {
        foreach (GameObject go in objects)
        {
            foreach (MeshRenderer mRenderer in go.GetComponentsInChildren<MeshRenderer>())
            {
                mRenderer.material.SetColor("_MainColor", color);
            }

            foreach (SkinnedMeshRenderer mRenderer in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                mRenderer.material.SetColor("_MainColor", color);
            }
        }
    }
}
