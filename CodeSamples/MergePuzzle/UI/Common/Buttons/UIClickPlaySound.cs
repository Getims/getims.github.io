using Project.Scripts.Runtime.Audio;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Common.Buttons
{
    public class UIClickPlaySound : MonoBehaviour
    {
        [Inject] private ISoundService _soundService;

        public void OnClick() =>
            _soundService?.PlayButtonClickSound();
    }
}