using Project.Scripts.Runtime.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.Game
{
    public class SoundButton : MonoBehaviour
    {
        [SerializeField]
        private Button _soundButton;

        [SerializeField]
        private GameObject _onIcon;

        [SerializeField]
        private GameObject _offIcon;

        [Inject] private ISoundService _soundService;

        public virtual void UpdateInfo()
        {
            bool isSoundOn = _soundService.IsSoundOn;
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
            bool newState = !(_soundService.IsSoundOn);

            _soundService.SwitchSoundState(newState);
            _soundService.SwitchMusicState(newState);

            SetVisual(newState);
        }
    }
}