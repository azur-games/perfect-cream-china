using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverUIHelper : MonoBehaviour
{
    [SerializeField] private CurveAnimatedScale feverTextsAnimation = null;
    [SerializeField] private CoinsFlightController2D flyingCoins = null;

    [SerializeField] private GameObject feverText_Good = null;
    [SerializeField] private GameObject feverText_Perfect = null;

    [SerializeField] private RectTransform sourceRectTransform = null;
    [SerializeField] private RectTransform targetRectTransform = null;

    [SerializeField] private CurveAnimatedScale feverStartTextsAnimation = null;

    [SerializeField] private ParticleSystem feverParticles = null;

    void Start()
    {
        feverTextsAnimation.gameObject.SetActive(false);
        feverStartTextsAnimation.gameObject.SetActive(false);
    }

    public bool DebugPlay = false;

    private void Update()
    {
        if (DebugPlay)
        {
            DebugPlay = false;
            PlayFinish(20, true, () => { });
        }
    }

    public void PlayStart()
    {
        feverStartTextsAnimation.Play();
    }
    
    public void PlayFinish(int coinsCount, bool isPerfect, System.Action onFinish)
    {
        feverText_Good.SetActive(!isPerfect);
        feverText_Perfect.SetActive(isPerfect);

        feverTextsAnimation.Play();
        // flyingCoins.Play(sourceRectTransform, targetRectTransform, coinsCount, () =>
        // {
            onFinish?.Invoke();
        // });

        feverParticles.Play();
    }
}
