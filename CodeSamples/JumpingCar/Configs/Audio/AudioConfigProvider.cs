using System;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Audio
{
    [Serializable]
    [ConfigCategory(ConfigCategory.Audio)]
    public class AudioConfigProvider : ScriptableConfig
    {
        [SerializeField]
        private VolumeConfig _volumeConfig;

        [Title("Background")]
        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_gameBackgroundMusic) + ")")]
        private AudioClipConfig _gameBackgroundMusic;

        [Title("UI")]
        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_buttonClick) + ")")]
        private AudioClipConfig _buttonClick;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_fail) + ")")]
        private AudioClipConfig _fail;

        [Title("Gameplay")]
        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_carHit) + ")")]
        private AudioClipConfig _carHit;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_carJump) + ")")]
        private AudioClipConfig _carJump;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_carLanded) + ")")]
        private AudioClipConfig _carLanded;

        public AudioClipConfig GameBackgroundMusic => _gameBackgroundMusic;
        public AudioClipConfig ButtonClick => _buttonClick;
        public AudioClipConfig FailSound => _fail;
        public AudioClipConfig CarHit => _carHit;
        public AudioClipConfig CarJump => _carJump;
        public AudioClipConfig CarLanded => _carLanded;
        public VolumeConfig VolumeConfig => _volumeConfig;


#if UNITY_EDITOR
        private string GetLabel(AudioClipConfig clipConfig)
        {
            if (clipConfig.IsDisabled)
                return "- Disabled";
            if (clipConfig.AudioClip == null)
                return "- No Audio";
            return clipConfig.AudioClip.name;
        }
#endif
    }
}