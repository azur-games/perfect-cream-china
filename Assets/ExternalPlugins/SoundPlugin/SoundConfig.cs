using UnityEngine;


public class SoundConfig : MonoBehaviour
{
    #region Fields

    [Header("Refs")]
    [SerializeField] AudioClip audioClip;

    [Header("Volume")]
    [SerializeField] [Range(0.0f, 1.0f)] float volume = 1.0f;

    [Header("Pitch")]
    [SerializeField] [Range(-3.0f, 3.0f)] float pitch = 1.0f;

    [Header("Settings")]
    [SerializeField] bool shouldIgnoreSourcesLimit;

    #endregion


    #region Properties

    public AudioClip Clip { get { return audioClip; } }
    public float Volume { get { return volume; } }
    public bool ShouldIgnoreSourcesLimit { get { return shouldIgnoreSourcesLimit; } }

    #endregion


    #region Public methods

    public void Init(AudioClip audioClip, float volume = 1.0f)
    {
        this.audioClip = audioClip;
        this.volume = volume;
    }

    
    public void ApplySelfToSource(IAudioSourceWrapper source)
    {
        source.clip = audioClip;
		source.volume = Mathf.Clamp01(volume);
        source.pitch = Mathf.Clamp(pitch, -3.0f, 3.0f);
        source.isNonPausable = false;//isNonPausable;
    }

    #endregion

}
