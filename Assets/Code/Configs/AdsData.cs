using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Configs/Ads Data")]
public class AdsData : ScriptableObject
{
	private static AdsData  instance;

	public static AdsData Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<AdsData>("AsdBalance/AdsData");
			}
			return instance;
		}
	}


	[SerializeField]
	private int			minLevelForInterstitialShowing = 3;
	[SerializeField]
	private int			minLevelForBannerShowing = 3;
	[SerializeField]
	private int			delayBetweenInterstitials = 30;
	[SerializeField]
	private bool		isNeedShowInterstitialAfterResult  = false;
	[SerializeField]
	private bool		isNeedShowInterstitialBeforeResult  = true;
	[SerializeField]
	private bool		isNeedShowInterstitialAfterBackground  = false;
	[SerializeField]
	private bool		isNeedShowInactivityInterstitial = false;
	[SerializeField]
	private float		delayBetweenInactivityInterstitials = 40.0f;
	[SerializeField]
	private bool		isNeedShowSettingsOpenInterstitials  = false;
	[SerializeField]
	private bool		isNeedShowSettingsCloseInterstitials  = false;
	[SerializeField]
	private bool		isNeedShowGalleryOpenInterstitials  = false;
	[SerializeField]
	private bool		isNeedShowGalleryCloseInterstitials  = false;
	[SerializeField]
	private bool		isNeedShowInGameRestartInterstitial  = false;
	[SerializeField]
	private bool		isNeedShow9ChestInterstitial = false;
	[SerializeField]
	private bool		isNeedShowInterstitialAfterSegment = true;


	public int MinLevelForInterstitialShowing => minLevelForInterstitialShowing;
	public int MinLevelForBannerShowing => minLevelForBannerShowing;
	public int DelayBetweenInterstitials => delayBetweenInterstitials;
	public bool IsNeedShowInterstitialAfterResult  => isNeedShowInterstitialAfterResult;
	public bool IsNeedShowInterstitialBeforeResult => isNeedShowInterstitialBeforeResult;
	public bool IsNeedShowInterstitialAfterBackground => isNeedShowInterstitialAfterBackground;
	public bool IsNeedShowInactivityInterstitial => isNeedShowInactivityInterstitial;
	public float DelayBetweenInactivityInterstitials => delayBetweenInactivityInterstitials;
	public bool IsNeedShowSettingsOpenInterstitials => isNeedShowSettingsOpenInterstitials;
	public bool IsNeedShowSettingsCloseInterstitials => isNeedShowSettingsCloseInterstitials;
	public bool IsNeedShowGalleryOpenInterstitials => isNeedShowGalleryOpenInterstitials;
	public bool IsNeedShowGalleryCloseInterstitials => isNeedShowGalleryCloseInterstitials;
	public bool IsNeedShowInGameRestartInterstitial => isNeedShowInGameRestartInterstitial;
	public bool IsNeedShow9ChestInterstitial => isNeedShow9ChestInterstitial;
	public bool IsNeedShowInterstitialAfterSegment => isNeedShowInterstitialAfterSegment;



	//[SerializeField]
	//private List<string> accessPlacements = new List<string>(){"BeforeResult"};
}
