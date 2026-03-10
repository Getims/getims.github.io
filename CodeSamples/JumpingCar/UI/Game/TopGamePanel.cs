using System;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Runtime.Events;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.Game
{
    public class TopGamePanel : UIPanel
    {
        [SerializeField]
        private Button _restartButton;

        [SerializeField]
        private TMP_Text _scoreValue;

        [Inject] private GameEventsProvider _gameEventsProvider;

        public event Action OnRestartClick;

        protected void Start()
        {
            _restartButton.onClick.AddListener(OnRestartButtonClick);
            _gameEventsProvider.PlayerLandedNewPlatformEvent.AddListener(OnPlayerLandedNewPlatform);
        }

        protected override void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonClick);
            _gameEventsProvider.PlayerLandedNewPlatformEvent.RemoveListener(OnPlayerLandedNewPlatform);
        }

        private void OnPlayerLandedNewPlatform(IPlatform platform)
        {
            if (platform.Number <= 1)
                return;

            _scoreValue.text = platform.Number.ToString();
        }

        private void OnRestartButtonClick() => OnRestartClick?.Invoke();
    }
}