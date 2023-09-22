using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using Modules.General.Obsolete;


public class SoundsSourceController : MonoBehaviour, IPoolCallback
{
    #region Fields

    ISoundSourceContainer simpleSourceContainer;
    
    #endregion


    #region Properties

    public bool IsAnyPlaybackActive
    {
        get 
		{ 
			return simpleSourceContainer.IsAnyPlaybackActive(); 
		}
    }

    #endregion



    #region Unity lifecycle

    void Awake()
    {
        simpleSourceContainer = new SoundSourceContainer<UnityAudioSourceWrapper>(transform);
    }


    void Update()
    {
        float deltaTime = Time.deltaTime;

        simpleSourceContainer.CustomUpdate(deltaTime);
    }

    #endregion


    #region Public methods

    public Guid PlaySound(SoundConfig sound, bool isLooping, bool isMuted = false, bool ignoreSourcesLimit = false, AudioMixerGroup mixerGroup = null)
    {
        Guid resultGuid = Guid.Empty;
        
		IAudioSourceWrapper sourceToPlay = simpleSourceContainer.PopFreeSource(ignoreSourcesLimit);
        if (sourceToPlay != null)
        {
            resultGuid = Guid.NewGuid();

            sourceToPlay.isEnabled = true;
            sound.ApplySelfToSource(sourceToPlay);
            sourceToPlay.loop = isLooping;   
            sourceToPlay.audioMixerGroup = mixerGroup;
            if (isMuted)
            {  
                sourceToPlay.volume = 0f;
            }

			simpleSourceContainer.WatchSource(resultGuid, sourceToPlay);

            sourceToPlay.Play();
        }

        return resultGuid;
    }

    
    public Guid PlayOneShot(SoundConfig sound, float volume, bool ignoreSourcesLimit = false, AudioMixerGroup mixerGroup = null)
    {
        Guid resultGuid = Guid.Empty;
        
        IAudioSourceWrapper sourceToPlay = simpleSourceContainer.PopFreeSource(ignoreSourcesLimit);
        if (sourceToPlay != null)
        {
            resultGuid = Guid.NewGuid();

            sourceToPlay.isEnabled = true;
            sourceToPlay.loop = false;
            sourceToPlay.audioMixerGroup = mixerGroup;
            

            sound.ApplySelfToSource(sourceToPlay);

            if (Mathf.Approximately(volume, 0f))
            {
                sourceToPlay.volume = 0f;
            }

            simpleSourceContainer.WatchSource(resultGuid, sourceToPlay);

            sourceToPlay.PlayOneShot(volume);
        }

        return resultGuid;
    }
    

    public void StopSoundByGuid(Guid guid)
    {           
        simpleSourceContainer.StopSoundByGuid(guid);
    }


    public void RunActionForSoundByGuid(Guid guid, Action<IAudioSourceWrapper, double> action, float duration, Action<Guid> callbackSoundActionEnded)
    {
        simpleSourceContainer.RunActionForSoundByGuid(guid, action, duration, callbackSoundActionEnded);
    }


    public void ChangeVolumeSmoothly(Guid guid, float targetVolume, float duration, Action<Guid> callbackSoundVolumeChanged)
    {
        simpleSourceContainer.ChangeVolumeSmoothly(guid, targetVolume, duration, callbackSoundVolumeChanged);
    }


    public void FadeInSoundByGuid(Guid guid, float targetVolume, float duration, Action<Guid> callbackSoundFinishFadeIn)
    {
        simpleSourceContainer.FadeInSoundByGuid(guid, targetVolume, duration, callbackSoundFinishFadeIn);
    }


    public void FadeOutSoundByGuid(Guid guid, float duration, Action<Guid> callbackSoundFinishFadeOut)
    {
        simpleSourceContainer.FadeOutSoundByGuid(guid, duration, callbackSoundFinishFadeOut);
    }


    public void StopAllSounds()
    {
        simpleSourceContainer.StopAllSounds();
    }


    public void SetPlaybackVolume(Guid playbackGuid, float targetVolume)
    {
        IAudioSourceWrapper source = simpleSourceContainer.GetSourceByPlaybackGuid(playbackGuid);      
		source.volume = targetVolume;
    }


    public void SetPlaybackPauseByGuid(Guid guid, bool isPlay)
    {
        simpleSourceContainer.SetPlaybackPauseByGuid(guid, isPlay);
    }


    public IAudioSourceWrapper GetSourceByPlaybackGuid(Guid guid)
    {
        return simpleSourceContainer.GetSourceByPlaybackGuid(guid);
    }
   
    #endregion



    #region IPoolCallback

    public void OnCreateFromPool() {}


    public void OnReturnToPool() 
    {
        simpleSourceContainer.OnReturnToPool();
    }


    public void OnPop() {}


    public void OnPush() { }

    #endregion
}
