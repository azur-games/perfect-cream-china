using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;


class SoundSourceContainer<TAudioSource> : ISoundSourceContainer where TAudioSource : IAudioSourceWrapper
{
    #region Nested types

    struct AudioSourceRemoveEntry
    {
        public Guid playbackGuid;
        public IAudioSourceWrapper sourceWrapper;
    }

    #endregion



    #region Fields

    const int SOURCES_LIMIT = 25;

    Transform sourcesParent;

    List<IAudioSourceWrapper> managedSources = new List<IAudioSourceWrapper>();
    List<IAudioSourceWrapper> freeSources = new List<IAudioSourceWrapper>();

    Dictionary<Guid, IAudioSourceWrapper> playingSingleSources = new Dictionary<Guid, IAudioSourceWrapper>();
    Dictionary<Guid, IAudioSourceWrapper> playingLoopSources = new Dictionary<Guid, IAudioSourceWrapper>();

    List<AudioSourceRemoveEntry> sourcesToRemove = new List<AudioSourceRemoveEntry>();
    HashSet<Guid> pausedPlaybackGuids = new HashSet<Guid>();

    #endregion



    #region Constructors

    public SoundSourceContainer(Transform sourcesParent)
    {
        this.sourcesParent = sourcesParent;
    }

    #endregion



    #region Public methods

    public void CustomUpdate(float deltaTime)
    {
        foreach (var singleSourcePair in playingSingleSources)
        {
            if (!singleSourcePair.Value.isPlaying && !pausedPlaybackGuids.Contains(singleSourcePair.Key))
            {
                sourcesToRemove.Add(new AudioSourceRemoveEntry
                {
                    playbackGuid = singleSourcePair.Key,
                    sourceWrapper = singleSourcePair.Value
                });
            }
        }

        for (int i = 0; i < sourcesToRemove.Count; ++i)
        {
            PerformRemoval(sourcesToRemove[i]);
        }

        sourcesToRemove.Clear();
    }


    public IAudioSourceWrapper PopFreeSource(bool ignoreSourcesLimit)
    {
        IAudioSourceWrapper freeSource = null;

        bool isMaxSourcesBisy = playingLoopSources.Count + playingSingleSources.Count >= SOURCES_LIMIT;

        if (!isMaxSourcesBisy || ignoreSourcesLimit)
        {
            if (isMaxSourcesBisy)
            {
                // CustomDebug.LogFormat("[SoundSourceContainer<{0}>] Pop unavailable. Not enough sources. Create new source", typeof(TAudioSource).ToString());
            }

            if (freeSources.Count == 0)
            {
                GameObject newSourceGO = new GameObject(string.Format("{0}_AudioSource_{1:D}", typeof(TAudioSource).ToString(), managedSources.Count));
                newSourceGO.transform.parent = sourcesParent;
                newSourceGO.transform.localPosition = Vector3.zero;
                newSourceGO.transform.localRotation = Quaternion.identity;
                newSourceGO.transform.localScale = Vector3.one;

                IAudioSourceWrapper addedAudioSource = null;
                if (typeof(TAudioSource) == typeof(UnityAudioSourceWrapper))
                {
                    AudioSource unitySource = newSourceGO.AddComponent<AudioSource>();
                    addedAudioSource = new UnityAudioSourceWrapper(unitySource);
                }

                managedSources.Add(addedAudioSource);
                freeSources.Add(addedAudioSource);
            }

            freeSource = freeSources[0];
            freeSources.RemoveAt(0);
        }
        else
        {
            // CustomDebug.LogFormat("[SoundSourceContainer<{0}>] Pop unavailable. Not enough sources.", typeof(TAudioSource).ToString());
        }

        return freeSource;
    }


    public void WatchSource(Guid playbackGuid, IAudioSourceWrapper source)
    {
        if (source.loop)
        {
            playingLoopSources.Add(playbackGuid, source);
        }
        else
        {
            playingSingleSources.Add(playbackGuid, source);
        }
    }


    public void SetPlaybackPauseByGuid(Guid guid, bool isPaused)
    {
        IAudioSourceWrapper targetSource = null;
        if (playingSingleSources.TryGetValue(guid, out targetSource))
        {
            if (targetSource.isNonPausable || !isPaused)
            {
                pausedPlaybackGuids.Remove(guid);
                targetSource.UnPause();
            }
            else
            {
                pausedPlaybackGuids.Add(guid);
                targetSource.Pause();
            }
        }
        else if (playingLoopSources.TryGetValue(guid, out targetSource))
        {
            if (targetSource.isNonPausable || !isPaused)
            {
                pausedPlaybackGuids.Remove(guid);
                targetSource.UnPause();
            }
            else
            {
                pausedPlaybackGuids.Add(guid);
                targetSource.Pause();
            }
        }
    }


    public void StopSoundByGuid(Guid guid)
    {
        IAudioSourceWrapper sourceToStop = null;

        playingSingleSources.TryGetValue(guid, out sourceToStop);

        if (sourceToStop == null)
        {
			playingLoopSources.TryGetValue(guid, out sourceToStop);
        }

		if (sourceToStop != null)
		{
			DOTween.Kill(guid, true);
			PerformRemoval(new AudioSourceRemoveEntry
			{
				playbackGuid = guid,
				sourceWrapper = sourceToStop
			});

			sourcesToRemove.RemoveAll(removeEntry => removeEntry.playbackGuid == guid);
		}
    }


    public void RunActionForSoundByGuid(Guid guid, Action<IAudioSourceWrapper, double> action, float duration, Action<Guid> callbackSoundActionEnded)
    {
        IAudioSourceWrapper sourceToAction = null;
        bool isFinded = false;
        isFinded = (playingSingleSources.TryGetValue(guid, out sourceToAction));

        if ((!isFinded || sourceToAction == null) && playingLoopSources.TryGetValue(guid, out sourceToAction))
        {
            isFinded = true;
        }

        if (sourceToAction != null)
        {
            DOTween.To(() => 0, (t) => action?.Invoke(sourceToAction, t), 1.0, duration)
                    .SetUpdate(true)
                    .SetId(guid)
                    .OnComplete(() => callbackSoundActionEnded?.Invoke(guid));
        }
    }


    public void ChangeVolumeSmoothly(Guid guid, float targetVolume, float duration, Action<Guid> callbackSoundVolumeChanged)
    {
        IAudioSourceWrapper sourceToChangeVolume = null;
        bool isFinded = false;
        isFinded = (playingSingleSources.TryGetValue(guid, out sourceToChangeVolume));

        if ((!isFinded || sourceToChangeVolume == null) && playingLoopSources.TryGetValue(guid, out sourceToChangeVolume))
        {
            isFinded = true;
        }

        float startVolume = sourceToChangeVolume.volume;

        if (sourceToChangeVolume != null)
        {
            DOTween.To(() => 0, (x) => sourceToChangeVolume.volume = Mathf.Lerp(startVolume, targetVolume, x), 1f, duration)
                    .SetUpdate(true)
                    .SetId(guid)
                    .OnComplete(() => callbackSoundVolumeChanged?.Invoke(guid));
        }
    }


    public void FadeInSoundByGuid(Guid guid, float targetVolume, float duration, Action<Guid> callbackSoundFinishFadeIn)
    {
        IAudioSourceWrapper sourceToFadeIn = null;
        bool isFinded = false;
        isFinded = (playingSingleSources.TryGetValue(guid, out sourceToFadeIn));

        if ((!isFinded || sourceToFadeIn == null) && playingLoopSources.TryGetValue(guid, out sourceToFadeIn))
        {
            isFinded = true;
        }

        sourceToFadeIn.volume = 0f;

        if (sourceToFadeIn != null)
        {
            DOTween.To(() => 0, (x) => sourceToFadeIn.volume = Mathf.Lerp(0f, targetVolume, x), 1f, duration)
                    .SetUpdate(true)
                    .SetId(guid)
                    .OnComplete(() => callbackSoundFinishFadeIn?.Invoke(guid));
        }
    }


    public void FadeOutSoundByGuid(Guid guid, float duration, Action<Guid> callbackSoundFinishFadeOut)
    {
        IAudioSourceWrapper sourceToStop = null;
        bool isFinded = false;
        isFinded = (playingSingleSources.TryGetValue(guid, out sourceToStop));

        if ((!isFinded || sourceToStop == null) && playingLoopSources.TryGetValue(guid, out sourceToStop))
        {
            isFinded = true;
        }

        float startVolume = sourceToStop.volume;

        if (sourceToStop != null)
        {
            DOTween.To(() => 0, (x) => sourceToStop.volume = Mathf.Lerp(startVolume, 0f, x), 1f, duration)
                    .SetUpdate(true)
                    .SetId(guid)
                    .OnComplete(() => callbackSoundFinishFadeOut?.Invoke(guid));
        }
    }


    public void StopAllSounds()
    {
        foreach (var sourceWrapperPair in playingSingleSources)
        {
            sourcesToRemove.Add(new AudioSourceRemoveEntry
            {
                playbackGuid = sourceWrapperPair.Key,
                sourceWrapper = sourceWrapperPair.Value
            });
        }

        foreach (var sourceWrapperPair in playingLoopSources)
        {
            sourcesToRemove.Add(new AudioSourceRemoveEntry
            {
                playbackGuid = sourceWrapperPair.Key,
                sourceWrapper = sourceWrapperPair.Value
            });
        }

        foreach (var sourceToRemoveEntry in sourcesToRemove)
        {
            PerformRemoval(sourceToRemoveEntry);
        }

        sourcesToRemove.Clear();
    }


    public IAudioSourceWrapper GetSourceByPlaybackGuid(Guid playbackGuid)
    {
        IAudioSourceWrapper source;

        if (!playingSingleSources.TryGetValue(playbackGuid, out source))
        {
            playingLoopSources.TryGetValue(playbackGuid, out source);
        }

        return source;
    }


    public bool IsAnyPlaybackActive()
    {
        return (playingLoopSources.Count > 0 || playingSingleSources.Count > 0);
    }


    public void OnReturnToPool()
    {
        foreach (var singlePair in playingSingleSources)
        {
            sourcesToRemove.Add(new AudioSourceRemoveEntry
            {
                playbackGuid = singlePair.Key,
                sourceWrapper = singlePair.Value
            });
        }

        foreach (var loopPair in playingLoopSources)
        {
            sourcesToRemove.Add(new AudioSourceRemoveEntry
            {
                playbackGuid = loopPair.Key,
                sourceWrapper = loopPair.Value
            });
        }

        for (int i = 0; i < sourcesToRemove.Count; ++i)
        {
            PerformRemoval(sourcesToRemove[i]);
        }

        sourcesToRemove.Clear();

        playingSingleSources.Clear();
        playingLoopSources.Clear();

        freeSources.Clear();
        freeSources.AddRange(managedSources);
    }

    #endregion



    #region Private methods

    void PerformRemoval(AudioSourceRemoveEntry removeEntry)
    {
        playingSingleSources.Remove(removeEntry.playbackGuid);
        playingLoopSources.Remove(removeEntry.playbackGuid);
        pausedPlaybackGuids.Remove(removeEntry.playbackGuid);

        removeEntry.sourceWrapper.isEnabled = false;
        freeSources.Add(removeEntry.sourceWrapper);
		SoundManager.Instance.StopSound(removeEntry.playbackGuid);
    }

    #endregion
}
