using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class InteriorUpgradeBox : UIMessageBox
{
    #region Fields

    [SerializeField] Image background;
    [SerializeField] Transform objectRoot;
    [SerializeField] Transform raysRoot;
    [SerializeField] Button fullscreenButton;
    [SerializeField] ParticleSystem puffEffect;
    [SerializeField] Text tapToContinue;

    [Header("Dynamics settings")]
    [SerializeField] float tapToContinueFadeDuration = 0.2f;
    [SerializeField] float tapToContinueShowDelay = 0.5f;
    [SerializeField] float autoCloseDelay = 3.0f;
    [SerializeField] float fadeDuration = 0.2f;

    [Header("Scale settings")]
    [SerializeField] AnimationCurve scaleUpCurve;
    [SerializeField] float scaleUpDuration = 0.2f;
    [SerializeField] AnimationCurve scaleDownCurve;
    [SerializeField] float scaleDownDuration = 0.2f;

    [Header("Rotation settings")]
    [SerializeField] AnimationCurve rotationCurve;
    [SerializeField] float rotationAngle = 20.0f;
    [SerializeField] float oneRotationDuration = 0.75f;

    [Header("Rays settings")]
    [SerializeField] DOTweenAnimation raysAnimation;
    [SerializeField] float raysScaleDuration = 0.2f;


    GameObject interiorObject;
    Action onClose = null;
    float tapToContinueAlpha = 1.0f;

    #endregion



    #region Unity lifecycle

    protected override void Awake()
    {
        base.Awake();

        tapToContinueAlpha = tapToContinue.color.a;

        background.color = background.color.SetA(0.0f);
        tapToContinue.color = tapToContinue.color.SetA(0.0f);
    }


    void Start()
    {
        fullscreenButton.onClick.AddListener(FullScreenButton_OnClick);

        fullscreenButton.interactable = false;
    }


    void OnDestroy()
    {
        fullscreenButton.onClick.RemoveListener(FullScreenButton_OnClick);

        if (interiorObject != null)
        {
            GameObject.Destroy(interiorObject);
        }

        DOTween.Kill(this);
    }

    #endregion



    #region Initialization

    public void Init(int level, Action onClose = null)
    {
        this.onClose = onClose;

        SetupItem(level);
        ShowAnimation().OnComplete(() => 
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(autoCloseDelay);
            sequence.AppendCallback(() => 
            {
                CloseSelf();
            });

            sequence.SetTarget(this);
            sequence.SetAutoKill(true);
        });
    }

    #endregion



    #region Item settings

    void SetupItem(int level)
    {
        List<ContentItemsLibrary.ContentItemsCollectionElement> interiorObjects = Env.Instance.Content.GetItems(ContentAsset.AssetType.InteriorObject).FindAll((element) => 
        {
            return element.Info.Order <= level;
        });
        
        interiorObjects.Sort((el1, el2) => el1.Info.Order.CompareTo(el2.Info.Order));

        if (interiorObjects.Count > 0)
        {
            ContentItemInfo requiredItemInfo = interiorObjects.Last().Info;
            ContentItem item = Env.Instance.Content.LoadContentItem(ContentAsset.AssetType.InteriorObject, (itemInfo) => 
            {
                return requiredItemInfo.Equals(itemInfo);
            });

            if (item != null)
            {
                InteriorObjectAsset contentAsset = item.Asset as InteriorObjectAsset;
                GameObject objectToInstantiate = (contentAsset.PreviewModel != null) ? contentAsset.PreviewModel : contentAsset.gameObject;

                interiorObject = GameObject.Instantiate(objectToInstantiate, objectRoot);
                interiorObject.gameObject.SetLayerRecursively(LayerMask.NameToLayer("UIMessages"));

                MeshFilter meshFilter = interiorObject.GetComponentInChildren<MeshFilter>();

                if (meshFilter != null)
                {
                    Bounds bounds = meshFilter.mesh.bounds;
                    float objectScale = 1.0f / Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

                    interiorObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
                    interiorObject.transform.localPosition = interiorObject.transform.localPosition - objectScale * bounds.center;
                }
            }
        }
    }

    #endregion



    #region Animations

    Sequence ShowAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        background.color = background.color.SetA(0.0f);
        objectRoot.transform.localScale = Vector3.zero;
        raysRoot.localScale = Vector3.zero;

        sequence.Append(BackgroundFadeInAnimation());
        sequence.AppendCallback(() => fullscreenButton.interactable = true);
        sequence.Append(ItemScaleUpAnimation());
        sequence.Append(RaysScaleUpAnimation());
        sequence.AppendCallback(() => raysAnimation.DOPlay());

        float currentDuration = sequence.Duration(); // temporary
        sequence.Insert(currentDuration, ItemRotationAnimation());
        sequence.Insert(currentDuration + tapToContinueShowDelay, TapToContinueFadeInAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Sequence HideAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, TapToContinueFadeOutAnimation());
        sequence.InsertCallback(0.0f, () => 
        {
            raysAnimation.DOPause();

            puffEffect.Clear();
            puffEffect.Play();
        });
        
        sequence.Insert(0.0f, RaysScaleDownAnimation());
        sequence.Insert(0.0f, ItemScaleDownAnimation());
        sequence.Append(BackgroundFadeOutAnimation());

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tween BackgroundFadeInAnimation()
    {
        return background.DOFade(1.0f, fadeDuration).SetTarget(this).SetAutoKill(true);
    }


    Tween BackgroundFadeOutAnimation()
    {
        return background.DOFade(0.0f, fadeDuration).SetTarget(this).SetAutoKill(true);
    }


    Tween TapToContinueFadeInAnimation()
    {
        return tapToContinue.DOFade(tapToContinueAlpha, fadeDuration).SetTarget(this).SetAutoKill(true);
    }


    Tween TapToContinueFadeOutAnimation()
    {
        return tapToContinue.DOFade(0.0f, fadeDuration).SetTarget(this).SetAutoKill(true);
    }


    Tween ItemScaleUpAnimation()
    {
        return objectRoot.DOScale(1.0f, scaleUpDuration).SetEase(scaleUpCurve).SetTarget(this).SetAutoKill(true);
    }


    Tween ItemScaleDownAnimation()
    {
        return objectRoot.DOScale(0.0f, scaleDownDuration).SetEase(scaleDownCurve).SetTarget(this).SetAutoKill(this);
    }


    Sequence ItemRotationAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(objectRoot.DOLocalRotate(new Vector3(0.0f, rotationAngle, 0.0f), 0.5f * oneRotationDuration, RotateMode.FastBeyond360).SetEase(rotationCurve)
                                                                                                                                              .SetLoops(2, LoopType.Yoyo)
                                                                                                                                              .SetTarget(this)
                                                                                                                                              .SetAutoKill(true));

        sequence.Append(objectRoot.DOLocalRotate(new Vector3(0.0f, -rotationAngle, 0.0f), 0.5f * oneRotationDuration, RotateMode.FastBeyond360).SetEase(rotationCurve)
                                                                                                                                               .SetLoops(2, LoopType.Yoyo)
                                                                                                                                               .SetTarget(this)
                                                                                                                                               .SetAutoKill(true));

        sequence.SetTarget(this);
        sequence.SetAutoKill(true);

        return sequence;
    }


    Tween RaysScaleUpAnimation()
    {
        return raysRoot.DOScale(1.0f, raysScaleDuration).SetTarget(this).SetAutoKill(true);
    }


    Tween RaysScaleDownAnimation()
    {
        return raysRoot.DOScale(0.0f, raysScaleDuration).SetTarget(this).SetAutoKill(true);
    }

    #endregion


    #region Closing logic

    void CloseSelf()
    {
        fullscreenButton.interactable = false;

        DOTween.Kill(this);

        HideAnimation().OnComplete(() => 
        {
            onClose?.Invoke();

            Close();
        });
    }

    #endregion



    #region Events handling

    void FullScreenButton_OnClick()
    {
        CloseSelf();
    }

    #endregion
}