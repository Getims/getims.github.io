using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Core.Sounds.Configs;
using Project.Scripts.Core.Sounds.Data;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Core.Sounds
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField]
        private List<AudioSource> _gameplayAS;

        [SerializeField]
        private AudioSource _musicAS;

        private Dictionary<string, SfxHistory> _sfxHistory = new Dictionary<string, SfxHistory>();
        private ISoundDataService _soundDataService;
        private VolumeConfig _volumeConfig;
        private Coroutine _musicCoroutine;

        private bool _isSoundOn = true;
        private bool _isMusicOn = true;
        private float _lastBgPercent = 0;
        private int _currentGameplayAS = 0;

        public bool IsSoundOn => _isSoundOn;
        public bool IsMusicOn => _isMusicOn;

        [Inject]
        public void Construct(ISoundDataService soundDataService, IConfigsProvider configsProvider)
        {
            _soundDataService = soundDataService;
            _volumeConfig = configsProvider.GetConfig<GlobalConfig>().VolumeConfig;
        }

        public void SwitchSoundState()
        {
            _isSoundOn = !_isSoundOn;
            if (!_isSoundOn)
            {
                foreach (var audioSource in _gameplayAS)
                    audioSource.Stop();
            }

            _soundDataService.IsSoundOn.Set(_isSoundOn);
        }

        public void SwitchMusicState()
        {
            _isMusicOn = !_isMusicOn;

            if (_isMusicOn && _musicAS.enabled)
                _musicAS.Play();
            else
                _musicAS.Stop();

            _soundDataService.IsMusicOn.Set(_isMusicOn);
        }

        public void PlaySound(AudioClipConfig audioClipConfig, bool ignoreTime = true, float pauseTime = 1.0f)
        {
            if (!_isSoundOn)
                return;

            if (audioClipConfig.IsDisabled)
                return;

            bool canPlay = true;

            if (!ignoreTime)
            {
                var clipName = audioClipConfig.FileName;
                SfxHistory history = null;

                if (_sfxHistory.ContainsKey(clipName))
                {
                    history = _sfxHistory[clipName];
                    if (Time.time - pauseTime < history.Time)
                        return;

                    if (history.Time < Time.time - pauseTime)
                    {
                        _sfxHistory.Remove(clipName);
                        history = null;
                    }
                }

                if (history == null)
                    _sfxHistory.Add(clipName, new SfxHistory(Time.time));
            }

            if (!canPlay)
                return;

            AudioSource gameplayAS = GetGameplayAS();
            gameplayAS.pitch = Random.Range(_volumeConfig.SoundMinPitch, _volumeConfig.SoundMaxPitch);
            gameplayAS.PlayOneShot(audioClipConfig.AudioClip, GetCustomGameplaySoundVolume(audioClipConfig));
        }

        private AudioSource GetGameplayAS()
        {
            var result = _gameplayAS[_currentGameplayAS];

            _currentGameplayAS++;
            if (_currentGameplayAS >= _gameplayAS.Count)
                _currentGameplayAS = 0;

            return result;
        }

        public void PlaySoundAndCreateAudioSource(AudioClipConfig audioClipConfig)
        {
            if (!_isSoundOn)
                return;

            if (audioClipConfig.IsDisabled)
                return;

            if (audioClipConfig.AudioClip == null)
                return;

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.pitch = Random.Range(0.95f, 1f);
            audioSource.priority = 100;
            audioSource.volume = audioClipConfig.SoundPercent;
            audioSource.PlayOneShot(audioClipConfig.AudioClip, GetCustomGameplaySoundVolume(audioClipConfig));

            Destroy(audioSource, audioClipConfig.AudioClip.length);
        }

        public AudioSource PlayLoopedSound(AudioClipConfig audioClipConfig)
        {
            if (!_isSoundOn)
                return null;

            if (audioClipConfig.IsDisabled)
                return null;

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.pitch = Random.Range(0.95f, 1.01f);
            audioSource.priority = 100;
            audioSource.loop = true;
            audioSource.clip = audioClipConfig.AudioClip;
            audioSource.volume = GetCustomGameplaySoundVolume(audioClipConfig);
            audioSource.Play();

            return audioSource;
        }

        public void DestroyAudioSource(AudioSource audioSource)
        {
            if (audioSource != null)
                Destroy(audioSource);
        }

        public void PlayMusic(AudioClipConfig AudioClipConfig, bool highPassEffect = false, int frequency = 1000)
        {
            if (AudioClipConfig == null)
            {
                Debug.LogWarning("No music config");
                return;
            }

            if (AudioClipConfig.IsDisabled)
                return;

            _musicAS.pitch = 1;
            EnableBackgroundHighPass(highPassEffect, frequency);

            if (_musicCoroutine != null)
                StopCoroutine(_musicCoroutine);

            _musicCoroutine =
                StartCoroutine(BackGroundMusicFading(AudioClipConfig.AudioClip, AudioClipConfig.SoundPercent));
        }

        private void Start()
        {
            _isSoundOn = _soundDataService.IsSoundOn.Value;
            _isMusicOn = _soundDataService.IsMusicOn.Value;
        }

        private float GetCustomGameplaySoundVolume(AudioClipConfig config) =>
            config == null
                ? _volumeConfig.SoundsVolume
                : _volumeConfig.SoundsVolume * config.SoundPercent;

        private void EnableBackgroundHighPass(bool state = false, int frequency = 1000)
        {
            AudioHighPassFilter audioHighPassFilter = _musicAS.GetComponent<AudioHighPassFilter>();
            audioHighPassFilter.cutoffFrequency = frequency;
            audioHighPassFilter.enabled = state;
        }

        private IEnumerator BackGroundMusicFading(AudioClip clip, float newPercent)
        {
            _musicAS.volume = _volumeConfig.MusicVolume * _lastBgPercent;
            float elapsedTime = 0;

            while (elapsedTime <= _volumeConfig.MusicFadeTime)
            {
                _musicAS.volume = Mathf.Lerp(_volumeConfig.MusicVolume * _lastBgPercent, 0,
                    elapsedTime / _volumeConfig.MusicFadeTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            elapsedTime = 0;
            _musicAS.volume = 0;
            _musicAS.Stop();
            _musicAS.clip = clip;

            if (_isMusicOn && _musicAS.enabled)
                _musicAS.Play();

            _lastBgPercent = newPercent;
            while (elapsedTime <= _volumeConfig.MusicFadeTime)
            {
                _musicAS.volume = Mathf.Lerp(0, _volumeConfig.MusicVolume * _lastBgPercent,
                    elapsedTime / _volumeConfig.MusicFadeTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            _musicAS.volume = _volumeConfig.MusicVolume * _lastBgPercent;
        }

        private class SfxHistory
        {
            public float Time;

            public SfxHistory(float time)
            {
                Time = time;
            }
        }
    }
}