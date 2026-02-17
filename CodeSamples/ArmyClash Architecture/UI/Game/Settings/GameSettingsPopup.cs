using System;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Game.Settings
{
    public class GameSettingsPopup : PopupPanel
    {
        [SerializeField]
        private Button _restartButton;

        [SerializeField]
        private Button _exitButton;

        public event Action OnRestartClick;
        public event Action OnExitClick;

        protected override void Start()
        {
            base.Start();
            _restartButton.onClick.AddListener(OnRestartButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        protected override void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonClick);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void OnRestartButtonClick() => OnRestartClick?.Invoke();
        private void OnExitButtonClick() => OnExitClick?.Invoke();
    }
}