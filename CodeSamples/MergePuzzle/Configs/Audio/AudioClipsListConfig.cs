using System;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Audio
{
    [Serializable]
    [ConfigCategory(ConfigCategory.Audio)]
    public class AudioClipsListConfig : ScriptableConfig
    {
        [Title("Background")]
        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_gameBackgroundMusic) + ")")]
        private AudioClipConfig _gameBackgroundMusic;

        [Title("UI")]
        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_addCoin) + ")")]
        private AudioClipConfig _addCoin;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_buttonClick) + ")")]
        private AudioClipConfig _buttonClick;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_win) + ")")]
        private AudioClipConfig _win;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_fail) + ")")]
        private AudioClipConfig _fail;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_closePopup) + ")")]
        private AudioClipConfig _closePopup;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_selectLevel) + ")")]
        private AudioClipConfig _selectLevel;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_elementMerge) + ")")]
        private AudioClipConfig _elementMerge;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_elementDrop) + ")")]
        private AudioClipConfig _elementDrop;

        [Title("Combo")]
        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_comboBackground) + ")")]
        private AudioClipConfig _comboBackground;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_amazing) + ")")]
        private AudioClipConfig _amazing;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_good) + ")")]
        private AudioClipConfig _good;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_great) + ")")]
        private AudioClipConfig _great;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_perfect) + ")")]
        private AudioClipConfig _perfect;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_unbelievable) + ")")]
        private AudioClipConfig _unbelievable;

        [SerializeField, SuffixLabel("@GetLabel(" + nameof(_wow) + ")")]
        private AudioClipConfig _wow;

        public AudioClipConfig GameBackgroundMusic => _gameBackgroundMusic;

        public AudioClipConfig ButtonClick => _buttonClick;
        public AudioClipConfig AddCoin => _addCoin;
        public AudioClipConfig WinSound => _win;
        public AudioClipConfig FailSound => _fail;
        public AudioClipConfig ClosePopup => _closePopup;
        public AudioClipConfig SelectLevel => _selectLevel;
        public AudioClipConfig Amazing => _amazing;
        public AudioClipConfig Good => _good;
        public AudioClipConfig Great => _great;
        public AudioClipConfig Perfect => _perfect;
        public AudioClipConfig Unbelievable => _unbelievable;
        public AudioClipConfig Wow => _wow;
        public AudioClipConfig ComboBackground => _comboBackground;
        public AudioClipConfig ElementMerge => _elementMerge;
        public AudioClipConfig ElementDrop => _elementDrop;

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