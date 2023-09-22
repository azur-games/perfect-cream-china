using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoGD.UI.PERFECTCREAM;

public class UIMessages
{   
    private Transform root = null;
    private Transform Root
    {
        get
        {
            if (null == root)
            {
                GameObject rootGameObject = new GameObject(this.GetType().ToString());
                GameObject.DontDestroyOnLoad(rootGameObject);
                root = rootGameObject.transform;
            }

            return root;
        }
    }

    private void DestroyRoot()
    {
        if (null == root) return;
        GameObject.Destroy(root.gameObject);
        root = null;
    }

    #region Messages internal
    private List<UIMessageBox> mboxes_stack = new List<UIMessageBox>();

    private T CreateMessageBox<T>() where T : UIMessageBox
    {
        foreach (UIMessageBox uiMessageContent in Env.Instance.UI.Config.MessageBoxes)
        {
            if (uiMessageContent.GetType().Equals(typeof(T)))
            {
                GameObject go = GameObject.Instantiate(uiMessageContent.gameObject);
                go.transform.parent = Root;
                return go.GetComponent<T>();
            }
        }

        return null;
    }

    private void AddMessageBoxToStack(UIMessageBox newMBox)
    {
        foreach (UIMessageBox mBox in mboxes_stack)
        {
            if (mBox.IsPopup == newMBox.IsPopup)
            {
                mBox.Visible = false;
            }
        }

        mboxes_stack.Insert(0, newMBox);
        newMBox.Visible = true;

        newMBox.AddOnCloseDelegate(
            delegate
            {
                OnMessageBoxClose(newMBox);
            });
    }

    private void OnMessageBoxClose(UIMessageBox closedMBox)
    {
        if (!mboxes_stack.Contains(closedMBox))
        {
            return;
        }

        mboxes_stack.Remove(closedMBox);

        foreach (UIMessageBox mBox in mboxes_stack)
        {
            if (mBox.IsPopup == closedMBox.IsPopup)
            {
                mBox.Visible = true;
                break;
            }
        }

        if (0 == mboxes_stack.Count)
        {
            DestroyRoot();
        }
    }

    public void CloseAll()
    {
        List<UIMessageBox> mboxesBuffer = new List<UIMessageBox>(mboxes_stack);
        foreach (UIMessageBox mbox in mboxesBuffer)
        {
            mbox.ForcedCloseWithoutCallbacks();
        }

        DestroyRoot();
    }
    #endregion

    #region Messages
    public RateUsFeedbackPopupScreen ShowRateUs()
    {
        RateUsFeedbackPopupScreen msg = CreateMessageBox<RateUsFeedbackPopupScreen>();
        AddMessageBoxToStack(msg);
        return msg;
    }

    public UIMessageBoxTwoButtons ShowUIMessageBoxTwoButtons(string caption, string text, string button1Text, Action onButton1ClickAction, string button2Text, Action onButton2ClickAction)
    {
        UIMessageBoxTwoButtons msg = CreateMessageBox<UIMessageBoxTwoButtons>();
        msg.Init(caption, text, button1Text, onButton1ClickAction, button2Text, onButton2ClickAction);
        AddMessageBoxToStack(msg);
        return msg;
    }

    public MBoxesDebugWindow ShowMBoxesDebugWindow()
    {
        MBoxesDebugWindow msg = CreateMessageBox<MBoxesDebugWindow>();
        msg.Init();
        AddMessageBoxToStack(msg);
        return msg;
    }

    public InfoBox ShowInfoBox(string description, Action closeCallback)
    {
        InfoBox msg = CreateMessageBox<InfoBox>();
        msg.Init(description);
        msg.AddOnCloseDelegate(closeCallback);
        AddMessageBoxToStack(msg);
        return msg;
    }

    public SingleChestBox ShowSingleChest(ContentAsset prize, MetagameRoomContext mrContext, bool isFree, Action closeCallback)
    {
        SingleChestBox msg = CreateMessageBox<SingleChestBox>();
        msg.Init(prize, mrContext, isFree);
        msg.AddOnCloseDelegate(closeCallback);
        AddMessageBoxToStack(msg);
        return msg;
    }

    public ContentReceiveBox ShowContentReceive(ContentItemInfo itemInfo, Action onGet)
    {
        ContentReceiveBox msg = CreateMessageBox<ContentReceiveBox>();
        msg.Init(itemInfo, onGet);
        AddMessageBoxToStack(msg);
        return msg;
    }


    public ChestsBox ShowChests(Action closeCallback, ContentAsset prize, MetagameRoomContext mrContext)
    {
        ChestsBox msg = CreateMessageBox<ChestsBox>();
        msg.Init(prize, mrContext);
        msg.AddOnCloseDelegate(closeCallback);
        AddMessageBoxToStack(msg);
        return msg;
    }


    public LotteryBox ShowLottery(MetagameRoomContext context, List<ContentAsset> prizes, Action onFinished)
    {
        var message = CreateMessageBox<LotteryBox>();
        message.Init(context, prizes, onFinished);
        AddMessageBoxToStack(message);
        return message;
    }


    public LevelCompleteBox ShowLevelCompletion(int starsCount, 
                                                int moneyCount, 
                                                MetagameRoomContext mrContext, 
                                                LevelCompleteBox.LevelCompleteBoxType type, 
                                                Action<MetagameRoomContext> onHide)
    {
        LevelCompleteBox msg = CreateMessageBox<LevelCompleteBox>();
        msg.Init(starsCount, moneyCount, mrContext, type, onHide);
        AddMessageBoxToStack(msg);
        return msg;
    }



    public LevelFailedBox ShowLevelFail(int score, int bestScore, Action onContinue, Action onAbort)
    {
        LevelFailedBox msg = CreateMessageBox<LevelFailedBox>();
        msg.Init(score, bestScore, onContinue, onAbort);
        AddMessageBoxToStack(msg);
        return msg;
    }


    public LevelResultFailBox ShowLevelResultFailBox(Action onClose = null)
    {
        LevelResultFailBox msg = CreateMessageBox<LevelResultFailBox>();
        msg.Init(onClose);
        AddMessageBoxToStack(msg);
        return msg;
    }


    public ShopBox ShowShopBox(ContentItemInfo itemToHighlight = null)
    {
        ShopBox msg = CreateMessageBox<ShopBox>();
        AddMessageBoxToStack(msg);
        msg.Init(itemToHighlight); // weird stuff
        return msg;
    }


    public InteriorUpgradeBox ShowInteriorUpgradeBox(int level, Action onClose = null)
    {
        InteriorUpgradeBox msg = CreateMessageBox<InteriorUpgradeBox>();
        msg.Init(level, onClose);
        AddMessageBoxToStack(msg);
        return msg;
    }

    public WindowWarningInterstitial ShowWarningInterstitial(string placement, Action<bool> onClose = null)
    {
        WindowWarningInterstitial msg = CreateMessageBox<WindowWarningInterstitial>();
        AddMessageBoxToStack(msg);
        msg.Visible = true; // another weird stuff
        msg.Init(placement, onClose);
        return msg;
    }

    public SubscriptionBox ShowSubscriptionPopup(SubscriptionBox.SubscriptionBoxType type, Action<bool> onClose = null, string message = null, bool isStartSubscription = false, string placement = "")
    {
        SubscriptionBox msg = CreateMessageBox<SubscriptionBox>();
        AddMessageBoxToStack(msg);
        msg.Visible = true; // another weird stuff
        msg.Init(type, onClose, message, isStartSubscription, placement);
        return msg;
    }


    public CoinsPiggyBox ShowCoinsPiggyBox(Action onHide = null)
    {
        CoinsPiggyBox msg = CreateMessageBox<CoinsPiggyBox>();
        msg.Init(onHide);
        AddMessageBoxToStack(msg);
        return msg;
    }


    public CoinsPiggyRewardBox ShowCoinsPiggyRewardBox(Action onHide = null)
    {
        CoinsPiggyRewardBox msg = CreateMessageBox<CoinsPiggyRewardBox>();
        msg.Init(onHide);
        AddMessageBoxToStack(msg);
        return msg;
    }

    #endregion
}
