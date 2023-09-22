using System;
using UnityEngine.Audio;

interface ISoundSourceContainer
{
    void CustomUpdate(float deltaTime);
    IAudioSourceWrapper PopFreeSource(bool ignoreSourcesLimit);
    void WatchSource(Guid playbackGuid, IAudioSourceWrapper source);
    void SetPlaybackPauseByGuid(Guid guid, bool isPaused);
    void StopSoundByGuid(Guid guid);
    void RunActionForSoundByGuid(Guid guid, Action<IAudioSourceWrapper, double> action, float duration, Action<Guid> callbackSoundActionEnded);
    void ChangeVolumeSmoothly(Guid guid, float targetVolume, float duration, Action<Guid> callbackSoundVolumeChanged);
    void FadeInSoundByGuid(Guid guid, float targetVolume, float duration, Action<Guid> callbackSoundFinishFadeIn);
    void FadeOutSoundByGuid(Guid guid, float duration, Action<Guid> callbackSoundFinishFadeOut);
    void StopAllSounds();
    IAudioSourceWrapper GetSourceByPlaybackGuid(Guid playbackGuid);
    bool IsAnyPlaybackActive();
    void OnReturnToPool();
}
