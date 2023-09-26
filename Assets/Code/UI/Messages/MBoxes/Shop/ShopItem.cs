using DG.Tweening;
using Modules.General;
using Modules.InAppPurchase;
using System;
using UnityEngine;
using UnityEngine.UI;


public class ShopItem : MonoBehaviour
{
    #region Fields

    public static Action OnShopItemSelected;

    [SerializeField] Image background;
    [SerializeField] Image unknownItemIcon;
    [SerializeField] Image itemIcon;
    [SerializeField] Image newIcon;
    [SerializeField] Image premium;
    [SerializeField] Image premiumSelected;
    [SerializeField] Image premiumLock;
    [SerializeField] GameObject selectedFrame;
    [SerializeField] Button button;

    [Space]
    [Header("Show highlight settings")]
    [SerializeField] AnimationCurve highlightItemCurve;
    [SerializeField] float highlightTargetScale;
    [SerializeField] float highlightDuration;

    ContentItemInfo itemInfo = null;
    ContentItemIconRef currentIconRef = null;
    bool isUnknownForced = false;
    bool isPreviewForced = false;

    #endregion



    #region Properties

    public ContentItemInfo ItemInfo
    {
        get => itemInfo;
        set
        {
            if (itemInfo != value)
            {
                UpdateInfo(value);
            }
            else
            {
                UpdateContent();
            }
        }
    }


    public bool IsUnknownForced
    {
        get => isUnknownForced;
        set
        {
            isUnknownForced = value;
            UpdateContent();
        }
    }


    public bool IsPreviewForced
    {
        get => isPreviewForced;
        set
        {
            isPreviewForced = value;
            UpdateContent();
        }
    }


    bool IsSelectable => itemInfo != null && ((itemInfo.AssetType == ContentAsset.AssetType.Valve || 
                                               itemInfo.AssetType == ContentAsset.AssetType.CreamSkin || 
                                               itemInfo.AssetType == ContentAsset.AssetType.Confiture) ||
                                              (IsPremium && IsSubscriptionActive));

    
    bool IsItemAvailable => itemInfo != null && (Env.Instance.Inventory.Contains(itemInfo.Name) || 
                                                (IsPremium && IsSubscriptionActive));


    bool IsUnknown => IsUnknownForced || !IsItemAvailable;

    
    bool IsPremium => itemInfo != null && Env.Instance.Inventory.IsPremiumItem(itemInfo);


    private bool IsSubscriptionActive => true;


    bool IsNextToOpen
    {
        get
        {
            if (itemInfo == null) return false;
            ContentItemsLibrary.ContentItemsCollectionElement nextElement = Env.Instance.Inventory.GetNext(itemInfo.AssetType);
            if (null == nextElement) return false;
            return nextElement.Info.Equals(itemInfo);
        }
    }


    bool IsShape => (itemInfo != null && itemInfo.AssetType == ContentAsset.AssetType.Shape);

    #endregion



    #region Unity lifecycle

    void Awake()
    {
        button.onClick.AddListener(Button_OnClick);
    }


    private void OnEnable()
    {
        OnShopItemSelected += ShopItem_OnShopItemSelected;
    }


    private void OnDisable()
    {
        OnShopItemSelected -= ShopItem_OnShopItemSelected;
    }


    void OnDestroy()
    {
        if (currentIconRef != null)
        {
            Resources.UnloadAsset(currentIconRef);
        }

        DOTween.Kill(this);
    }

    #endregion



    #region Update content

    public void UpdateContent()
    {
        UpdateInfo(itemInfo);
    }


    void UpdateInfo(ContentItemInfo info)
    {
        bool isShape = (info != null && info.AssetType == ContentAsset.AssetType.Shape);

        if (itemInfo != info)
        {
            itemInfo = info;

            isUnknownForced = false;
            isPreviewForced = false;

            if (currentIconRef != null)
            {
                Resources.UnloadAsset(currentIconRef);
                currentIconRef = null;
            }

            if (itemInfo != null)
            {
                currentIconRef = Env.Instance.Content.LoadContentItemIconRef(itemInfo.AssetType, itemInfo.Name);
            }
        }

        UpdateIcon();

        bool isUnknown = IsUnknownForced || !(IsPreviewForced || 
                                              IsItemAvailable || 
                                              (isShape && IsNextToOpen));

        itemIcon.gameObject.SetActive(IsPremium || !isUnknown);
        unknownItemIcon.gameObject.SetActive(!IsPremium && isUnknown);

        bool isNewItem = (itemInfo != null) &&
                         !IsUnknownForced &&
                         !IsPreviewForced &&
                         Env.Instance.Inventory.NewItemsDetector.IsHasUnknowns(itemInfo.Name) && 
                         Env.Instance.Inventory.Contains(itemInfo.Name);
        newIcon.gameObject.SetActive(isNewItem);

        bool isSelected = CheckItemSelected();
        background.gameObject.SetActive(!IsPremium || isSelected);
        premium.gameObject.SetActive(IsPremium && !isSelected);
        // premiumLock.gameObject.SetActive(IsPremium && !IsSubscriptionActive && !isSelected);
        premiumSelected.gameObject.SetActive(IsPremium && isSelected);

        button.interactable = IsPremium || (IsSelectable && !IsUnknown);

        selectedFrame.SetActive(!IsPremium && isSelected && !isUnknown);
    }


    void UpdateIcon()
    {
        if (currentIconRef != null)
        {
            itemIcon.sprite = (IsPreviewForced || (IsShape && IsNextToOpen)) ? currentIconRef.AlternativeIcon : currentIconRef.Icon;
        }
        else
        {
            itemIcon.sprite = null;
        }
    }


    bool CheckItemSelected()
    {
        bool result = false;
        
        if (itemInfo != null)
        {
            switch(itemInfo.AssetType)
            {
                case ContentAsset.AssetType.CreamSkin:
                    result = Env.Instance.Rules.CreamSkin.Value.Equals(ItemInfo.Name);
                    break;
                        
                case ContentAsset.AssetType.Valve:
                    result = Env.Instance.Inventory.ValveName.Equals(itemInfo.Name);
                    break;

                case ContentAsset.AssetType.Confiture:
                    result = Env.Instance.Inventory.CurrentConfiture.Equals(itemInfo.Name);
                    break;
            }
        }

        return result;
    }


    bool TrySelectElement(bool highlight = false)
    {
        if (IsItemAvailable)
        {
            selectedFrame.SetActive(true);
            
            switch(itemInfo.AssetType)
            {
                case ContentAsset.AssetType.CreamSkin:
                    if (null != Env.Instance.Rooms.GameplayRoom)
                    {
                        Env.Instance.Rooms.GameplayRoom.Controller.GetComponentInChildren<CreamCreator>(true).CreamSkinName = itemInfo.Name;
                    }
                    else
                    {
                        Env.Instance.Rules.CreamSkin.Value = itemInfo.Name;
                    }
                    Env.Instance.Inventory.SetCurrentCream(itemInfo.Name, highlight);
                    break;
                
                case ContentAsset.AssetType.Valve:
                    if (null != Env.Instance.Rooms.GameplayRoom)
                    {
                        ValveAsset valve = Env.Instance.Content.LoadContentAsset<ValveAsset>(ContentAsset.AssetType.Valve, itemInfo.Name);
                        Env.Instance.Rooms.GameplayRoom.Controller.Valve.UpdateSkin(valve);
                    };

                    Env.Instance.Inventory.SetCurrentValve(itemInfo.Name, highlight);
                    Env.Instance.Inventory.Save();
                    break;

                case ContentAsset.AssetType.Confiture:
                    Env.Instance.Inventory.SetCurrentConfiture(itemInfo.Name, highlight);
                    break;

                default:
                    break;
            }

            OnShopItemSelected?.Invoke();

            return true;
        }

        return false;
    }

    #endregion



    #region Animations

    public void PlayHighlightAnimation(Action onComplete = null)
    {
        int loopsCounter = 0;

        transform.DOScale(highlightTargetScale, 0.5f * highlightDuration).SetEase(highlightItemCurve)
                                                                         .SetLoops(2, LoopType.Yoyo)
                                                                         .SetAutoKill(true)
                                                                         .SetTarget(this)
                                                                         .OnStepComplete(() =>
                                                                         {
                                                                             loopsCounter++;

                                                                             if (loopsCounter == 1)
                                                                             {
                                                                                 TrySelectElement(true);
                                                                             }
                                                                         })
                                                                         .OnComplete(() => 
                                                                         {
                                                                             onComplete?.Invoke();
                                                                         });
    }

    #endregion



    #region Events handling

    void Button_OnClick()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

        if (IsPremium && !Services.AdvertisingManagerSettings.AdvertisingInfo.IsSubscriptionActive)
        {
            Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.Subscription, (isSubscriptionActivated) => 
            {
                if (isSubscriptionActivated)
                {
                    // Env.Instance.UI.Messages.ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType.SuccessfulSubscription, (isClaimed) =>
                    // {
                    //     if (isClaimed)
                    //     {
                    //         OnShopItemSelected?.Invoke();
                    //     }
                    // });
                }
            }, placement: "click_vip_item");
        }
        else
        {
            TrySelectElement();
        }
    }


    void ShopItem_OnShopItemSelected()
    {
        UpdateContent();
    }

    #endregion
}
