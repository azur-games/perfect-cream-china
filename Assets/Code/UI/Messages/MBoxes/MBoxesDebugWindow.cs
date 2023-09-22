using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MBoxesDebugWindow : UIMessageBox
{
    private const int LEVEL_COMPLETE_MONEY_REWARD = 35;
    #region Fields

    [SerializeField] Button buttonClose;

    [SerializeField] Button buttonResult1Star;
    [SerializeField] Button buttonResult2Stars;
    [SerializeField] Button buttonResult3Stars;
    [SerializeField] Button buttonResultNoStars;

    [SerializeField] Button buttonResult1StarExtra;
    [SerializeField] Button buttonResult2StarsExtra;
    [SerializeField] Button buttonResult3StarsExtra;
    [SerializeField] Button buttonResultNoStarsExtra;

    [SerializeField] Button SingleChest;
    [SerializeField] Button NineChests;
    [SerializeField] Button lottery;

    [SerializeField] Button CoinsPiggy;
    [SerializeField] Button CoinsPiggyReward;

    #endregion


    public void Init()
    {
        buttonClose.onClick.AddListener(Close);

        buttonResult1Star.onClick.AddListener(() => { ShowLevelComplete(1, false); });
        buttonResult2Stars.onClick.AddListener(() => { ShowLevelComplete(2, false); });
        buttonResult3Stars.onClick.AddListener(() => { ShowLevelComplete(3, false); });
        buttonResultNoStars.onClick.AddListener(() => { ShowLevelComplete(0, false); });

        buttonResult1StarExtra.onClick.AddListener(() => { ShowLevelComplete(1, true); });
        buttonResult2StarsExtra.onClick.AddListener(() => { ShowLevelComplete(2, true); });
        buttonResult3StarsExtra.onClick.AddListener(() => { ShowLevelComplete(3, true); });
        buttonResultNoStarsExtra.onClick.AddListener(() => { ShowLevelComplete(0, true); });

        SingleChest.onClick.AddListener(() => 
        {
            MetagameRoomContext mrContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.None);
            ContentAsset prize = Env.Instance.Inventory.Delivery.GetPrize(false);
            if (null != prize)
            {
                Env.Instance.UI.Overlay.Set(this, new Color(0.16f, 0.59f, 0.953f, 1.0f), (overlay) =>
                {
                    Env.Instance.UI.Messages.ShowSingleChest(prize, mrContext, false, () =>
                    {
                        
                    });

                    overlay.Close();
                });
            }
        });

        NineChests.onClick.AddListener(OnNineChestsClick);


        CoinsPiggy.onClick.AddListener(() => 
        {
            ShowCoinsPiggy();
        });

        CoinsPiggyReward.onClick.AddListener(() => 
        {
            ShowCoinsPiggyReward();
        });
        
        lottery.onClick.AddListener(() =>
        {
            var context = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.None);
            List<ContentAsset> prizes = Env.Instance.Inventory.Delivery.GetPrizes(2);            
            
            Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), overlay =>
            {
                Env.Instance.UI.Messages.ShowLottery(context, prizes, null);
                overlay.Close();
            });
        });
    }


    private void OnNineChestsClick()
    {
        MetagameRoomContext mrContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.None);
        ContentAsset prize = Env.Instance.Inventory.Delivery.GetPrize(true);
        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            Env.Instance.UI.Messages.ShowChests(() =>
            {
            }, prize, mrContext);
            overlay.Close();
        });
    }

    private void ShowLevelComplete(int starsCount, bool isExtra)
    {
        if (0 == starsCount)
        {
            ShowLevelFailed(isExtra);
            return;
        }

        MetagameRoomContext mrContext = new MetagameRoomContext(MetagameRoomContext.GameplaySessionResult.None);
        LevelCompleteBox.LevelCompleteBoxType screenType = isExtra ? 
            LevelCompleteBox.LevelCompleteBoxType.Extra :
            LevelCompleteBox.LevelCompleteBoxType.Default;

        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            Env.Instance.UI.Messages.ShowLevelCompletion(starsCount, LEVEL_COMPLETE_MONEY_REWARD, mrContext, screenType, (levelCompleteBoxContext) =>
            {
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
                {
                    overlayInstance.Close();
                });
            });

            overlay.Close();
        });
    }

    private void ShowLevelFailed(bool isExtra)
    {
        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            Env.Instance.UI.Messages.ShowLevelResultFailBox(() =>
            {
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayInstance) =>
                {
                    overlayInstance.Close();
                });
            });

            overlay.Close();
        });
    }


    private void ShowCoinsPiggy()
    {
        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
        {
            Env.Instance.UI.Messages.ShowCoinsPiggyBox();

            overlay.Close();
        });
    }


    private void ShowCoinsPiggyReward()
    {
        Env.Instance.UI.Messages.ShowCoinsPiggyRewardBox();
    }
}
