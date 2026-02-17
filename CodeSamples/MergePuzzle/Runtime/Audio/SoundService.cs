using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Audio;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Enums;
using Project.Scripts.Runtime.Events;
using UnityEngine;

namespace Project.Scripts.Runtime.Audio
{
    public interface ISoundService
    {
        bool IsSoundOn { get; }
        bool IsMusicOn { get; }

        void SwitchSoundState(bool isOn);
        void SwitchMusicState(bool isOn);
        void PlayGameBackgroundMusic();
        void PlayButtonClickSound();
        void PlayOneShot(AudioClipConfig clipConfig);
        void PlayWinSound();
        void PlayComboSound(ComboType comboType);
        void PlayCoinSound();
        void PlayMergeSound();
        void PlayDropSound();
    }

    public class SoundService : ISoundService
    {
        private readonly SoundPlayer _soundPlayer;
        private readonly AudioClipsListConfig _audioClipsListConfig;
        private readonly GlobalEventProvider _globalEventProvider;
        private Dictionary<ComboType, AudioClipConfig> _comboSounds;

        public bool IsSoundOn => _soundPlayer.IsSoundOn;
        public bool IsMusicOn => _soundPlayer.IsMusicOn;

        public SoundService(SoundPlayer soundPlayer, IConfigsProvider configsProvider,
            GlobalEventProvider globalEventProvider)
        {
            _soundPlayer = soundPlayer;
            _audioClipsListConfig = configsProvider.GetConfig<GlobalConfig>().AudioClipsListConfig;

            _globalEventProvider = globalEventProvider;
            _globalEventProvider.SoundSwitchEvent.Invoke(_soundPlayer.IsSoundOn);
            _globalEventProvider.MusicSwitchEvent.Invoke(_soundPlayer.IsMusicOn);

            _comboSounds = new Dictionary<ComboType, AudioClipConfig>
            {
                { ComboType.Amazing, _audioClipsListConfig.Amazing },
                { ComboType.Good, _audioClipsListConfig.Good },
                { ComboType.Great, _audioClipsListConfig.Great },
                { ComboType.Perfect, _audioClipsListConfig.Perfect },
                { ComboType.Unbelievable, _audioClipsListConfig.Unbelievable },
                { ComboType.Wow, _audioClipsListConfig.Wow }
            };
        }

        public void SwitchSoundState(bool isOn)
        {
            _soundPlayer.SwitchSoundState();
            _globalEventProvider.SoundSwitchEvent.Invoke(isOn);
        }

        public void SwitchMusicState(bool isOn)
        {
            _soundPlayer.SwitchMusicState();
            _globalEventProvider.MusicSwitchEvent.Invoke(isOn);
        }

        public void PlayGameBackgroundMusic() =>
            PlayMusic(_audioClipsListConfig.GameBackgroundMusic);

        public void PlayButtonClickSound() =>
            PlaySound(_audioClipsListConfig.ButtonClick);

        public void PlayOneShot(AudioClipConfig clipConfig) =>
            _soundPlayer.PlaySoundAndCreateAudioSource(clipConfig);

        public void PlayWinSound() =>
            PlaySound(_audioClipsListConfig.WinSound);

        public void PlayComboSound(ComboType comboType)
        {
            PlaySound(_audioClipsListConfig.ComboBackground);
            if (_comboSounds.TryGetValue(comboType, out var clip))
                PlaySound(clip);
            else
                Debug.LogWarning($"ComboType {comboType} not found in _comboSounds dictionary.");
        }

        public void PlayCoinSound() =>
            PlaySound(_audioClipsListConfig.AddCoin);

        public void PlayMergeSound() =>
            PlaySound(_audioClipsListConfig.ElementMerge);

        public void PlayDropSound() =>
            PlaySound(_audioClipsListConfig.ElementDrop);

        private void PlayMusic(AudioClipConfig musicConfig) => _soundPlayer.PlayMusic(musicConfig);
        private void PlaySound(AudioClipConfig soundConfig) => _soundPlayer.PlaySound(soundConfig);
    }
}