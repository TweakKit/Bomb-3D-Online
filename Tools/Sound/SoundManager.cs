using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZB.Client.Management;
using ZB.Model;
using ZB.UI;

namespace ZB.Gameplay
{
    public class SoundManager : Singleton<SoundManager>
    {
        #region Members

        private static readonly int initialAudioSources = 10;
        private static readonly int extraAudioSources = 5;

        [Header("SFX clips")]
        public AudioClip SFX_Ingame_pickup_items;
        public AudioClip SFX_ingame_losegame;
        public AudioClip SFX_ingame_wingame;
        public AudioClip SFX_Ingame_level_up;
        public AudioClip SFX_ingame_hero_put_bom;
        public AudioClip SFX_ingame_hero_hit;
        public AudioClip SFX_ingame_hero_die;
        public AudioClip SFX_ingame_chest_broken;
        public AudioClip SFX_ingame_brick_explosion;
        public AudioClip SFX_ingame_bomb_explosion;

        [Header("UI clips")]
        public AudioClip SFX_button_click;

        [Space(10)]

        [Header("Gameplay background music sound", order = 1)]
        public AudioClip[] backgroundMusicClips;

        private bool _soundBGMOn;
        private bool _soundSFXOn;

        private AudioSource _backgroundMusic;
        private List<AudioSource> _availableAudioSources = new List<AudioSource>();
        private List<AudioSource> _playingAudioSources = new List<AudioSource>();

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnNewSceneLoaded;
            CreateAudioSourcePool(initialAudioSources);
        }

        private void Start()
        {
            InitBGMSource();
            PlayerProfile profile = PlayerManagement.Instance.profile;
            if (profile != null)
            {
                _soundBGMOn = profile.settingMusic;
                _soundSFXOn = profile.settingSfx;
            }
            else
            {
                _soundBGMOn = true;
                _soundSFXOn = true;
            }

            if (_soundBGMOn)
                UnmuteSoundBGM();
            else
                MuteSoundBGM();

            if (_soundSFXOn)
                UnmuteSoundSFX();
            else
                MuteSoundSFX();
        }

        #endregion API Methods

        #region Class Methods

        public void SetSfx(bool soundSFXOn)
        {
            if (soundSFXOn)
                UnmuteSoundSFX();
            else
                MuteSoundSFX();

            _soundSFXOn = soundSFXOn;
        }

        public void SetBgm(bool soundBgm)
        {
            if (soundBgm)
                UnmuteSoundBGM();
            else
                MuteSoundBGM();

            _soundBGMOn = soundBgm;
        }

        public void PlaySFXSeting()
        {
            UnmuteSoundSFX();
            PlaySound(SFX_Ingame_pickup_items, 1, true);
        }

        public void ResetSeting()
        {
            Start();
            SettingManagerUI.UpdateSetingViaProfile();
            if (SwitchManagerUI.Instance.ContainsKey("SFX"))
            {
                SwitchManagerUI.Instance["SFX"].SetOnOff();
            }
            if (SwitchManagerUI.Instance.ContainsKey("MUSIC"))
            {
                SwitchManagerUI.Instance["MUSIC"].SetOnOff();
            }
        }

        public void PauseSound()
        {
            if (_playingAudioSources != null)
                foreach (AudioSource source in _playingAudioSources)
                    if (source != null)
                        source.Stop();

            if (_backgroundMusic)
                _backgroundMusic.mute = true;
        }

        public void UnpauseSound()
        {
            if (_backgroundMusic)
                _backgroundMusic.mute = false;
        }

        public void PlayBackgroundMusic(AudioClip backgroundMusicClip, float volume, float fadeDuration)
        {
            if (_backgroundMusic.isPlaying && _backgroundMusic.clip == backgroundMusicClip)
                return;

            _backgroundMusic.Stop();
            _backgroundMusic.clip = backgroundMusicClip;
            _backgroundMusic.volume = volume;
            _backgroundMusic.loop = true;
            _backgroundMusic.gameObject.SetActive(true);

            if (backgroundMusicClip != null)
                StartCoroutine(FadeInBackgroundMusic(fadeDuration));
        }

        public void PlayNewBackgroundMusic(AudioClip newBackgroundMusicClip, float fadeDuration, float volume = 1, Action callback = null)
        {
            if (_backgroundMusic.isPlaying && _backgroundMusic.clip == newBackgroundMusicClip)
                return;

            if (_backgroundMusic.isPlaying)
                StartCoroutine(FadeOutBackgroundMusic(newBackgroundMusicClip, volume, fadeDuration, callback));
            else
                PlayBackgroundMusic(newBackgroundMusicClip, volume, fadeDuration);
        }

        public void StopBackgroundMusic(float fadeDuration, Action callback = null)
        {
            if (_backgroundMusic == null)
                return;

            if (fadeDuration == 0)
            {
                _backgroundMusic.Stop();
                _backgroundMusic.clip = null;
            }
            else StartCoroutine(FadeOutBackgroundMusic(null, 0, fadeDuration, callback));
        }

        public AudioSource PlaySound(AudioClip sfxClip, float volume = 1, bool forcePlay = false)
        {
            if (!forcePlay && !_soundSFXOn)
                return null;

            if (!forcePlay && sfxClip == null)
                return null;

            AudioSource audioSource = GetAudioSource();
            audioSource.clip = sfxClip;
            audioSource.volume = volume;
            audioSource.gameObject.SetActive(true);
            audioSource.Play();

            _playingAudioSources.Add(audioSource);
            return audioSource;
        }

        public AudioSource PlaySound(AudioClip[] sfxClips, int clipIndex, float volume = 1)
        {
            if (!_soundSFXOn)
                return null;

            if (sfxClips == null)
                return null;

            AudioSource audioSource = GetAudioSource();
            audioSource.clip = sfxClips[Mathf.Clamp(clipIndex, 0, sfxClips.Length - 1)];
            audioSource.volume = volume;
            audioSource.gameObject.SetActive(true);
            audioSource.Play();

            _playingAudioSources.Add(audioSource);
            return audioSource;
        }

        public AudioSource PlaySound(AudioClip sfxClip, Transform parentTransform, float volume = 1, bool setParent = true)
        {
            AudioSource audioSource = PlaySound(sfxClip, volume);
            if (!audioSource)
                return null;

            if (setParent)
            {
                audioSource.transform.SetParent(parentTransform);
                audioSource.transform.localPosition = Vector3.zero;
            }
            else audioSource.transform.position = parentTransform.position;

            return audioSource;
        }

        public AudioSource PlaySound(AudioClip sfxClip, Vector3 position, float volume = 1)
        {
            AudioSource audioSource = PlaySound(sfxClip, volume);
            if (!audioSource)
                return null;

            audioSource.transform.position = position;
            return audioSource;
        }

        public void ReturnAudioSourceToPool(AudioSource audioSource)
        {
            if (_availableAudioSources.Contains(audioSource))
                return;

            _playingAudioSources.Remove(audioSource);
            _availableAudioSources.Add(audioSource);
            audioSource.transform.SetParent(transform);
            audioSource.gameObject.SetActive(false);
        }

        private IEnumerator FadeInBackgroundMusic(float fadeDuration)
        {
            _backgroundMusic.Play();

            float time = 0;
            float originalVolume = _backgroundMusic.volume;

            while (time <= fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                _backgroundMusic.volume = Mathf.Clamp01(originalVolume * (time / fadeDuration));
                yield return null;
            }
            _backgroundMusic.volume = originalVolume;
        }

        private IEnumerator FadeOutBackgroundMusic(AudioClip newBackgroundMusicClip, float volume, float fadeDuration, Action callback)
        {
            float time = fadeDuration;
            float originalVolume = _backgroundMusic.volume;

            while (time >= 0)
            {
                time -= Time.unscaledDeltaTime;
                _backgroundMusic.volume = Mathf.Clamp01(originalVolume * (time / fadeDuration));
                yield return null;
            }

            _backgroundMusic.Stop();
            _backgroundMusic.clip = null;

            if (newBackgroundMusicClip != null)
                PlayBackgroundMusic(newBackgroundMusicClip, volume, fadeDuration);

            if (callback != null)
                callback.Invoke();
        }

        private void OnNewSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            for (int i = _playingAudioSources.Count - 1; i >= 0; i--)
                StopSound(_playingAudioSources[i]);
        }

        private void StopSound(AudioSource source)
        {
            if (source == null || source.isPlaying == false)
                return;

            source.volume = 0;
        }

        public void MuteSoundBGM()
        {
            if (_backgroundMusic)
                _backgroundMusic.mute = true;
        }

        private void MuteSoundSFX()
        {
            if (_playingAudioSources != null)
                foreach (AudioSource source in _playingAudioSources)
                    source.mute = true;
        }

        public void UnmuteSoundBGM()
        {
            if (_backgroundMusic)
                _backgroundMusic.mute = false;
        }

        private void UnmuteSoundSFX()
        {
            if (_playingAudioSources != null)
                foreach (AudioSource source in _playingAudioSources)
                    source.mute = false;
        }

        private void CreateAudioSourcePool(int initialCount)
        {
            for (int index = 0; index < initialCount; index++)
            {
                GameObject audioSourceObject = new GameObject("AudioSource");
                audioSourceObject.SetActive(false);
                audioSourceObject.AddComponent<ReturnAudioSourceToPool>();
                audioSourceObject.transform.SetParent(transform);

                AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
                _availableAudioSources.Add(audioSource);
            }
        }

        private AudioSource InitBGMSource()
        {
            if (_backgroundMusic == null)
            {
                var bgm = new GameObject("BGMAudioSource");
                bgm.transform.SetParent(transform);
                _backgroundMusic = bgm.AddComponent<AudioSource>();
            }

            return _backgroundMusic;
        }

        private AudioSource GetAudioSource()
        {
            if (_availableAudioSources.Count <= 0)
                CreateAudioSourcePool(extraAudioSources);

            AudioSource audioSource = _availableAudioSources[0];
            _availableAudioSources.RemoveAt(0);
            audioSource.playOnAwake = false;

            return audioSource;
        }

        #endregion Class Methods
    }
}