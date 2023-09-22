using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;


public class ShopItemsScroll : MonoBehaviour
{
    #region Fields
    
    [SerializeField] GameObject shopGridPrefab;
    [SerializeField] PagesHandler pagesHandler;
    [SerializeField] HorizontalLayoutGroup gridsLayout;
    [SerializeField] RectTransform gridRect;
    [SerializeField] RectTransform tabRect;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] ScrollRectHandler scrollRectHandler;
    [SerializeField] Transform touchBlocker;
    [SerializeField] ParticleSystem particleEffect;

    [Header("Spacing settings")]
    [SerializeField] int nextItemOffset = 10;

    [Space]
    [Header("Scroll settings")]
    [SerializeField] int scrollMaxBorderOffset = 200;
    [SerializeField] float maxSwipeTimeForPageChange = 0.1f;
    [SerializeField] int minSwipeDistanceToPageChange = 40;
   
    [Header("Scroll snapping settings")]
    [SerializeField] AnimationCurve snappingCurve;
    [SerializeField] float snappingCellTime = 1.0f;
    [SerializeField] float maxSpeedForSnapping = 50.0f;

    [Header("Show item settings")]
    [SerializeField] float showItemScrollDuration = 1.0f;


    List<ShopItemsGrid> activeGrids = new List<ShopItemsGrid>();
    Stack<ShopItemsGrid> gridsPool = new Stack<ShopItemsGrid>();


    float lastScrollPosition = 0.0f;
    float swipeStartPosition = 0.0f;
    DateTime swipeStartTime = DateTime.MinValue;
    int pageOnSwipeBegin = -1;
    bool isSnapping = false;
    bool shouldHighlightLastItem = false;

    #endregion



    #region Properties

    float ScrollDelta
    {
        get
        {
            float layoutWidth = ((RectTransform) gridsLayout.transform).rect.width;
            return layoutWidth * Mathf.Abs(scrollRect.horizontalNormalizedPosition - lastScrollPosition);
        }
    }


    float SwipeOffset
    {
        get
        {
            float layoutWidth = ((RectTransform) gridsLayout.transform).rect.width;
            return layoutWidth * (scrollRect.horizontalNormalizedPosition - swipeStartPosition);
        }
    }


    double SwipeTime
    {
        get
        {
            return (DateTime.Now - swipeStartTime).TotalSeconds;
        }
    }

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(ScrollRect_OnValueChanged);
        scrollRectHandler.OnDragStateChanged += ScrollRectHandler_OnDragStateChanged;
    }


    void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(ScrollRect_OnValueChanged);
        scrollRectHandler.OnDragStateChanged -= ScrollRectHandler_OnDragStateChanged;
    }


    void Update()
    {
        float scrollSpeed = ScrollDelta / Time.deltaTime;

        if (!isSnapping && !scrollRectHandler.IsDragging && scrollSpeed <= maxSpeedForSnapping)
        {
            SnapScroll();
        }

        lastScrollPosition = scrollRect.horizontalNormalizedPosition;
    }


    void OnDestroy()
    {
        StopSnapping();

        foreach (var activeGrid in activeGrids)
        {
            Destroy(activeGrid.gameObject);
        }

        while (gridsPool.Count > 0)
        {
            ShopItemsGrid grid = gridsPool.Pop();
            Destroy(grid.gameObject);
        }
    }

    #endregion



    #region Initialization

    public void Init(List<ContentItemInfo> items)
    {
        int itemsCount = items.Count;
        int pagesCount = 0;
        int itemIndex = 0;

        MoveActiveGridsToPool();

        #warning ACHTUNG! ACHTUNG! SHIT FIELD AHEAD!
        while (itemsCount > 0)
        {
            ShopItemsGrid grid = GetItemsGridFromPool();
            activeGrids.Add(grid);

            int limit = Math.Min(itemsCount, grid.AvailableItems.Count);
            for (int i = 0; i < limit; i++)
            {
                grid.AvailableItems[i].ItemInfo = items[itemIndex + i];
            }

            itemsCount -= grid.ItemsPerPage;
            itemIndex += grid.ItemsPerPage;
            pagesCount++;
        }

        ShopItemsGrid lastGrid = activeGrids.LastObject();
        int remaining = -itemsCount;
        int gridItemsCount = lastGrid.AvailableItems.Count;
        int firstNonActiveIndex = gridItemsCount - remaining;
        for (int i = gridItemsCount - 1; i >= firstNonActiveIndex; i--)
        {
            lastGrid.AvailableItems[i].gameObject.SetActive(false);
        }

        StopSnapping();
        UpdateLayout();

        scrollRect.horizontalNormalizedPosition = 0.0f;
        pagesHandler.Init(pagesCount, 0);

        PlayShowAnimation();

        scrollRect.enabled = (activeGrids.Count > 1);
    }

    #endregion



    #region Layout update

    void UpdateLayout()
    {
        float screenWidth = tabRect.rect.width;

        foreach (var grid in activeGrids)
        {
            grid.UpdateLayout(gridRect.rect);
        }

        if (activeGrids.Count > 0)
        {
            ShopItemsGrid grid = activeGrids[0];

            float spacing = 0.5f * (screenWidth + grid.ContentSize.x) - grid.Size.x + nextItemOffset;
            gridsLayout.spacing = spacing;

            float layoutWidth = activeGrids.Count * grid.Size.x;
            layoutWidth += (activeGrids.Count - 1) * spacing;
            layoutWidth += gridsLayout.padding.left;
            layoutWidth += gridsLayout.padding.right;

            ((RectTransform) gridsLayout.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layoutWidth);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) gridsLayout.transform);
        }
    }

    #endregion



    #region Show animation

    void PlayShowAnimation(Action onComplete = null)
    {
        scrollRect.horizontal = false;

        activeGrids[pagesHandler.CurrentPage].PlayShowAnimation(() => 
        {
            scrollRect.horizontal = true;
            onComplete?.Invoke();
        });
    }

    #endregion



    #region Scroll snapping

    void SnapScroll()
    {
        if (activeGrids.Count < 2)
            return;
        
        float cellSnapWidth = 1.0f / Mathf.Clamp(activeGrids.Count - 1, 1, activeGrids.Count);
        float targetPosition = pagesHandler.CurrentPage * cellSnapWidth;
        float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition);
        float duration = (snappingCellTime * distance / cellSnapWidth);

        SnapScroll(targetPosition, duration);
    }


    void SnapScroll(float targetPosition, float duration, Action onComplete = null)
    {
        StopSnapping();

        DOTween.To(() => scrollRect.horizontalNormalizedPosition, position => 
        {
            scrollRect.horizontalNormalizedPosition = position;
        }, targetPosition, duration).SetEase(snappingCurve)
                                    .SetTarget(scrollRect)
                                    .SetAutoKill(true)
                                    .OnComplete(() => 
                                    {
                                        SnappingTweener_OnComplete();
                                        onComplete?.Invoke();
                                    });

        isSnapping = true;
    }


    void StopSnapping()
    {
        DOTween.Kill(scrollRect);
        isSnapping = false;
    }

    #endregion



    #region Show item

    public ShopItem ShowItem(ContentItemInfo itemInfo, bool isTabChanged, Action onComplete = null)
    {
        int gridsCount = activeGrids.Count;
        int page = FindItemPage(itemInfo, out ShopItem result, out ShopItem nextItem);

        if (page != -1)
        {
            float cellSnapWidth = 1.0f / Mathf.Clamp(gridsCount - 1, 1, gridsCount);
            float targetPosition = page * cellSnapWidth;

            touchBlocker.gameObject.SetActive(true);

            if (result != null)
            {
                if (itemInfo.AssetType == ContentAsset.AssetType.Shape)
                {
                    result.IsPreviewForced = true;

                    if (nextItem != null)
                        nextItem.IsUnknownForced = true;
                }
                else
                {
                    result.IsUnknownForced = true;
                }
            }

            if (isTabChanged)
            {
                scrollRect.horizontalNormalizedPosition = targetPosition;
                pagesHandler.CurrentPage = page;
                PlayShowAnimation(() => 
                {
                    OpenAnimation_OnItemAppeared(result, nextItem, onComplete);
                });
            }
            else
            {
                float epsilon = 0.05f * cellSnapWidth;
                bool isInstantSnap = (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) <= epsilon);
                float resultDuration = (isInstantSnap) ? 0.0f : showItemScrollDuration;

                SnapScroll(targetPosition, resultDuration, () => 
                {
                    OpenAnimation_OnItemAppeared(result, nextItem, onComplete);
                });
            }
        }
        else
        {
            onComplete?.Invoke();
        }

        return result;
    }


    int FindItemPage(ContentItemInfo itemInfo, out ShopItem item, out ShopItem nextItem)
    {
        int gridsCount = activeGrids.Count;
        int page = -1;

        item = null;
        nextItem = null;

        for (int i = 0; i < gridsCount; i++)
        {
            ShopItemsGrid lookUpGrid = activeGrids[i];
            int itemsCount = lookUpGrid.AvailableItems.Count;
            int itemIndex = 0;

            while (itemIndex < itemsCount)
            {
                ShopItem lookUpItem = lookUpGrid.AvailableItems[itemIndex];

                if (lookUpItem.ItemInfo.AssetType == itemInfo.AssetType && lookUpItem.ItemInfo.Name.Equals(itemInfo.Name))
                {
                    item = lookUpItem;
                    break;
                }

                itemIndex++;
            }

            if (item != null)
            {
                page = i;

                if (itemIndex < itemsCount - 1)
                {
                    nextItem = lookUpGrid.AvailableItems[itemIndex + 1];
                }
                else if (i < gridsCount - 1)
                {
                    nextItem = activeGrids[i + 1].AvailableItems.First();
                }

                break;
            }
        }

        return page;
    }

    #endregion
    


    #region Pool managment

    void MoveActiveGridsToPool()
    {
        while (activeGrids.Count > 0)
        {
            ShopItemsGrid grid = activeGrids.FirstObject();
            activeGrids.RemoveAt(0);
            MoveGridToPool(grid);
        }
    }


    ShopItemsGrid GetItemsGridFromPool()
    {
        ShopItemsGrid result = null;

        if (gridsPool.Count > 0)
        {
            result = gridsPool.Pop();
            result.gameObject.SetActive(true);
            result.gameObject.transform.SetParent(gridsLayout.gameObject.transform);
        }
        else
        {
            result = Instantiate(shopGridPrefab, gridsLayout.gameObject.transform).GetComponent<ShopItemsGrid>();
        }

        result.ResetItems();
        foreach (var item in result.AvailableItems)
        {
            item.gameObject.SetActive(true);
        }

        return result;
    }


    void MoveGridToPool(ShopItemsGrid shopGrid)
    {
        shopGrid.gameObject.transform.SetParent(null);
        shopGrid.gameObject.SetActive(false);

        gridsPool.Push(shopGrid);
    }

    #endregion



    #region Events handling

    void ScrollRect_OnValueChanged(Vector2 value)
    {
        float cellWidth = 1.0f / activeGrids.Count;
        int currentCellIndex = (int)(value.x / cellWidth);

        currentCellIndex = (currentCellIndex < activeGrids.Count) ? currentCellIndex : activeGrids.Count - 1;
        currentCellIndex = (currentCellIndex > 0) ? currentCellIndex : 0;

        pagesHandler.CurrentPage = currentCellIndex;

        float layoutWidth = ((RectTransform) gridsLayout.transform).rect.width;
        float currentPosition = value.x * layoutWidth;

        if (currentPosition - layoutWidth >= scrollMaxBorderOffset)
        {
            scrollRect.horizontalNormalizedPosition = 1.0f + scrollMaxBorderOffset / layoutWidth;
        }
        else if (currentPosition <= -scrollMaxBorderOffset)
        {
            scrollRect.horizontalNormalizedPosition = -scrollMaxBorderOffset / layoutWidth;
        }
    }


    void ScrollRectHandler_OnDragStateChanged(bool isDragging)
    {
        if (isDragging)
        {
            StopSnapping();

            swipeStartPosition = scrollRect.horizontalNormalizedPosition;
            swipeStartTime = DateTime.Now;

            pageOnSwipeBegin = pagesHandler.CurrentPage;
        }
        else
        {
            float swipeOffset = SwipeOffset;

            if (SwipeTime <= maxSwipeTimeForPageChange && Mathf.Abs(swipeOffset) >= minSwipeDistanceToPageChange)
            {
                if (swipeOffset >= 0.0f)
                {
                    pagesHandler.CurrentPage = (pageOnSwipeBegin != -1) ? pageOnSwipeBegin + 1 : pagesHandler.CurrentPage + 1;
                }
                else
                {
                    pagesHandler.CurrentPage = (pageOnSwipeBegin != -1) ? pageOnSwipeBegin - 1 : pagesHandler.CurrentPage - 1;
                }

                if (activeGrids.Count > 1)
                {
                    Env.Instance.Sound.PlaySound(AudioKeys.UI.Whoosh);
                }

                SnapScroll();
            }

            pageOnSwipeBegin = -1;
        }
    }


    void SnappingTweener_OnComplete()
    {
        isSnapping = false;
    }


    void OpenAnimation_OnItemAppeared(ShopItem item, ShopItem nextItem, Action onComplete = null)
    {
        if (item != null)
        {
            string audioKey = (item.ItemInfo.AssetType == ContentAsset.AssetType.Shape) ? AudioKeys.UI.DessertUnlock : AudioKeys.UI.Unlocked2;
            Env.Instance.Sound.PlaySound(audioKey);

            if (item.ItemInfo.AssetType == ContentAsset.AssetType.Shape)
            {
                item.IsPreviewForced = false;

                if (nextItem != null)
                    nextItem.IsUnknownForced = false;
            }
            else
            {
                item.IsUnknownForced = false;
            }

            if (!particleEffect.gameObject.activeSelf)
                particleEffect.gameObject.SetActive(true);

            particleEffect.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0.0f);

            particleEffect.Clear();
            particleEffect.Play();

            if (OptionsPanel.IsVibroEnabled)
                MMVibrationManager.Haptic(HapticTypes.LightImpact);

            item.PlayHighlightAnimation(() => 
            {
                touchBlocker.gameObject.SetActive(false);
                particleEffect.Stop();
                onComplete?.Invoke();
            });
        }
        else
        {
            touchBlocker.gameObject.SetActive(false);
            particleEffect.Stop();
            onComplete?.Invoke();
        }
    }

    #endregion
}