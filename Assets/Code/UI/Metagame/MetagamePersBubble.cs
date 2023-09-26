using AbTest;
using Modules.Advertising;
using Modules.General.Abstraction;
using System;
using UnityEngine;


public class MetagamePersBubble : MonoBehaviour
{
    private const string PREFS_KEY = "METAGAME_BUBBLE_SHOWLEVEL";
    private const float TIMEDELAY_BEFORE_POPUP = 2.0f;
    [SerializeField] WatchVideoBubble persBubble;

    public Action<ContentItemInfo> OnNewShape;

    private float startTime = 0.0f;
    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        UpdatePersBubble();
    }

    private void UpdatePersBubble()
    {
        if ((Time.realtimeSinceStartup - startTime) < TIMEDELAY_BEFORE_POPUP) return;
        if (null == Env.Instance.Rooms.MetagameRoom.Controller) return;

        bool videoAvailable = true;
        if (persBubble.IsShowing)
        {
            if (!videoAvailable)
            {
                persBubble.Hide();
            }

            return;
        }

        if (!videoAvailable)
        {
            return;
        }

        if (AdvertisingManager.Instance.GetPlacementSettings(AdsPlacements.BUBBLE)
            .showAdsState != RewardedVideoShowingAdsState.Rewarded)
        {
            return;
        }

        int lastShowingBubbleLevel = PlayerPrefs.GetInt(PREFS_KEY, -1);
        if (lastShowingBubbleLevel == Env.Instance.Inventory.CurrentLevelIndex) return;

        ContentItemsLibrary.ContentItemsCollectionElement contentElement = Env.Instance.Inventory.GetNext(ContentAsset.AssetType.Shape);
        if (null == contentElement)
        {
            if (persBubble.IsShowing) persBubble.Hide();
            return;
        }

        Vector3? bubblePivotPoint = ScenePivot.GetPivotViewPoint(Env.Instance.Rooms.MetagameRoom.Controller.Pivots, ScenePivot.Subject.MetaPersBubble);
        if (!bubblePivotPoint.HasValue) return;

        MetagameRoomUI metagameUI = gameObject.GetComponentInParent<MetagameRoomUI>();
        Vector3 bubblePoint = metagameUI.UICamera.ViewportToScreenPoint(new Vector3(bubblePivotPoint.Value.x - 0.5f, bubblePivotPoint.Value.y - 0.5f, bubblePivotPoint.Value.z));

        ContentItemIconRef currentItemIconRef = Env.Instance.Content.LoadContentItemIconRef(contentElement.Info.AssetType, contentElement.Info.Name);

        Env.Instance.Sound.PlaySound(AudioKeys.UI.PopupCloud);

        persBubble.Show(bubblePoint, currentItemIconRef.Icon, contentElement.Info.Name, () =>
        {
            PlayerPrefs.SetInt(PREFS_KEY, Env.Instance.Inventory.CurrentLevelIndex);
            Env.Instance.Inventory.ReceiveShape(contentElement.Info.Name, isAds: true);
            OnNewShape?.Invoke(contentElement.Info);
        });
    }
}
