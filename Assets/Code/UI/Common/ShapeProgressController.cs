using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class ShapeProgressController : MonoBehaviour
{
    #region Nested types

    enum ShapeProgressState
    {
        None = 0,
        ReceivingShape
    }

    #endregion



    #region Fields

    [SerializeField] Image shapeImage;
    [SerializeField] Image progressImage;
    [SerializeField] Image shapeFadedImage;
    [SerializeField] Image maskImage;
    [SerializeField] TextMeshProUGUI text;

    [Header("Progress settings")]
    [SerializeField] float completeValue = 1.0f;

    [Space]
    [Header("Receive animation settings")]
    [SerializeField] AnimationCurve receiveCurve;
    [SerializeField] float receiveScale;
    [SerializeField] float receiveDuration;


    ShapeProgressState currentState = ShapeProgressState.None;
    ContentItemIconRef currentItemIconRef = null;

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        Env.Instance.Inventory.OnStarsCountUpdated += Inventory_OnStarsCountUpdated;

        UpdateContent();
    }


    void OnDisable()
    {
        Env.Instance.Inventory.OnStarsCountUpdated -= Inventory_OnStarsCountUpdated;
    }


    void OnDestroy()
    {
        if (currentItemIconRef)
        {
            Resources.UnloadAsset(currentItemIconRef);
        }

        DOTween.Kill(this);
    }

    #endregion
    


    #region Content update

    bool UpdateContent()
    {
        if (currentState == ShapeProgressState.ReceivingShape)
        {
            return false;
        }

        int starsToUnlock = Env.Instance.Inventory.GetNextShapeStarsPrice();

        ContentItemsLibrary.ContentItemsCollectionElement contentElement = Env.Instance.Inventory.GetNext(ContentAsset.AssetType.Shape);
        if (null == contentElement)
        {
            this.gameObject.SetActive(false);
            return false;
        }

        if (contentElement != null && Env.Instance.Inventory.Stars >= starsToUnlock && 
                                      Env.Instance.Inventory.TrySpendStars(starsToUnlock))
        {
            Env.Instance.Inventory.ReceiveShape(contentElement.Info.Name);

            progressImage.fillAmount = completeValue;
            text.text = starsToUnlock.ToString() + "/" + starsToUnlock.ToString();

            PlayReceiveAnimation();

            return true;
        }

        if (null != contentElement)
        {
            ContentItemInfo itemInfo = contentElement.Info;

            if (currentItemIconRef != null)
            {
                Resources.UnloadAsset(currentItemIconRef);
            }

            currentItemIconRef = Env.Instance.Content.LoadContentItemIconRef(itemInfo.AssetType, itemInfo.Name);
        }
        else if (currentItemIconRef != null)
        {
            Resources.UnloadAsset(currentItemIconRef);
            currentItemIconRef = null;
        }

        if (currentItemIconRef != null)
        {
            float progress = Mathf.Clamp01((float)Env.Instance.Inventory.Stars / starsToUnlock);

            shapeImage.gameObject.SetActive(true);
            text.gameObject.SetActive(true);

            if (maskImage != null)
            {
                maskImage.sprite = currentItemIconRef.AlternativeIcon;
            }

            if (shapeFadedImage != null)
            {
                shapeFadedImage.sprite = currentItemIconRef.AlternativeIcon;
                shapeFadedImage.gameObject.SetActive(true);
            }

            shapeImage.sprite = currentItemIconRef.Icon;
            progressImage.fillAmount = progress * completeValue;
            text.text = Env.Instance.Inventory.Stars.ToString() + "/" + starsToUnlock;

            return true;
        }

        if (null != shapeFadedImage)
        {
            shapeFadedImage.gameObject.SetActive(false);
            shapeImage.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }

        return false;
    }

    #endregion



    #region Receive animation

    void PlayReceiveAnimation()
    {
        currentState = ShapeProgressState.ReceivingShape;

        transform.DOScale(receiveScale, 0.5f * receiveDuration).SetEase(receiveCurve)
                                                               .SetLoops(2, LoopType.Yoyo)
                                                               .SetTarget(this)
                                                               .SetAutoKill(true)
                                                               .OnComplete(ReceiveAnimation_OnComplete);
    }

    #endregion


    
    #region Events handling

    void Inventory_OnStarsCountUpdated(int amount, Transform animationRoot, Action callback)
    {
        if (amount > 0)
        {
            UpdateContent();
        }
    }


    void ReceiveAnimation_OnComplete()
    {
        currentState = ShapeProgressState.None;

        UpdateContent();
    }

    #endregion
}