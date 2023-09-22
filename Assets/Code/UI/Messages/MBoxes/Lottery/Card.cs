using System;
using TMPro;
using UnityEngine;

public sealed class Card: MonoBehaviour
{
    [NonSerialized] public bool IsOpened;

    public MeshRenderer BackRenderer;
    public MeshRenderer ItemRenderer;

    public GameObject CoinsIcon;
    public TMP_Text CoinsText;
    
    public Vector3 OpenRotation;
    public Vector3 ClosedRotation;

    public ParticleSystem Confetti;
}