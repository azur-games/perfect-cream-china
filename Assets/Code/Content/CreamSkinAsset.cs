using System;
using UnityEngine;

public class CreamSkinAsset : ContentAsset
{
    public override AssetType GetAssetType() { return AssetType.CreamSkin; }

    public Material Material;
    public CreamSkinMeshGenerator SkinMeshGenerator;
    public float MeshTwistingAngle;
    public float TextureTwistingAngle;
    public float TextureLengthScale;
    public float TextureWidthScale;
    public AnimationCurve AmplitudeCurve;
    public Color FXColor;

    private int precalculatedSegs = -1;
    public float[] precalculatedAmps { get; private set; } = null;

    public void PrecalculateAmps(int segsCount)
    {
        if (precalculatedSegs == segsCount) return;

        precalculatedSegs = segsCount;
        precalculatedAmps = new float[precalculatedSegs];
        float unSegs = 1.0f / (float)segsCount;

        for (int i = 0; i < segsCount; i++)
        {
            precalculatedAmps[i] = AmplitudeCurve.Evaluate(unSegs * (float)i);
        }
    }

    public void SetMainColorToParticles(GameObject particlesRoot)
    {
        foreach (ParticleSystem particleSystem in particlesRoot.GetComponentsInChildren<ParticleSystem>(true))
        {
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.startColor = FXColor;
        }
    }
}
