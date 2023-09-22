using Modules.Advertising;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHelper
{
    private bool isSoundEnabled = false;
    public bool IsSoundEnabled
    {
        get
        {
            return isSoundEnabled;
        }

        set
        {
            bool needSave = (isSoundEnabled != value);
            isSoundEnabled = value;

            // if (!IsSoundEnabled)
            //     StopMusic();

            if (needSave)
                CustomPlayerPrefs.SetBool("sound", value);
        }
    }
    
    public bool isMusicEnabled 
    { 
        get => CustomPlayerPrefs.GetBool("MusicButtonState", true); 
        set => CustomPlayerPrefs.SetBool("MusicButtonState", value); 
    }

    private Guid? currentlyPlayingMusic = null;

    public SoundHelper(GameObject soundManagerPrefab)
    {
        GameObject soundManagerInstance = GameObject.Instantiate(soundManagerPrefab);
        GameObject.DontDestroyOnLoad(soundManagerInstance.gameObject);

        IsSoundEnabled = CustomPlayerPrefs.GetBool("sound", true);
        // isMusicEnabled = CustomPlayerPrefs.GetBool("MusicButtonState", true);

        AdvertisingManager.Instance.OnAdShow += AdvertisingManager_OnAdShow;
        AdvertisingManager.Instance.OnAdHide += AdvertisingManager_OnAdHide;
    }

    public bool IsPlaybackActive(Guid guid)
    {
        return SoundManager.Instance.IsPlaybackActive(guid);
    }

    public Guid? PlaySound(string soundId, bool isLooping = false)
    {
        if (!IsSoundEnabled) return null;
        return SoundManager.Instance.PlaySound(soundId, isLooping);
    }

    public void StopSound(Guid? soundGuid)
    {
        if (!soundGuid.HasValue) return;

        if (IsPlaybackActive(soundGuid.Value))
        {
            SoundManager.Instance.StopSound(soundGuid.Value);
        }

        if (currentlyPlayingMusic.HasValue && (currentlyPlayingMusic.Value == soundGuid.Value))
        {
            currentlyPlayingMusic = null;
        }
    }

    public void PlayMusic(string musicId)
    {
        if (!isMusicEnabled) return;
        currentlyPlayingMusic = SoundManager.Instance.PlayMusic(musicId);
    }

    public void StopMusic()
    {
        SoundManager.Instance.StopMusic();
        currentlyPlayingMusic = null;
    }

    public void FadeOutCurrentlyPlayingMusic(float duration = 1.0f)
    {
        if (!currentlyPlayingMusic.HasValue) return;
        if (!IsPlaybackActive(currentlyPlayingMusic.Value)) return;

        SoundManager.Instance.FadeOutSoundByGuid(currentlyPlayingMusic.Value, duration);
    }

    public bool InstanceIfExist
    {
        get
        {
            return SoundManager.InstanceIfExist;
        }
    }


    private void AdvertisingManager_OnAdShow(AdModule adModule, AdActionResultType responseResultType)
    {
        if (adModule == AdModule.Interstitial || adModule == AdModule.RewardedVideo)
        {
            SoundManager.Instance.MuteMusic(true);
            SoundManager.Instance.MuteSounds(true);
        }
    }


    private void AdvertisingManager_OnAdHide(AdModule adModule, AdActionResultType responseResultType)
    {
        if (adModule == AdModule.Interstitial || adModule == AdModule.RewardedVideo)
        {
            SoundManager.Instance.MuteMusic(false);
            SoundManager.Instance.MuteSounds(false);
        }
    }
}
