using System.Net.Security;
using UnityEngine;
using DG.Tweening;


public class GlowController : MonoBehaviour
{
    #region Fields

    [SerializeField] AnimationCurve curve;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] float duration;
    [SerializeField] float delayBetweenLoops;

    #endregion



    #region Unity lifecycle

    void Start()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMove(startPosition, 0.0f).SetTarget(this)
                                                                  .SetAutoKill(true));

        sequence.Append(transform.DOLocalMove(endPosition, duration).SetEase(curve)
                                                                    .SetTarget(this)
                                                                    .SetAutoKill(true));

        sequence.AppendInterval(delayBetweenLoops);

        sequence.SetLoops(-1, LoopType.Restart);

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);
        sequence.Play();
    }


    void OnDestroy()
    {
        DOTween.Kill(this);
    }

    #endregion
}