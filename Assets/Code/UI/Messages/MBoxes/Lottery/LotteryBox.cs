using System;
using System.Collections;
using System.Collections.Generic;
using Modules.General;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using BoGD;
using TMPro;

public sealed class LotteryBox: UIMessageBox
{
    private static readonly int AnimationStart = Animator.StringToHash("Start");
    private static readonly int AnimationHide = Animator.StringToHash("Hide");
    private static readonly int AnimationShuffle = Animator.StringToHash("Shuffle");
    private static readonly int AnimationIdle = Animator.StringToHash("Idle");
    private static readonly int AnimationClose = Animator.StringToHash("Close");
    private static readonly int[] CardsAnimations = {
        Animator.StringToHash("Card1Rotate"),
        Animator.StringToHash("Card2Rotate"),
        Animator.StringToHash("Card3Rotate"),
        Animator.StringToHash("Card4Rotate"),
        Animator.StringToHash("Card5Rotate"),
    };
    private static readonly int MaterialMainColorProperty = Shader.PropertyToID("_MainColor");
    
    [Header("Cards and video button")]
    [SerializeField] Button videoButton;

    [SerializeField] private GameObject _videoButtonLock;
    [SerializeField] private TextMeshProUGUI _addCardPriceText;
    [SerializeField] List<Card> cards;
    [SerializeField] private int _addCardPrice;
    
    [Space]
    [Header("Continue Button")]
    [SerializeField] Button continueButton;
    [SerializeField] TextMeshProUGUI continueLabel;
    [SerializeField] Image continueImage;

    [Space] [Header("Best Prize")] 
    [SerializeField] GameObject bestPrizeUi;
    [SerializeField] Image bestPrizeImage;

    [Space] [Header("Visual Settings")] 
    [SerializeField] Animator animator;
    [SerializeField] float noThanksDelay = 1.0f;
    [SerializeField] float delayBeforeHide = 0.5f;
    [SerializeField] Color activeColor = Color.white;
    [SerializeField] Color inactiveColor = Color.gray;

    private int openedCards;
    private int attemptsCount;

    private MetagameRoomContext context;
    
    // Key is place index, value is prize index
    private Dictionary<int, int> prizesPositions = new Dictionary<int, int>();
    private List<ContentAsset> prizes;
    private List<int> coinPrizes = new List<int>();
    
    private bool isInteractionsEnabled;
    private bool hasSeenAds;
    private bool wasLastAttempt;

    private Action onCardRotated;
    private Action onFinished;
    private int     cardCount = 0;
    public void Init(MetagameRoomContext context, List<ContentAsset> prizes, Action onFinished)
    {
        this.prizes = prizes;
        this.context = context;
        this.onFinished = onFinished;
        _addCardPriceText.text = _addCardPrice.ToString();
        attemptsCount = BalanceDataProvider.Instance.LotteryCountCardsAtStart;
        
        continueButton.onClick.AddListener(ContinueButton_OnClick);
        videoButton.onClick.AddListener(VideoButton_OnClick);

        // Fill prizes
        while (prizesPositions.Count < prizes.Count)
        {
            var placeIndex = Random.Range(0, cards.Count);
            if (prizesPositions.ContainsKey(placeIndex))
            {
                continue;
            }
            
            var prizeIndex = prizesPositions.Count;
            prizesPositions[placeIndex] = prizeIndex;
            SetCardFace(placeIndex, prizes[prizeIndex].Icon);
        }

        // Fill other places with coins
        for (var placeIndex = 0; placeIndex < cards.Count; placeIndex++)
        {
            if (prizesPositions.ContainsKey(placeIndex))
            {
                continue;
            }

            var amount = Random.Range(2, 20) * BalanceDataProvider.Instance.CoinsMultiplierInLottery;
            SetCoinsCard(placeIndex, amount);
            coinPrizes.Add(amount);
        }
        
        // Select best prize
        if (prizes.Count == 0)
        {
            bestPrizeUi.SetActive(false);
            return;
        }
        
        ContentAsset bestPrize = null;
        for (var i = 0; i < prizes.Count; i++)
        {
            if (bestPrize == null)
            {
                bestPrize = prizes[i];
            }
            else if(prizes[i].GetAssetType() > bestPrize.GetAssetType())
            {
                bestPrize = prizes[i];
            }

            if (bestPrize.GetAssetType() == ContentAsset.AssetType.Valve)
            {
                break;
            }
        }

        bestPrizeImage.sprite = bestPrize.Icon;
        RectTransform rectTransform = bestPrizeImage.rectTransform;
        Rect rect = rectTransform.rect;
        rectTransform.sizeDelta = new Vector2(rect.width,
            rect.width * bestPrize.Icon.rect.height / bestPrize.Icon.rect.width);
        
        CheckBuyButtonAvailable();
    }
    
    private void CheckBuyButtonAvailable()
    {
        _videoButtonLock.SetActive(Env.Instance.Inventory.Bucks < _addCardPrice);
    }


    private void Start()
    {
        animator.SetTrigger(AnimationStart);
    }
    
    private void Update()
    {
        if (isInteractionsEnabled && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            
            var ray = ownCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit, float.MaxValue))
            {
                return;
            }

            var card = hit.transform.GetComponentInParent<Card>();
            if (card == null)
            {
                return;
            }

            CardSelected(card);
        }
    }


    private void CardSelected(Card card)
    {
        if (card.IsOpened)
        {
            return;
        }
        card.IsOpened = true;
        string prizeName = "";
        openedCards++;
        attemptsCount--;
        
        animator.SetBool(AnimationIdle, false);

        var placeIndex = card.transform.GetSiblingIndex();
        if (prizesPositions.ContainsKey(placeIndex) && !hasSeenAds)
        {
            var newPositionForPrize = 0;
            
            do
            {
                newPositionForPrize = Random.Range(0, cards.Count);
            } while (prizesPositions.ContainsKey(newPositionForPrize) || cards[newPositionForPrize].IsOpened);

            prizesPositions[newPositionForPrize] = prizesPositions[placeIndex];
            prizesPositions.Remove(placeIndex);
        }

        if (prizesPositions.ContainsKey(placeIndex))
        {
            var prizeIndex = prizesPositions[placeIndex];
            var prize = prizes[prizeIndex];
            SetCardFace(placeIndex, prize.Icon);

            List<ContentItemInfo> itemsInfos =
                Env.Instance.Content.GetAvailableInfos(prize.GetAssetType(), info => info.Name.Equals(prize.Name));

            if (!itemsInfos.IsNullOrEmpty())
            {
                context.LastItemReceived = itemsInfos[0];
            }
            prizeName = itemsInfos[0].Name;
            Scheduler.Instance.PauseAllMethodForTarget(this);
            onCardRotated = () =>
            {
                Env.Instance.Sound.PlaySound(AudioKeys.UI.CardsWin);
                if (Env.Instance.Rules.Effects.Value)
                {
                    card.Confetti.Clear();
                    card.Confetti.Play();
                }
                
                Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), overlay =>
                {
                    Env.Instance.UI.Messages.ShowContentReceive(context.LastItemReceived, () =>
                    {
                        Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f),
                            nestedOverlay =>
                            {
                                Visible = true;
                                nestedOverlay.Close();
                                Scheduler.Instance.UnpauseAllMethodForTarget(this);
                            });
                    });
                    Visible = false;
                    overlay.Close();
                });
            };
        }
        else
        {
            var rewardIndex = Random.Range(0, coinPrizes.Count);
            var reward = coinPrizes[rewardIndex];
            coinPrizes.RemoveAt(rewardIndex);
            prizeName = "soft_" + reward;
            SetCoinsCard(placeIndex, reward);
            onCardRotated = () =>
            {
                Env.Instance.Sound.PlaySound(AudioKeys.UI.CardsWin);
                if (Env.Instance.Rules.Effects.Value)
                {
                    card.Confetti.Clear();
                    card.Confetti.Play();
                }
                
                
                Env.Instance.Inventory.AddBucks(reward, card.transform, category: "reward", itemId: "lottery");
                CheckBuyButtonAvailable();
            };
        }

        animator.SetTrigger(CardsAnimations[placeIndex]);


        cardCount++;

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["lootbox_id"] = "lottery";
        data["lootbox_count"] = cardCount;
        data["reward"] = prizeName.ToLower().Replace(' ', '_');

        if (attemptsCount >= 1 && openedCards < cards.Count)
        {
            animator.SetBool(AnimationIdle, true);
            return;
        }
        
        isInteractionsEnabled = false;

        foreach (var value in cards)
        {
            if (value.IsOpened) continue;
            value.BackRenderer.material.SetColor(MaterialMainColorProperty, inactiveColor);
        }

        if (openedCards < BalanceDataProvider.Instance.TimesPlayerCanGetAdditionalCard + 1)
        {
            wasLastAttempt = false;
            continueLabel.text = "label_no_thanks".Translate();
            continueImage.color = Color.clear;
            // videoButton.Reset();
            return;
        }

        wasLastAttempt = true;
        continueLabel.text = "label_continue".Translate();
        continueImage.color = Color.white;
        continueButton.transform.position = videoButton.transform.position;      
    }
    
    
    private void SetCardFace(int placeIndex, Sprite icon)
    {
        var card = cards[placeIndex];
        card.ItemRenderer.gameObject.SetActive(true);
        card.CoinsIcon.SetActive(false);
        card.CoinsText.text = "";

        var material = card.ItemRenderer.material;
        var texture = icon.texture;

        material.mainTexture = texture;
        if (texture.width == texture.height)
            return;

        if (texture.width > texture.height)
        {
            var ratio = (float) texture.width / texture.height;
            material.mainTextureScale = new Vector2(1, ratio);
            material.mainTextureOffset = new Vector2(0, (ratio - 1f) / -2f);
        }
        else
        {
            var ratio = (float) texture.height / texture.width;
            material.mainTextureScale = new Vector2(ratio, 1);
            material.mainTextureOffset = new Vector2((ratio - 1f) / -2f, 0);
        }
    }


    private void SetCoinsCard(int placeIndex, int count)
    {
        var card = cards[placeIndex];
        card.ItemRenderer.gameObject.SetActive(false);
        card.CoinsIcon.gameObject.SetActive(true);
        card.CoinsText.text = count.ToString();
    }


    private void ShowNoThanksButton()
    {
        continueButton.gameObject.SetActive(true);
    }


    private void VideoButton_OnClick()
    {
        if (Env.Instance.Inventory.Bucks >= _addCardPrice)
        {
            Env.Instance.Inventory.TrySpendBucks(_addCardPrice);
            
            Scheduler.Instance.UnscheduleMethod(this, ShowNoThanksButton);
        
            animator.SetBool(AnimationIdle, true);
            hasSeenAds = true;
            isInteractionsEnabled = true;
            videoButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
                    
            attemptsCount = BalanceDataProvider.Instance.LotteryCountCardsPerAdShow;
                    
            foreach (var card in cards)
            {
                card.BackRenderer.material.SetColor(MaterialMainColorProperty, activeColor);
            }
        }
        CheckBuyButtonAvailable();
        // videoButton.Reset();
    }
    
    
    private void ContinueButton_OnClick()
    {
        animator.SetTrigger(AnimationClose);
        
        videoButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    private IEnumerator SetTriggerWithDelay(int hash, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger(hash);
    }

    public void Animator_BeforeShown()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.CardsRespawn);
    }

    public void Animator_BeforeClosed()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.CardsDisappear);
    }

    public void Animator_BeforeShuffled()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.CardsShuffle);
    }

    public void Animator_BeforeFlip()
    {
        Env.Instance.Sound.PlaySound(AudioKeys.UI.CardsFlip);
    }
    
    public void Animator_Shown()
    {
        StartCoroutine(SetTriggerWithDelay(AnimationHide, delayBeforeHide));
    }

    public void Animator_Hidden()
    {
        StartCoroutine(SetTriggerWithDelay(AnimationShuffle, delayBeforeHide));
    }

    public void Animator_Shuffled()
    {
        animator.SetBool(AnimationIdle, true);
        isInteractionsEnabled = true;
    }

    public void Animator_Closed()
    {
        Env.Instance.UI.Overlay.Set(this, new Color(0.16f, 0.59f, 0.953f, 1.0f), overlay =>
        {
            onFinished?.Invoke();
            Close();
            overlay.Close();
        }); 
    }

    public void Animator_CardRotated()
    {
        onCardRotated?.Invoke();
        onCardRotated = null;

        if (attemptsCount >= 1 && !wasLastAttempt) return;
        
        videoButton.gameObject.SetActive(!wasLastAttempt);

        if (!wasLastAttempt)
        {
            Scheduler.Instance.CallMethodWithDelay(this, ShowNoThanksButton, noThanksDelay);
            return;
        }

        continueButton.gameObject.SetActive(true);
    }
}
