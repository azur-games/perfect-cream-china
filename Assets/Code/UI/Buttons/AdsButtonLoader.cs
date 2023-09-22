using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class AdsButtonLoader : MonoBehaviour
{
    [SerializeField] private Image loaderImage;
    [SerializeField] private List<Transform> objectsToDisable;
    [SerializeField] private float rotationDuration = 0.7f;

    private Tween rotationTween = null;

    public bool IsEnabled { get; set; }

    public Tween RotationTween
    {
        get
        {
            if (rotationTween == null)
            {
                rotationTween = loaderImage.transform.DOLocalRotate(Vector3.forward * -360f, rotationDuration, RotateMode.FastBeyond360).SetLoops(-1)
                                                                                                                                        .SetEase(Ease.Linear);
            }

            return rotationTween;
        }
    }


    private void Awake()
    {
        loaderImage.gameObject.SetActive(false);
    }


    public void SetLoaderState(bool isEnabled)
    {
        if (IsEnabled != isEnabled)
        {
            IsEnabled = isEnabled;
            if (isEnabled)
            {
                loaderImage.gameObject.SetActive(true);
                RotationTween.Play();
            }
            else
            {
                RotationTween.Pause();
                loaderImage.gameObject.SetActive(false);
            }

            foreach(var objectTransform in objectsToDisable)
            {
                objectTransform.gameObject.SetActive(!isEnabled);
            }
        }
    }
}
