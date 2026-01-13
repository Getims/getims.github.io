using System;
using Project.Scripts.Core.Events;
using Project.Scripts.Data;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.Game.Top
{
    public class TopGamePanel : UIPanel
    {
        [SerializeField]
        private Button _settingsButton;

        [SerializeField]
        private Button _hintButton;

        [Inject] private ICurrencyDataService _currencyDataService;
        [Inject] private GameplayEventProvider _gameplayEventProvider;

        public event Action OnSettingsClick;
        public event Action OnNoHintsClick;

        protected void Start()
        {
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _hintButton.onClick.AddListener(OnHintButtonClick);
        }

        protected override void OnDestroy()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            _hintButton.onClick.RemoveListener(OnHintButtonClick);
        }

        private void OnSettingsButtonClick() => OnSettingsClick?.Invoke();

        private void OnHintButtonClick()
        {
            if (_currencyDataService.HintsCount.Value > 0)
            {
                _gameplayEventProvider.HintActivateRequest.Invoke();
                return;
            }

            OnNoHintsClick?.Invoke();
        }
    }
}