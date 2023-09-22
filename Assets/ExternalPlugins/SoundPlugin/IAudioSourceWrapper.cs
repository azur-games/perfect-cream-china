using UnityEngine;
using UnityEngine.Audio;

public interface IAudioSourceWrapper 
{
    AudioClip clip { get; set; }
    bool loop { get; set; }
    bool isPlaying { get; }
    bool isEnabled { get; set; }
    float volume { get; set; }
    float pitch { get; set; }
    bool isNonPausable { get; set; }
    AudioMixerGroup audioMixerGroup { get; set; }
    
   
    void Play();
    void PlayOneShot(float volumeScale);
    void Pause();
    void UnPause();
    void Stop();
    int GetInstanceID();
}


public class UnityAudioSourceWrapper : IAudioSourceWrapper
{
    #region Fields

    AudioSource boundSource;

    #endregion


    #region Constructors

    public UnityAudioSourceWrapper(AudioSource boundSource)
    {
        this.boundSource = boundSource;
    }

    #endregion


    #region IAudioSource

    public AudioClip clip { get { return boundSource.clip; } set { boundSource.clip = value; } }
    public bool loop { get { return boundSource.loop; } set { boundSource.loop = value; } }
    public bool isPlaying { get { return boundSource.isPlaying; } }
    public bool isEnabled { get { return boundSource.enabled; } set { boundSource.enabled = value; } }
    public float volume { get { return boundSource.volume; } set { boundSource.volume = value; } }
    public float pitch { get { return boundSource.pitch; } set { boundSource.pitch = value; } }
    public AudioMixerGroup audioMixerGroup { get { return boundSource.outputAudioMixerGroup; } set { boundSource.outputAudioMixerGroup = value; } }
    public bool isNonPausable { get; set; }
 

	public void Play()
    {
        boundSource.Play();
    }


    public void Pause()
    {
        boundSource.Pause();
    }


    public void UnPause()
    {
        boundSource.UnPause();
    }
    
    
    public void PlayOneShot(float volumeScale)
    {
        boundSource.PlayOneShot(clip, volumeScale);
    }


    public void Stop()
    {
        boundSource.Stop();
    }


    public int GetInstanceID()
    {
        return boundSource.GetInstanceID();
    }

    #endregion
}