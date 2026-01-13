using Project.Scripts.Core.Sounds;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.Game.Settings
{
    public class SoundButton : MonoBehaviour
    {
        [SerializeField]
        private Button _soundButton;

        [SerializeField]
        private SoundType _type;

        [SerializeField]
        private GameObject _onIcon;

        [SerializeField]
        private GameObject _offIcon;

        [Inject] private ISoundService _soundService;

        public virtual void UpdateInfo()
        {
            bool isSoundOn = _type == SoundType.Sound ? _soundService.IsSoundOn : _soundService.IsMusicOn;
            SetVisual(isSoundOn);
        }

        private void Start()
        {
            _soundButton.onClick.AddListener(OnSoundClick);
            UpdateInfo();
        }

        private void OnDestroy() =>
            _soundButton.onClick.RemoveListener(OnSoundClick);

        private void SetVisual(bool isOn)
        {
            _onIcon.SetActive(isOn);
            _offIcon.SetActive(!isOn);
        }

        private void OnSoundClick()
        {
            bool newState = !(_type == SoundType.Sound ? _soundService.IsSoundOn : _soundService.IsMusicOn);

            if (_type == SoundType.Sound)
                _soundService.SwitchSoundState(newState);
            else
                _soundService.SwitchMusicState(newState);

            SetVisual(newState);
        }

        private enum SoundType
        {
            Sound,
            Music
        }
    }
}