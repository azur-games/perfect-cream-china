using Modules.General.HelperClasses;
using System.Collections.ObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    #region Helper types

    [Serializable]
    public enum MusicCategory
    {
        None = 0,
        UI = 1,
        InGame = 2,
        Music = 3
    }


    [Serializable]
    struct SoundContainerByCategory
    {
        public MusicCategory category;
        public SoundConfigsContainer itemsContainer;

        public List<AssetLink> soundLinks;
        // public SoundLinksConfiguration customLinks;
        public AudioMixerGroup categoryMixerGroup;
    }


    [Serializable]
    struct SoundConfigInfo
    {
        public SoundConfig soundConfig;
        public MusicCategory musicCategory;
    }


    public class PlaySoundData
    {
        public Guid soundGuid;
        public MusicCategory soundCategory;
        public float initialVolume;


        public PlaySoundData()
        {
            soundGuid = Guid.Empty;
            soundCategory = MusicCategory.None;
            initialVolume = 0f;
        }


        public PlaySoundData(Guid soundGuid, MusicCategory musicCategory, float initialVolume)
        {
            this.soundGuid = soundGuid;
            this.soundCategory = musicCategory;
            this.initialVolume = initialVolume;
        }
    }


    class LinksDictionary : Dictionary<string, AssetLink> { }

    #endregion


    #region Fields
    
    public const string MUSIC_KEY = "music_key";
    public const string MUSIC_MUTED_KEY = "music_muted_key";
    public const string MUSIC_PAUSED_KEY = "music_paused_key";
    public const string SOUNDS_KEY = "sounds_key";
    public const string SOUNDS_MUTED_KEY = "sounds_muted_key";
    public const string SOUNDS_PAUSED_KEY = "sounds_paused_key";

    [Header("Source Controllers")]
    [SerializeField] SoundsSourceController musicController;
    [SerializeField] SoundsSourceController soundController;

    [SerializeField] List<SoundContainerByCategory> containers;


    [Header("Default volumes")]
    [SerializeField][Range(0.0f, 1.0f)] float uiVolume;
    [SerializeField][Range(0.0f, 1.0f)] float inGameVolume;
    [SerializeField][Range(0.0f, 1.0f)] float musicVolume;

    Dictionary<MusicCategory, LinksDictionary> sourceDictionaries = new Dictionary<MusicCategory, LinksDictionary>();

    List<Guid> activeSounds = new List<Guid>();
    Dictionary<Guid, SoundsSourceController> soundsControllersByGuid = new Dictionary<Guid, SoundsSourceController>();
    Dictionary<MusicCategory, List<Guid>> activeSoundsGuidsByCategory = new Dictionary<MusicCategory, List<Guid>>();
    Dictionary<string, List<Guid>> activeSoundsGuidsByKey = new Dictionary<string, List<Guid>>();
    Dictionary<Guid, float> activeSoundsInitialVolume = new Dictionary<Guid, float>();

    MusicCategory[] enumCategoriesArray = null;
    Dictionary<string, SoundConfigInfo> soundConfigCache = new Dictionary<string, SoundConfigInfo>();

    Dictionary<MusicCategory, bool> mutedCategoriesDictionary = null;
    Dictionary<MusicCategory, bool> pausedCategoriesDictionary = null;

    #endregion



    #region Properties

    public bool IsMusicEnabled
    {
        get
        {
            return CustomPlayerPrefs.GetBool(MUSIC_KEY, true);
        }

        private set
        {
            CustomPlayerPrefs.SetBool(MUSIC_KEY, value, true);

            if (!value)
            {
                musicController.StopAllSounds();
            }
        }
    }


    public bool IsMusicMuted
    {
        get
        {
            return CustomPlayerPrefs.GetBool(MUSIC_MUTED_KEY, false);
        }

        set
        {
            if (IsMusicMuted != value)
            {
                CustomPlayerPrefs.SetBool(MUSIC_MUTED_KEY, value, true);
                SetSoundsVolumeForCategory(MusicCategory.Music, (value) ? 0f : 1f);
            }
        }
    }


    public bool IsMusicPaused
    {
        get
        {
            return CustomPlayerPrefs.GetBool(MUSIC_PAUSED_KEY, false);
        }
        set
        {
            if (value != IsMusicPaused)
            {
                CustomPlayerPrefs.SetBool(MUSIC_PAUSED_KEY, value, true);
                SetSoundsPauseByCategory(MusicCategory.Music, value);
            }
        }
    }


    public bool IsSoundsEnabled
    {
        get
        {
            return CustomPlayerPrefs.GetBool(SOUNDS_KEY, true);
        }

        private set
        {
            CustomPlayerPrefs.SetBool(SOUNDS_KEY, value, true);

            if (!value)
            {
                soundController.StopAllSounds();
            }
        }
    }


    public bool IsSoundsMuted
    {
        get
        {
            return CustomPlayerPrefs.GetBool(SOUNDS_MUTED_KEY, false);
        }

        set
        {
            if (IsSoundsMuted != value)
            {
                CustomPlayerPrefs.SetBool(SOUNDS_MUTED_KEY, value, true);
                SetSoundsVolumeForCategory(MusicCategory.InGame, (value) ? 0f : 1f);
                SetSoundsVolumeForCategory(MusicCategory.UI, (value) ? 0f : 1f);

                mutedCategoriesDictionary[MusicCategory.InGame] = value;
                mutedCategoriesDictionary[MusicCategory.UI] = value;
            }
        }
    }


    public bool IsSoundsPaused
    {
        get
        {
            return CustomPlayerPrefs.GetBool(SOUNDS_PAUSED_KEY, false);
        }
        set
        {
            if (value != IsSoundsPaused)
            {
                CustomPlayerPrefs.SetBool(SOUNDS_PAUSED_KEY, value, true);
                SetSoundsPauseByCategory(MusicCategory.InGame, value);
            }
        }
    }


    MusicCategory[] EnumCategoriesArray
    {
        get
        {
            if (enumCategoriesArray == null)
            {
                enumCategoriesArray = (MusicCategory[])Enum.GetValues(typeof(MusicCategory));
            }

            return enumCategoriesArray;
        }
    }

    #endregion



    #region Unity lifecycle

    protected override void Awake()
    {
        base.Awake();

        mutedCategoriesDictionary = new Dictionary<MusicCategory, bool>
        {
            { MusicCategory.UI, IsSoundsMuted       },
            { MusicCategory.InGame, IsSoundsMuted   },
            { MusicCategory.Music, IsMusicMuted     }
        };

        pausedCategoriesDictionary = new Dictionary<MusicCategory, bool>
        {
            { MusicCategory.UI, false       },
            { MusicCategory.InGame, false   },
            { MusicCategory.Music, false    }
        };

        FillSoundsDictionary();
    }

    #endregion



    #region Public methods

    public Guid PlayMusic(string key, bool isLooping = true)
    {
        PlaySoundData musicData = new PlaySoundData();

        StopMusic();
        if (IsMusicEnabled)
        {
            musicData = PlaySound(key, musicController, isLooping, false);
            AddActiveGuid(musicData.soundGuid, key, musicData.soundCategory, musicController, musicData.initialVolume);
        }

        return musicData.soundGuid;
    }


    public void MuteMusic(bool isMuted)
    {
        if (IsMusicEnabled)
        {
            SetSoundsVolumeForCategory(MusicCategory.Music, (isMuted) ? 0f : 1f);
        }
    }


    public void MuteSounds(bool isMuted)
    {
        if (IsMusicEnabled)
        {
            SetSoundsVolumeForCategory(MusicCategory.InGame, (isMuted) ? 0f : 1f);
            SetSoundsVolumeForCategory(MusicCategory.UI, (isMuted) ? 0f : 1f);

            mutedCategoriesDictionary[MusicCategory.InGame] = isMuted;
            mutedCategoriesDictionary[MusicCategory.UI] = isMuted;
        }
    }


    public void StopMusic()
    {
        List<Guid> musicGuids = null;

        if (activeSoundsGuidsByCategory.TryGetValue(MusicCategory.Music, out musicGuids))
        {
            for (int i = 0; i < musicGuids.Count; i++)
            {
                StopSound(musicGuids[i]);
            }
        }
    }


    public Guid PlaySound(string key, bool isLooping = false, bool ignoreSourcesLimit = false)
    {
        PlaySoundData soundData = new PlaySoundData();

        if (IsSoundsEnabled && !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
        {
            soundData = PlaySound(key, soundController, isLooping, ignoreSourcesLimit);
            AddActiveGuid(soundData.soundGuid, key, soundData.soundCategory, soundController, soundData.initialVolume);
        }

        return soundData.soundGuid;
    }

    
    public Guid PlayOneShot(string key, float volume)
    {
        Guid guid = Guid.Empty;

        if (IsSoundsEnabled && !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
        {
            PlaySoundData soundData = PlayOneShot(key, soundController, volume);

            AddActiveGuid(soundData.soundGuid, key, soundData.soundCategory, soundController, volume);
            guid = soundData.soundGuid;
        }

        return guid;
    }
    

    public void StopSound(Guid guid)
    {
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(guid, out boundedController))
        {
            boundedController.StopSoundByGuid(guid);

            activeSounds.Remove(guid);
            soundsControllersByGuid.Remove(guid);

            foreach (MusicCategory category in EnumCategoriesArray)
            {
                List<Guid> guidsByCategory = null;
                bool isFindedCategoryGuids = activeSoundsGuidsByCategory.TryGetValue(category, out guidsByCategory);

                if (isFindedCategoryGuids && guidsByCategory != null && guidsByCategory.Contains(guid))
                {
                    guidsByCategory.Remove(guid);
                    activeSoundsInitialVolume.Remove(guid);
                    break;
                }
            }

            List<string> keys = new List<string>(activeSoundsGuidsByKey.Keys);
            foreach (string key in keys)
            {
                List<Guid> guidsByKey = null;
                bool isFindedKeyGuids = activeSoundsGuidsByKey.TryGetValue(key, out guidsByKey);

                if (isFindedKeyGuids && guidsByKey != null && guidsByKey.Contains(guid))
                {
                    guidsByKey.Remove(guid);
                    break;
                }
            }
        }
    }


    public void PlayPauseSound(Guid guid, bool isPause)
    {
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(guid, out boundedController))
        {
            boundedController.SetPlaybackPauseByGuid(guid, isPause);
        }
    }


    public void SetSoundsVolumeForCategory(MusicCategory category, float volume)
    {
        List<Guid> categorySoundsGuid = null;

        if (activeSoundsGuidsByCategory.TryGetValue(category, out categorySoundsGuid))
        {
            categorySoundsGuid.ForEach((guid) =>
            {
                float initialVolume;
                if (!activeSoundsInitialVolume.TryGetValue(guid, out initialVolume))
                {
                    initialVolume = 1f;
                }
                SetSoundVolume(guid, volume * initialVolume);
            });
        }
    }


    public void SetSoundVolume(Guid guid, float volume)
    {
        if (activeSounds.Contains(guid))
        {
            SoundsSourceController boundedSoundController = null;

            if (soundsControllersByGuid.TryGetValue(guid, out boundedSoundController))
            {
                boundedSoundController.SetPlaybackVolume(guid, volume);
            }
        }
    }


    public bool IsPlaybackActive(Guid guid)
    {
        return activeSounds.Contains(guid);
    }


    public void RunActionForSoundByGuid(Guid soundGuid, Action<IAudioSourceWrapper, double> action, float duration, Action<Guid> callbackSoundActionEnded)
    {
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(soundGuid, out boundedController))
        {
            boundedController.RunActionForSoundByGuid(soundGuid, action, duration, callbackSoundActionEnded);
        }
    }


    public void ChangeVolumeSmoothly(Guid soundGuid, float targetVolume, float duration, Action<Guid> callbackSoundVolumeChanged = null)
    {
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(soundGuid, out boundedController))
        {
            boundedController.ChangeVolumeSmoothly(soundGuid, targetVolume, duration, callbackSoundVolumeChanged);
        }
    }


    public void FadeInSoundByGuid(Guid soundGuid, float targetVolume, float duration, Action<Guid> callbackSoundFinishFadeIn = null)
    {
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(soundGuid, out boundedController))
        {
            boundedController.FadeInSoundByGuid(soundGuid, targetVolume, duration, callbackSoundFinishFadeIn);
        }
    }


    public void FadeOutSoundByGuid(Guid soundGuid, float duration, Action<Guid> callbackSoundFinishFadeOut = null)
    {
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(soundGuid, out boundedController))
        {
            boundedController.FadeOutSoundByGuid(soundGuid, duration, callbackSoundFinishFadeOut);
        }
    }
    
    
    public void SetIsMusicEnabled(bool isEnabled)
    {
        IsMusicEnabled = isEnabled;
    }
    
    
    public void SetIsSoundsEnabled(bool isEnabled)
    {
        IsSoundsEnabled = isEnabled;
    }


    public List<Guid> ActiveSoundsGuidsByCategory(MusicCategory category)
    {
        List<Guid> activeSoundsGuids;

        activeSoundsGuidsByCategory.TryGetValue(category, out activeSoundsGuids);

        if (activeSoundsGuids == null)
            activeSoundsGuids = new List<Guid>();

        return activeSoundsGuids;
    }


    public List<Guid> ActiveSoundsGuidsByKey(string key)
    {
        List<Guid> activeSoundsGuids;

        activeSoundsGuidsByKey.TryGetValue(key, out activeSoundsGuids);

        if (activeSoundsGuids == null)
            activeSoundsGuids = new List<Guid>();

        return activeSoundsGuids;
    }


    public SoundConfig SoundConfigByKey(string key)
    {
        MusicCategory category = MusicCategory.None;
        return SoundConfigByKey(key, out category);
    }


    public IAudioSourceWrapper GetSourceByPlaybackGuid(Guid soundGuid)
    {
        IAudioSourceWrapper wrapper = null;
        SoundsSourceController boundedController = null;

        if (soundsControllersByGuid.TryGetValue(soundGuid, out boundedController))
        {
            wrapper = boundedController.GetSourceByPlaybackGuid(soundGuid);
        }

        return wrapper;
    }

    #endregion



    #region Private methods

    AssetLink AssetLinkByKey(string key, out MusicCategory category)
    {
        AssetLink findedLink = null;
        category = MusicCategory.None;

        foreach (MusicCategory iterCategory in sourceDictionaries.Keys)
        {
            LinksDictionary iterDictionary = sourceDictionaries[iterCategory];

            if (iterDictionary.TryGetValue(key, out findedLink))
            {
                category = iterCategory;
                break;
            }
            else
            {
                continue;
            }
        }

        return findedLink;
    }


    AudioMixerGroup AudioMixerGroupForCategory(MusicCategory category)
    {
        return containers.Find((item) => item.category == category).categoryMixerGroup;
    }


    void SetSoundsMutingByCategory(MusicCategory category, bool isMuted)
    {
        List<Guid> guids = null;
        if (activeSoundsGuidsByCategory.TryGetValue(category, out guids))
        {
            guids.ForEach((guid) => PlayPauseSound(guid, isMuted));
        }
        mutedCategoriesDictionary[category] = isMuted;
    }


    void SetSoundsPauseByCategory(MusicCategory category, bool isPaused)
    {
        List<Guid> guids = null;
        if (activeSoundsGuidsByCategory.TryGetValue(category, out guids))
        {
            guids.ForEach((guid) => PlayPauseSound(guid, isPaused));
        }
        pausedCategoriesDictionary[category] = isPaused;
    }


    PlaySoundData PlaySound(string key, SoundsSourceController controller, bool isLooping, bool shouldIgnoreSourcesLimit)
    {
        PlaySoundData result = new PlaySoundData();
        Guid playbackGuid = Guid.Empty;
        MusicCategory category = MusicCategory.None;

        category = MusicCategory.None;
        SoundConfig sound = SoundConfigByKey(key, out category);

        if (sound == null)
        {
            CustomDebug.LogError($"No sound {key}");
        }
        else
        {
            bool isCategoryMuted;
            bool isCategoryPaused;
            mutedCategoriesDictionary.TryGetValue(category, out isCategoryMuted);
            pausedCategoriesDictionary.TryGetValue(category, out isCategoryPaused);

            bool isIgnoredSourcesLimit = shouldIgnoreSourcesLimit || sound.ShouldIgnoreSourcesLimit;

            playbackGuid = controller.PlaySound(sound, isLooping, isCategoryMuted, isIgnoredSourcesLimit, AudioMixerGroupForCategory(category));
            
            if (playbackGuid != Guid.Empty)
            {
                if (isCategoryPaused)
                {
                    PlayPauseSound(playbackGuid, true);
                }
            }

            result = new PlaySoundData(playbackGuid, category, sound.Volume);
        }

        return result;
    }

    
    PlaySoundData PlayOneShot(string key, SoundsSourceController controller, float volume)
    {
        PlaySoundData result = new PlaySoundData();
        Guid playbackGuid = Guid.Empty;
        MusicCategory category = MusicCategory.None;
        float initialVolume = 0f;

        category = MusicCategory.None;
        SoundConfig sound = SoundConfigByKey(key, out category);

        if (sound == null)
        {
            CustomDebug.LogError($"No one shot sound {key}");
        }
        else
        {
            initialVolume = volume;
            bool isCategoryMuted;
            bool isCategoryPaused;
            mutedCategoriesDictionary.TryGetValue(category, out isCategoryMuted);
            pausedCategoriesDictionary.TryGetValue(category, out isCategoryPaused);

            float currentVolume = (isCategoryMuted) ? 0f : volume;
            playbackGuid = controller.PlayOneShot(sound, currentVolume, false, AudioMixerGroupForCategory(category));
            if (playbackGuid != Guid.Empty)
            {
                if (isCategoryPaused)
                {
                    PlayPauseSound(playbackGuid, true);
                }
            }
            result = new PlaySoundData(playbackGuid, category, initialVolume);
        }
        
        return result;
    }
    

    void StopAllSounds()
    {
        musicController.StopAllSounds();
        soundController.StopAllSounds();
    }


    SoundConfig SoundConfigByKey(string key, out MusicCategory category)
    {
        SoundConfigInfo result;

        if(!soundConfigCache.TryGetValue(key, out result))
        {
            AssetLink findedLink = AssetLinkByKey(key, out category);
            result = new SoundConfigInfo();
            if (findedLink != null)
            {
                System.Object asset = findedLink.GetAsset();
                if (!asset.IsNull())
                {
                    SoundConfig soundConfig = ((GameObject)asset).GetComponent<SoundConfig>();
                    result.soundConfig = soundConfig;
                    result.musicCategory = category;
                }
            }
            soundConfigCache.Add(key, result);
        }
        category = result.musicCategory;
        return result.soundConfig;
    }


    void AddActiveGuid(Guid guid, string key, MusicCategory category, SoundsSourceController controller, float initialVolume)
    {
        if (guid != Guid.Empty)
        {
            soundsControllersByGuid.Add(guid, controller);

            List<Guid> guidsForCategory;
            List<Guid> guidsForKey;

            activeSoundsGuidsByCategory.TryGetValue(category, out guidsForCategory);
            activeSoundsGuidsByKey.TryGetValue(key, out guidsForKey);

            if (guidsForCategory == null)
            {
                guidsForCategory = new List<Guid>();
                activeSoundsGuidsByCategory.Add(category, guidsForCategory);
            }

            if (guidsForKey == null)
            {
                guidsForKey = new List<Guid>();
                activeSoundsGuidsByKey.Add(key, guidsForKey);
            }

            guidsForCategory.Add(guid);
            guidsForKey.Add(guid);
            activeSounds.Add(guid);
            activeSoundsInitialVolume.Add(guid, initialVolume);
        }
    }


    void FillSoundsDictionary()
    {
        for (int i = 0; i < containers.Count; i++)
        {
            SoundContainerByCategory currentContainer = containers[i];

            LinksDictionary srcDictionary = new LinksDictionary();

            currentContainer.soundLinks.ForEach((link) =>
            {
                if (!link.IsNull() && !string.IsNullOrEmpty(link.Name) && !srcDictionary.ContainsKey(link.Name))
                {
                    srcDictionary.Add(link.Name, link);
                }
            });

            sourceDictionaries.Add(currentContainer.category, srcDictionary);
        }
    }

    #endregion
}