using System;
using System.Linq;
using Project.Scripts.Data;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.Game
{
    public class MainMenuPanel : UIPanel
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private TMP_Text _infoText;

        [Inject] private IGameDataService _gameDataService;

        public event Action OnStartClick;

        public override void Show()
        {
            base.Show();
            UpdateWinText();
        }

        protected void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
        }

        protected override void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void OnStartButtonClick() => OnStartClick?.Invoke();

        private void UpdateWinText()
        {
            if (_gameDataService.GameScores.Count == 0)
            {
                _infoText.text = GameConstants.TUTORIAL;
                return;
            }

            int last = _gameDataService.GameScores.Last();
            int max = _gameDataService.GameScores.Max();

            if (max <= 1)
                _infoText.text = GameConstants.TUTORIAL;
            else
                _infoText.text = $"{GameConstants.LAST_RECORD_TITLE}: {last}\n " +
                                 $"{GameConstants.RECORD_TITLE}: {max}";
        }
    }
}