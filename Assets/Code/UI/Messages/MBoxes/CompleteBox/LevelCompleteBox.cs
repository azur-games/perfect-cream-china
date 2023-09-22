using AbTest;
using DG.Tweening;
using Modules.Advertising;
using Modules.Analytics;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.InAppPurchase;
using MoreMountains.NiceVibrations;
using System;
using System.Collections.Generic;
using BoGD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class LevelCompleteBox : UIMessageBox
{
	#region Nested types

	[Serializable]
	public class LevelCompleteStar
	{
		public Image back;
		public Image icon;
		public ParticleSystem particle;
	}


	public enum LevelCompleteBoxType
	{
		Default = 0,
		Extra
	}


	enum LevelCompleteBoxState
	{
		Bucks = 0,
		Stars
	}

	#endregion



	#region Fields

	[SerializeField]
	private List<Transform>                 bucksConfigObjects;
	[SerializeField]
	private List<Transform>                 starsConfigObjects;
	[SerializeField]
	private List<LevelCompleteStar>         stars;
	[SerializeField]
	private List<ParticleSystem>            confettiParticles;
	[SerializeField]
	private ParticleSystem                  transitionalConfetti;
	[SerializeField]
	private Transform                       headerLabelRoot;
	[SerializeField]
	private VideoButton                     videoButton;
	[SerializeField]
	private Image                           videoButtonBack;
	[SerializeField]
	private ButtonScaleBehaviour            videoButtonScaleBehaviour;
	[SerializeField]
	private Button                          noAdsButton;
	[SerializeField]
	private Spinner                         noAdsSpinner;
	[SerializeField]
	private GlowController                  noAdsGlow;
	[SerializeField]
	private TextMeshProUGUI                 noThanksLabel;
	[SerializeField]
	private Button                          noThanksButton;
	[SerializeField]
	private CanvasGroup                     noThanksCanvasGroup;
	[SerializeField]
	private Image                           noThanksImage;
	[SerializeField]
	private Image                           headerImage;
	[SerializeField]
	private Image                           backgroundImage;
	[SerializeField]
	private Sprite                          freeVideoButtonBack;

	[Space]
	[Header("Color schemes")]
	[SerializeField]
	private Color                           defaultHeaderColor;
	[SerializeField]
	private Color                           defaultBackColor;
	[SerializeField]
	private Sprite                          defaultVideoButtonBack;

	[Space]
	[SerializeField]
	private Color                           extraHeaderColor;
	[SerializeField]
	private Color                           extraBackColor;
	[SerializeField]
	private Sprite                          extraVideoButtonBack;

	[Space]
	[Header("Bucks configuration settings")]
	[SerializeField]
	private Text                            bucksRewardText;
	[SerializeField]
	private Text                            bucksVideoRewardText;
	[SerializeField]
	private Transform                       bucksIconRoot;
	[SerializeField]
	private Transform                       rewardRoot;
	[SerializeField]
	private Transform                       videoButtonBucksConfig;
	[SerializeField]
	private Text                            bucksFreeRewardText;
	[SerializeField]
	private Transform                       freeButtonBucksConfig;

	[Space]
	[Header("Stars configuration settings")]
	[SerializeField]
	private Transform                       shapeProgressRoot;
	[SerializeField]
	private Transform                       videoButtonStarsConfig;
	[SerializeField]
	private Transform                       videoButtonStarsIconRoot;
	[SerializeField]
	private CoinsFlightController           starsFlightController;

	[Space]
	[Header("Animations settings")]
	[SerializeField]
	private float                           initialAnimationDelay = 0.5f;
	[SerializeField]
	private AnimationCurve                  starsScaleCurve;
	[SerializeField]
	private AnimationCurve                  scaleAnimationCurve;
	[SerializeField]
	private float                           starsAnimationDuration = 0.25f;
	[SerializeField]
	private float                           starsAnimationTimeOffset = 0.1f;
	[SerializeField]
	private float                           starsAppearanceDelay = 1.5f;
	[SerializeField]
	private float                           baseElementAnimationDuration = 0.2f;
	[SerializeField]
	private float                           baseElementAnimationTimeOffset = 0.2f;
	[SerializeField]
	private float                           progressAnimationStarsTimeOffset = 0.25f;
	[SerializeField]
	private float                           noThanksButtonDelay = 1.0f;
	[SerializeField]
	private float                           claimDelay = 0.25f;
	[SerializeField]
	private float                           autoCloseDelay = 0.25f;
	[SerializeField]
	private float                           confettiDelay = 0.2f;

	[Space]
	[Header("UA objects")]
	[SerializeField]
	private Text[]                          _texts;

	private LevelCompleteBoxType            currentType = LevelCompleteBoxType.Default;
	private LevelCompleteBoxState           currentState = LevelCompleteBoxState.Stars;
	private Action<MetagameRoomContext>     onHide = null;
	private int                             bucksReward = 0;
	private int                             starsReward = 0;
	private int                             bonusStarsReward = 0;
	private int                             bonusBucksReward = 0;

	private bool                            sawAdsBeforeResults = false;

	private CreamAbTestData                 abTestData;
	private MetagameRoomContext             mrContext;

	private RewardedVideoShowingAdsState    coinsAdsState = RewardedVideoShowingAdsState.None;
	private RewardedVideoShowingAdsState    starsAdsState = RewardedVideoShowingAdsState.None;

	private Vector3                         initialNoThanksButtonPosition;

	private Color                           completionFontColor = Color.white;
	private bool                            chestsShow = false;

	#endregion



	#region Properties
	private int BucksMultiplier
	{
		get
		{
			int result = abTestData.videoCoinsMultiplierOnComplete;
			return (currentType != LevelCompleteBoxType.Extra) ? result : 5;
		}
	}

	private int StarsForNextShape
	{
		get
		{
			int result = Env.Instance.Inventory.GetNextShapeStarsPrice() - Env.Instance.Inventory.Stars;
			return result;
		}
	}


	private RewardedVideoShowingAdsState ShowAdsState => (currentState == LevelCompleteBoxState.Bucks) ? (coinsAdsState) : (starsAdsState);

	#endregion



	#region Unity lifecycle

	protected override void Awake()
	{
		UpdateFontColors(completionFontColor);
		base.Awake();
	}


	private void Start()
	{
		abTestData = new CreamAbTestData();
		initialNoThanksButtonPosition = noThanksButton.transform.position;
		videoButton.Init(AdModule.RewardedVideo, string.Empty, VideoButton_OnClick);
	}


	private void OnEnable()
	{
		noThanksButton.onClick.AddListener(NoThanksButton_OnClick);
		noAdsButton.onClick.AddListener(NoAdsButton_OnClick);

		Env.Instance.Inventory.OnShapeReceived += Inventory_OnShapeReceived;
		chestsShow = false;
	}


	private void OnDisable()
	{
		noThanksButton.onClick.RemoveListener(NoThanksButton_OnClick);
		noAdsButton.onClick.RemoveListener(NoAdsButton_OnClick);

		Env.Instance.Inventory.OnShapeReceived -= Inventory_OnShapeReceived;
		chestsShow = false;
	}


	private void OnDestroy()
	{
		DOTween.Kill(this, true);
	}

	#endregion



	#region Initialization

	public void Init(int starsCount,
					 int moneyCount,
					 MetagameRoomContext mrContext,
					 LevelCompleteBoxType type,
					 Action<MetagameRoomContext> callback)
	{
		this.mrContext = mrContext;

		onHide = callback;

		bucksReward = moneyCount;
		starsReward = starsCount;
		bonusStarsReward = 0;

		currentType = type;

#warning another top notch solution
		for (int i = 0; i < stars.Count; i++)
		{
			stars[i].back.color = stars[i].back.color.SetA(0.0f);
			stars[i].icon.transform.localScale = Vector3.zero;
		}

		coinsAdsState = AdvertisingManager.Instance.GetPlacementSettings(AdsPlacements.RESULTCOINS).showAdsState;
		starsAdsState = AdvertisingManager.Instance.GetPlacementSettings(AdsPlacements.RESULTSTARS).showAdsState;

		ResetStarsConfiguration();
		UpdateColorScheme();

		AdvertisingManager.Instance.TryShowAdByModule(AdModule.Interstitial,
			AdPlacementType.BeforeResult, (result) =>
			{
				sawAdsBeforeResults =
					result == AdActionResultType.Success || result == AdActionResultType.Skip;
				ShowAnimation();
			});
	}

	#endregion



	#region Color schemes

	private void UpdateColorScheme()
	{
		switch (currentType)
		{
			case LevelCompleteBoxType.Extra:
				backgroundImage.color = extraBackColor;
				headerImage.color = extraHeaderColor;
				videoButtonBack.sprite = extraVideoButtonBack;
				break;

			default:
				backgroundImage.color = defaultBackColor;
				headerImage.color = defaultHeaderColor;
				videoButtonBack.sprite = defaultVideoButtonBack;
				break;
		}

		if (ShowAdsState == RewardedVideoShowingAdsState.FreeReward)
		{
			videoButtonBack.sprite = freeVideoButtonBack;
			videoButton.IsFreeRewardAvailable = true;
		}
		videoButton.VideoIcon.enabled = ShowAdsState == RewardedVideoShowingAdsState.Rewarded;
	}

	#endregion



	#region Animations

	private void ResetStarsConfiguration()
	{
		videoButton.IsIteractable = false;
		noThanksButton.interactable = false;

		videoButtonScaleBehaviour.IsEnabled = false;

		DOTween.Complete(noThanksButton);
		DOTween.Kill(noThanksButton);

		videoButton.transform.localScale = Vector3.zero;
		shapeProgressRoot.localScale = Vector3.zero;

		noThanksCanvasGroup.alpha = 0.0f;
	}


	private void ResetBucksConfiguration()
	{
		videoButton.IsIteractable = false;
		noThanksButton.interactable = false;

		videoButtonScaleBehaviour.IsEnabled = false;

		DOTween.Complete(noThanksButton);
		DOTween.Kill(noThanksButton);

		videoButton.transform.localScale = Vector3.zero;
		rewardRoot.localScale = Vector3.zero;

		noThanksCanvasGroup.alpha = 0.0f;
	}


	private Tweener HeaderAnimation()
	{
		headerLabelRoot.localScale = Vector3.zero;

		return headerLabelRoot.DOScale(1.0f, baseElementAnimationDuration).SetEase(scaleAnimationCurve)
																		  .SetTarget(this)
																		  .SetAutoKill(true);
	}


	private Sequence ShowAnimation()
	{
		Env.Instance.Sound.PlaySound(AudioKeys.Gameplay.Fireworks);

		Sequence sequence = DOTween.Sequence();

		sequence.Insert(initialAnimationDelay, HeaderAnimation());

		sequence.InsertCallback(initialAnimationDelay + confettiDelay, () =>
		{
			Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

			int confettiParticlesCount = confettiParticles.Count;
			for (int i = 0; i < confettiParticlesCount; i++)
			{
				if (Env.Instance.Rules.Effects.Value)
				{
					confettiParticles[i].Clear();
					confettiParticles[i].Play();
				}
			}
		});

		sequence.InsertCallback(sequence.Duration() + starsAppearanceDelay, () =>
		{
			UpdateConfiguration(LevelCompleteBoxState.Stars);
		});

		sequence.SetTarget(this);
		sequence.SetAutoKill(true);

		return sequence;
	}


	private Sequence StarsAnimation()
	{
		int starsCount = stars.Count;
		int starsToShow = starsReward;
		starsToShow = (starsToShow <= stars.Count) ? starsToShow : stars.Count;
		starsToShow = (starsToShow >= 0) ? starsToShow : 0;

		ResetStarsConfiguration();

		for (int i = 0; i < stars.Count; i++)
		{
			stars[i].back.color = stars[i].back.color.SetA(0.0f);
			stars[i].icon.transform.localScale = Vector3.zero;
		}

		Sequence sequence = DOTween.Sequence();

		for (int i = 0; i < stars.Count; i++)
		{
			sequence.Insert(0.0f, stars[i].back.DOFade(1.0f, starsAnimationDuration).SetTarget(this).SetAutoKill(true));
		}

		int starIndex = 0;
		for (int i = 0; i < starsToShow; i++)
		{
			string soundKey = null;

			if (i == 0)
			{
				soundKey = AudioKeys.UI.Star01;
			}
			else if (i == 1)
			{
				soundKey = AudioKeys.UI.Star02;
			}
			else if (i == 2)
			{
				soundKey = AudioKeys.UI.Star03;
			}

			sequence.Insert(starsAnimationDuration + i * starsAnimationTimeOffset, stars[i].icon.transform.DOScale(1.0f, starsAnimationDuration).SetEase(starsScaleCurve)
																																				.SetTarget(this)
																																				.SetAutoKill(true)
																																				.OnComplete(() =>
																																				{
																																					if (OptionsPanel.IsVibroEnabled)
																																						MMVibrationManager.Haptic(HapticTypes.LightImpact);

																																					if (Env.Instance.Sound.IsSoundEnabled && soundKey != null)
																																					{
																																						Env.Instance.Sound.PlaySound(soundKey);
																																					}

																																					if (Env.Instance.Rules.Effects.Value)
																																					{
																																						stars[starIndex].particle.Clear();
																																						stars[starIndex].particle.Play();
																																					}
																																					starIndex++;
																																				}));
		}

		sequence.SetTarget(this);
		sequence.SetAutoKill(true);

		return sequence;
	}


	private Sequence StarsConfigurationAnimation(bool isStarsBonusesEnabled)
	{
		ResetStarsConfiguration();

#warning it's just getting retarded
		float resultDuration = 0.0f;
		Sequence sequence = DOTween.Sequence();

		sequence.Insert(resultDuration, shapeProgressRoot.DOScale(1.0f, baseElementAnimationDuration).SetEase(scaleAnimationCurve).SetTarget(this).SetAutoKill(true));
		resultDuration += baseElementAnimationDuration;

		int starIndex = 0;
		int starsForNextShape = StarsForNextShape;
		for (int i = 0; i < starsReward; i++)
		{
			sequence.InsertCallback(resultDuration, () =>
			{
				starsFlightController.ProcessCoinsAnimation(stars[starIndex++].icon.transform, () =>
				{
					if (OptionsPanel.IsVibroEnabled)
						MMVibrationManager.Haptic(HapticTypes.LightImpact);

					Env.Instance.Inventory.AddStars(1);
				});
			});

			resultDuration += progressAnimationStarsTimeOffset;

			if (i == starsForNextShape - 1)
			{
#warning mmmm, vkysno-vkysno
				resultDuration += starsFlightController.AnimationTime;
			}
		}

		if (isStarsBonusesEnabled)
		{
			resultDuration += baseElementAnimationTimeOffset;

			sequence.Insert(resultDuration, videoButton.transform.DOScale(1.0f, baseElementAnimationDuration).SetEase(scaleAnimationCurve)
																											 .SetTarget(this)
																											 .SetAutoKill(true)
																											 .OnComplete(() =>
																											 {
																												 videoButton.Reset(VideoPlacement());
																												 videoButton.IsIteractable = true;
																												 videoButtonScaleBehaviour.IsEnabled = true;
																											 }));
			resultDuration += baseElementAnimationDuration;
			resultDuration += noThanksButtonDelay;
		}

		sequence.Insert(resultDuration, noThanksCanvasGroup.DOFade(1.0f, baseElementAnimationDuration).SetTarget(this)
																									  .SetAutoKill(true)
																									  .OnStart(() =>
																									  {
																										  noThanksButton.interactable = true;
																									  }));

		sequence.SetTarget(this);
		sequence.SetAutoKill(true);

		return sequence;
	}


	private Sequence BucksConfigurationAnimation(bool isCoinsBonusesEnabled)
	{
		ResetBucksConfiguration();

#warning it's just getting retarded
		float resultDuration = 0.0f;
		Sequence sequence = DOTween.Sequence();

		sequence.Insert(resultDuration, rewardRoot.DOScale(1.0f, baseElementAnimationDuration).SetEase(scaleAnimationCurve).SetTarget(this).SetAutoKill(true));
		resultDuration += baseElementAnimationDuration;

		if (isCoinsBonusesEnabled)
		{
			sequence.Insert(resultDuration, videoButton.transform.DOScale(1.0f, baseElementAnimationDuration).SetEase(scaleAnimationCurve)
																											.SetTarget(this)
																											.SetAutoKill(true)
																											.OnComplete(() =>
																											{
																												videoButton.IsIteractable = true;
																												videoButton.Reset(VideoPlacement());
																												videoButtonScaleBehaviour.IsEnabled = true;
																											}));
			resultDuration += baseElementAnimationDuration;
			resultDuration += noThanksButtonDelay;
		}

		sequence.Insert(resultDuration, noThanksCanvasGroup.DOFade(1.0f, baseElementAnimationDuration).SetTarget(this)
																									  .SetAutoKill(true)
																									  .OnStart(() =>
																									  {
																										  noThanksButton.interactable = true;
																									  }));

		sequence.SetTarget(this);
		sequence.SetAutoKill(true);

		return sequence;
	}


	Sequence BonusStarsAnimation()
	{
		Sequence sequence = DOTween.Sequence();
		float duration = 0.0f;
		int starsForNextShape = StarsForNextShape;

		for (int i = 0; i < bonusStarsReward; i++)
		{
			int starIndex = i;
			sequence.InsertCallback(duration, () =>
			{
				string soundKey = null;
				switch (starIndex)
				{
					case 0: soundKey = AudioKeys.UI.StarMini1; break;
					case 1: soundKey = AudioKeys.UI.StarMini2; break;
					case 2: soundKey = AudioKeys.UI.StarMini3; break;
				}

				if (!string.IsNullOrEmpty(soundKey))
				{
					Env.Instance.Sound.PlaySound(soundKey);
				}

				starsFlightController.ProcessCoinsAnimation(videoButtonStarsIconRoot, () =>
				{
					if (OptionsPanel.IsVibroEnabled)
						MMVibrationManager.Haptic(HapticTypes.LightImpact);

					Env.Instance.Inventory.AddStars(1);
				});
			});

			duration += i * progressAnimationStarsTimeOffset;

			if (i == starsForNextShape - 1)
			{
#warning mmmm, vkysno-vkysno
				duration += starsFlightController.AnimationTime;
			}
		}

		sequence.SetTarget(this);
		sequence.SetAutoKill(true);

		return sequence;
	}

	#endregion



	#region Update configuration

	private void UpdateBucksReward()
	{
		bucksRewardText.text = (bucksReward + bonusBucksReward).ToString();
	}


	private void UpdateVideoButton()
	{
		switch (currentState)
		{
			case LevelCompleteBoxState.Bucks:
				bool isFreeBucks = ShowAdsState == RewardedVideoShowingAdsState.FreeReward;
				videoButtonBucksConfig.gameObject.SetActive(!isFreeBucks);
				videoButtonStarsConfig.gameObject.SetActive(false);
				freeButtonBucksConfig.gameObject.SetActive(isFreeBucks);
				break;

			case LevelCompleteBoxState.Stars:
				videoButtonBucksConfig.gameObject.SetActive(false);
				videoButtonStarsConfig.gameObject.SetActive(true);
				freeButtonBucksConfig.gameObject.SetActive(false);
				break;

			default:
				videoButtonBucksConfig.gameObject.SetActive(false);
				videoButtonStarsConfig.gameObject.SetActive(false);
				freeButtonBucksConfig.gameObject.SetActive(false);
				break;
		}
	}


	private void UpdateConfiguration(LevelCompleteBoxState nextState)
	{
		DisableAllConfigurations();

		UpdateColorScheme();

		switch (nextState)
		{
			case LevelCompleteBoxState.Bucks:
				EnableBucksConfiguration();
				break;

			case LevelCompleteBoxState.Stars:
				EnableStarsConfiguration();
				break;

			default:
				videoButton.IsIteractable = true;
				break;
		}
	}


	private void EnableBucksConfiguration()
	{
		currentState = LevelCompleteBoxState.Bucks;

		UpdateColorScheme();
		UpdateVideoButton();

		foreach (Transform obj in bucksConfigObjects)
		{
			obj.gameObject.SetActive(true);
		}

		UpdateVideoButton();

		bool isCoinsBonusesEnabled = ShowAdsState != RewardedVideoShowingAdsState.None;
		videoButton.gameObject.SetActive(isCoinsBonusesEnabled);

		noAdsButton.gameObject.SetActive(false);
		noThanksButton.gameObject.SetActive(ShowAdsState != RewardedVideoShowingAdsState.FreeReward);

		if (!isCoinsBonusesEnabled)
		{
			noThanksLabel.text = "label_continue".Translate();
			noThanksButton.transform.position = videoButton.transform.position;
			noThanksImage.color = Color.white;
		}
		else
		{
			noThanksLabel.text = "NO, THANKS";
			noThanksButton.transform.position = initialNoThanksButtonPosition;
			noThanksImage.color = Color.clear;
		}

		bucksRewardText.text = bucksReward.ToString();
		bucksVideoRewardText.text = (BucksMultiplier * bucksReward).ToString();
		bucksFreeRewardText.text = (BucksMultiplier * bucksReward).ToString();

		BucksConfigurationAnimation(isCoinsBonusesEnabled).Play();
	}


	private void EnableStarsConfiguration()
	{
		currentState = LevelCompleteBoxState.Stars;

		UpdateColorScheme();
		UpdateVideoButton();

		foreach (Transform obj in starsConfigObjects)
		{
			obj.gameObject.SetActive(true);
		}

		IAdvertisingNecessaryInfo adInfo = Services.AdvertisingManagerSettings.AdvertisingInfo;
		int extraLevelPeriod = abTestData.extraLevelsPeriod;
		int levelsPeriodToShowNoAds = abTestData.levelsPeriodToShowNoAds;

		bool isNoAdsEnabled = (Env.Instance.Inventory.CurrentLevelIndex % levelsPeriodToShowNoAds == 0) &&
			(Env.Instance.Inventory.CurrentLevelIndex % extraLevelPeriod != 0) &&
			!adInfo.IsNoAdsActive &&
			!adInfo.IsSubscriptionActive &&
			sawAdsBeforeResults &&
			abTestData.isNoAdsButtonOnResultsScreenEnabled &&
			abTestData.inAppsEnabled;
		bool isStarsBonusesEnabled = ShowAdsState != RewardedVideoShowingAdsState.None && !isNoAdsEnabled;

		noAdsButton.gameObject.SetActive(isNoAdsEnabled);
		videoButton.gameObject.SetActive(isStarsBonusesEnabled);
		noThanksButton.gameObject.SetActive(ShowAdsState != RewardedVideoShowingAdsState.FreeReward ||
											isNoAdsEnabled);

		if (!isStarsBonusesEnabled && !isNoAdsEnabled)
		{
			noThanksLabel.text = "label_continue".Translate();
			noThanksButton.transform.position = videoButton.transform.position;
			noThanksImage.color = Color.white;
		}
		else
		{
			noThanksLabel.text = "NO, THANKS";
			noThanksButton.transform.position = initialNoThanksButtonPosition;
			noThanksImage.color = Color.clear;
		}

		StarsAnimation().OnComplete(() =>
		{
			StarsConfigurationAnimation(isStarsBonusesEnabled).Play();
		}).Play();
	}


	private void DisableAllConfigurations()
	{
		foreach (Transform obj in bucksConfigObjects)
		{
			obj.gameObject.SetActive(false);
		}

		foreach (Transform obj in starsConfigObjects)
		{
			obj.gameObject.SetActive(false);
		}
	}

	#endregion



	#region Reward

	private void MultiplyReward()
	{
		switch (currentState)
		{
			case LevelCompleteBoxState.Bucks:
				bonusBucksReward = (BucksMultiplier - 1) * bucksReward;
				break;

			case LevelCompleteBoxState.Stars:
				bonusStarsReward = 3;
				break;

			default:
				break;
		}
	}

	#endregion



	#region Monetization placements

	private string VideoPlacement()
	{
		string result = "";

		switch (currentState)
		{
			case LevelCompleteBoxState.Bucks:
				result = "complete_bucks";
				break;

			case LevelCompleteBoxState.Stars:
				result = "complete_stars";
				break;

			default:
				break;
		}

		return result;
	}

	#endregion



	#region Move next

	private void MoveNext()
	{
		switch (currentState)
		{
			case LevelCompleteBoxState.Stars:
				Env.Instance.Sound.PlaySound(AudioKeys.UI.ConfettiDrop);

				if (Env.Instance.Rules.Effects.Value)
				{
					transitionalConfetti.Clear();
					transitionalConfetti.Play();
				}

				UpdateConfiguration(LevelCompleteBoxState.Bucks);
				break;

			default:
				GoToNextState(onHide);
				break;
		}
	}


	private void GoToNextState(Action<MetagameRoomContext> callback)
	{
		AdvertisingManager.Instance.TryShowAdByModule(AdModule.Interstitial, AdPlacementType.AfterResult,
			(result) =>
			{
				AutonomousTimer.Create(0.0f, () =>
				{
					GoToMetagame(callback);
				});
			});
	}


	private void GoToMetagame(Action<MetagameRoomContext> callback)
	{
		ContentAsset prize = Env.Instance.Inventory.Delivery.GetPrize(true);
		Debug.Log("Prize for chests:" + (prize ? prize.Name : "Null"));
		if ((Env.Instance.Inventory.Keys >= 3) && abTestData.isNeedShowKeysChests)
		{
			if (!chestsShow)
			{
				Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
				{
					Env.Instance.UI.Messages.ShowChests(() =>
					{
						Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlayUi) =>
						{
							Close();
							callback?.Invoke(mrContext);
							overlayUi.Close();
						});
					}, prize, mrContext);

					overlay.Close();
				});
				chestsShow = true;
			}
		}
		else
		{
			Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
			{
				overlay.Close();

				Close();
				callback?.Invoke(mrContext);
			});
		}
	}

	#endregion



	#region Events handling

	void VideoButton_OnClick()
	{
		Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

		noThanksButton.interactable = false;

		if (ShowAdsState == RewardedVideoShowingAdsState.Rewarded)
		{
			AdvertisingManager.Instance.TryShowAdByModule(AdModule.RewardedVideo, VideoPlacement(), RewardedVideo_OnShown);
		}
		else
		{
			RewardedVideo_OnShown(AdActionResultType.Success);
		}
	}


	private void NoThanksButton_OnClick()
	{
		Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

		videoButton.IsIteractable = false;
		noThanksButton.interactable = false;

		DOTween.Kill(this, true);
		noThanksCanvasGroup.alpha = 0.0f;

		var sum = bucksReward + bonusBucksReward;
		if (currentState == LevelCompleteBoxState.Bucks && bonusBucksReward == 0)
		{
			Env.Instance.Inventory.AddBucks(bucksReward + bonusBucksReward, bucksIconRoot, () =>
			{
				if (OptionsPanel.IsVibroEnabled)
					MMVibrationManager.Haptic(HapticTypes.LightImpact);

				DOTween.Sequence().InsertCallback(claimDelay, () =>
				{
					MoveNext();
				}).SetTarget(this).SetAutoKill(true).Play();

				Env.Instance.SendResultWindow("win", "close", sum, 0);
			}, category: "reward", itemId: "level_complete");
		}
		else
		{
			MoveNext();
		}
	}

	void NoAdsButton_OnClick()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopupManager.Instance.ShowMessagePopup(messageHandler: transform);
			return;
		}
		if (!Services.StoreManager.IsInitialized)
		{
			PopupManager.Instance.ShowMessagePopup(message: "Store is not initialized yet. Try after 5 seconds", messageHandler: transform);
			return;
		}
		Env.Instance.Sound.PlaySound(AudioKeys.UI.Click);

		noAdsButton.interactable = false;
		noAdsGlow.gameObject.SetActive(false);
		noAdsSpinner.Show();

		onPurchasedCallbackEvent = OnRestorePurchaseCallback;
		IAPsItemsHandler.Instance.NoAds.Purchase(purchaseItemResult =>
		{
			onPurchasedCallbackEvent?.Invoke(purchaseItemResult);
			return true;
		});
	}

	private Action<IPurchaseItemResult> onPurchasedCallbackEvent = null;

	private void OnRestorePurchaseCallback(IPurchaseItemResult purchaseItemResult)
	{
		onPurchasedCallbackEvent = null;
		noAdsSpinner.Hide();

		if (purchaseItemResult.IsSucceeded)
		{
			Env.Instance.UI.Messages.ShowInfoBox("Purchase completed successfully", () => { });
			noAdsButton.gameObject.SetActive(false);
		}
		else
		{
			noAdsGlow.gameObject.SetActive(true);
			noAdsButton.interactable = true;
		}
	}

	private void RewardedVideo_OnShown(AdActionResultType type)
	{
		if (type == AdActionResultType.Success)
		{
			MultiplyReward();
			UpdateBucksReward();

			videoButton.gameObject.SetActive(false);

			if (currentState == LevelCompleteBoxState.Stars)
			{
				Sequence animation = BonusStarsAnimation();
				float delay = autoCloseDelay;

				animation.InsertCallback(animation.Duration() + delay, () =>
				{
					MoveNext();
				}).Play();

				//Env.Instance.SendResultWindow("win", "get_stars", 0, bonusStarsReward);
			}
			else
			{
				Env.Instance.Inventory.AddBucks(bucksReward + bonusBucksReward, bucksIconRoot, () =>
				{
					if (OptionsPanel.IsVibroEnabled)
						MMVibrationManager.Haptic(HapticTypes.LightImpact);

					DOTween.Sequence().InsertCallback(claimDelay, () =>
					{
						MoveNext();
					}).SetAutoKill(true).SetTarget(this).Play();
				}, category: "reward", itemId: "level_complete");

				noThanksButton.interactable = false;

				var sum = bucksReward + bonusBucksReward;

				Env.Instance.SendResultWindow("win", "get_x2", sum, 0);
			}
		}
		else
		{
			noThanksButton.interactable = true;
			videoButton.Reset();

			if (type == AdActionResultType.NoInternet)
			{
				Env.Instance.UI.Messages.ShowInfoBox("No video available", () => { });
			}

			UpdateVideoButton();
		}
	}


	private void Inventory_OnShapeReceived(string shapeName)
	{
		Env.Instance.UI.Overlay.Set(this, new Color(0.1607f, 0.5921f, 0.9568f, 1.0f), (overlay) =>
		{
			DOTween.Pause(this);

			Env.Instance.UI.Messages.ShowContentReceive(Env.Instance.Content.GetItem(ContentAsset.AssetType.Shape, shapeName).Info, () =>
			{
				DOTween.Play(this);

				List<ContentItemInfo> itemsInfos = Env.Instance.Content.GetAvailableInfos(ContentAsset.AssetType.Shape, (info) =>
				{
					return info.Name.Equals(shapeName);
				});

				if (!itemsInfos.IsNullOrEmpty())
				{
					mrContext.LastItemReceived = itemsInfos.First();
				}
			});

			overlay.Close();
		});
	}

	#endregion



	#region UA Fonts

	private void UpdateFontColors(Color color)
	{
		foreach (var t in _texts)
			t.color = color;
	}

	#endregion
}
