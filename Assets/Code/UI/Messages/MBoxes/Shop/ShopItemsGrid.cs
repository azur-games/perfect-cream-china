using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ShopItemsGrid : MonoBehaviour
{
    #region Fields
    
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField] int columnsNum = 3;
    [SerializeField] int rowsNum = 3;

    [Header("Spacing settings")]
    [SerializeField] Vector2 minimalSpacing = new Vector2(10.0f, 10.0f);
    [SerializeField] Vector2 maximalSpacing = new Vector2(60.0f, 60.0f);

    [Header("Items")]
    [SerializeField] List<ShopItem> availableItems = new List<ShopItem>();

    [Space]
    [Header("Show animation settings")]
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] float scaleDuration = 0.3f;
    [SerializeField] float delayBetweenItems = 0.1f;


    int itemsPerPage = 0;

    #endregion



    #region Properties

    public int ItemsPerPage
    {
        get
        {
            if (itemsPerPage <= 0)
            {
                itemsPerPage = Math.Min(rowsNum * columnsNum, availableItems.Count);
            }

            return itemsPerPage;
        }
    }


    public Vector2 ContentSize
    {
        get
        {
            Vector2 size = Size;
            RectOffset padding = gridLayout.padding;
            
            size.x -= padding.left + padding.right;
            size.y -= padding.bottom + padding.top;

            return size;
        }
    }


    public Vector2 Size => ((RectTransform) transform).rect.size;


    public List<ShopItem> AvailableItems => availableItems;

    #endregion



    #region Unity lifecycle

    void OnDestroy()
    {
        DOTween.Kill(gameObject, true);
    }

    #endregion



    #region Layout calculations

    public void UpdateLayout(Rect rect)
    {
        RectTransform rectTransform = transform as RectTransform;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

        Vector2 layoutSize = rect.size;
        Vector2 cellSize = gridLayout.cellSize;
        Vector2 spacing = CalculateSpacing(layoutSize);

        Vector2 currentSize = new Vector2(columnsNum * cellSize.x + (columnsNum - 1) * spacing.x, 
                                          rowsNum * cellSize.y + (rowsNum - 1) * spacing.y);

        float minimalScale = Mathf.Min(layoutSize.x / currentSize.x, layoutSize.y / currentSize.y);

        currentSize *= minimalScale;
        
        Vector2 flexibleSpace = 0.5f * (layoutSize - currentSize);

        RectOffset paddings = new RectOffset();
        paddings.left = (int)flexibleSpace.x;
        paddings.right = (int)flexibleSpace.x;
        paddings.bottom = (int)flexibleSpace.y;
        paddings.top = (int)flexibleSpace.y;

        gridLayout.padding = paddings;
        gridLayout.cellSize = minimalScale * cellSize;
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columnsNum;
    }


    Vector2 CalculateSpacing(Vector2 layoutSize)
    {
        Vector2 cellSize = gridLayout.cellSize;
        Vector2 spacing = new Vector2();

        spacing.x = Mathf.Clamp((layoutSize.x - cellSize.x * columnsNum) / (columnsNum - 1), minimalSpacing.x, maximalSpacing.x);
        spacing.y = Mathf.Clamp((layoutSize.y - cellSize.y * rowsNum) / (rowsNum - 1), minimalSpacing.y, maximalSpacing.y);

        return spacing;
    }

    #endregion



    #region Reset items

    public void ResetItems()
    {
        DOTween.Kill(gameObject, true);
        foreach (var item in availableItems)
        {
            item.ItemInfo = null;
            item.transform.localScale = Vector3.one;;
        }
    }

    #endregion



    #region Show animation

    public void PlayShowAnimation(Action onComplete = null)
    {
        Sequence sequence = DOTween.Sequence();

        DOTween.Kill(gameObject, true);
        foreach (var item in availableItems)
        {
            if (item.gameObject.activeInHierarchy)
            {
                item.transform.localScale = Vector3.zero;

                Tween itemTween = item.transform.DOScale(1.0f, scaleDuration).SetEase(scaleCurve).SetDelay(delayBetweenItems);
                sequence.Join(itemTween);
            }
        }

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        }).SetAutoKill(true).SetTarget(gameObject).Play();
    }

    #endregion
}