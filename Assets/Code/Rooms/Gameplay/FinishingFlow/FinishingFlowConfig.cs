using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FinishingFlowConfig", menuName = "Configs/FinishingFlowConfig")]
public class FinishingFlowConfig : ScriptableObject
{
    public AnimationCurve SceneItemsMoveOutGraph;
    public AnimationCurve SceneItemsMoveInGraph;
    public AnimationCurve FinalItemRotationGraph;

    public Material ConfitureFallingMaterial;
    public Material ConfitureMaterial;

    public List<DelitiousSmallPiece> DelitiousMainPieces;
    public Vector3 MainPieceTargetPosition;
    public Vector3 MainPieceTargetRotation;

    public Shader CreamPowShader;

    public List<Color> ConfitureColors;
}
