using Project.Scripts.Configs.Audio;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Events;

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
        void PlayFailSound();
        void PlayCarHitSound();
        void PlayCarJumpSound();
        void PlayCarLandedSound();
    }

    public class SoundService : ISoundService
    {
        private readonly SoundPlayer _soundPlayer;
        private readonly AudioConfigProvider _audioConfigProvider;
        private readonly GlobalEventProvider _globalEventProvider;

        public bool IsSoundOn => _soundPlayer.IsSoundOn;
        public bool IsMusicOn => _soundPlayer.IsMusicOn;

        public SoundService(SoundPlayer soundPlayer, IConfigsProvider configsProvider,
            GlobalEventProvider globalEventProvider)
        {
            _soundPlayer = soundPlayer;
            _audioConfigProvider = configsProvider.GetConfig<AudioConfigProvider>();

            _globalEventProvider = globalEventProvider;
            _globalEventProvider.SoundSwitchEvent.Invoke(_soundPlayer.IsSoundOn);
            _globalEventProvider.MusicSwitchEvent.Invoke(_soundPlayer.IsMusicOn);
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
            PlayMusic(_audioConfigProvider.GameBackgroundMusic);

        public void PlayButtonClickSound() =>
            PlaySound(_audioConfigProvider.ButtonClick);

        public void PlayOneShot(AudioClipConfig clipConfig) =>
            _soundPlayer.PlaySoundAndCreateAudioSource(clipConfig);

        public void PlayFailSound() =>
            PlaySound(_audioConfigProvider.FailSound);

        public void PlayCarHitSound() =>
            PlaySound(_audioConfigProvider.CarHit);

        public void PlayCarJumpSound() =>
            PlaySound(_audioConfigProvider.CarJump);

        public void PlayCarLandedSound() =>
            PlaySound(_audioConfigProvider.CarLanded);

        private void PlayMusic(AudioClipConfig musicConfig) => _soundPlayer.PlayMusic(musicConfig);
        private void PlaySound(AudioClipConfig soundConfig) => _soundPlayer.PlaySound(soundConfig);
    }
}