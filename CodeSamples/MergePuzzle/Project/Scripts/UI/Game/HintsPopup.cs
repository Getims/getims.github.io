using System;
using Project.Scripts.Configs;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Utils = Project.Scripts.Core.Utilities.Utils;

namespace Project.Scripts.UI.Game
{
    public class HintsPopup : PopupPanel
    {
        [SerializeField]
        private Button _buyButton;

        [SerializeField]
        private Button _adsButton;

        [SerializeField]
        private TMP_Text _hintsCount;

        [SerializeField]
        private TMP_Text _hintsCost;

        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private ICurrencyDataService _currencyDataService;

        private PuzzleConfig _puzzleConfig;

        public event Action OnBuyClick;
        public event Action OnAdsClick;

        public override void Show()
        {
            base.Show();
        }

        public override void Initialize()
        {
            base.Initialize();
            HideEvent += DestroySelf;

            _puzzleConfig = _configsProvider.GetConfig<PuzzleConfig>();
            _hintsCost.text = _puzzleConfig.HintsCost.ToString();
            _hintsCount.text = $"X{_puzzleConfig.HintsCountPerBuy}";
        }

        protected override void Start()
        {
            base.Start();
            _buyButton.onClick.AddListener(OnBuyButtonClick);
            _adsButton.onClick.AddListener(OnADSButtonClick);
        }

        protected override void OnDestroy()
        {
            _buyButton.onClick.RemoveListener(OnBuyButtonClick);
            _adsButton.onClick.RemoveListener(OnADSButtonClick);
        }

        private void OnBuyButtonClick()
        {
            int cost = _puzzleConfig.HintsCost;
            long money = _currencyDataService.Money.Value;
            if (money < cost)
                return;

            _currencyDataService.Money.Spend(cost);
            _currencyDataService.HintsCount.Add(_puzzleConfig.HintsCountPerBuy);
            OnBuyClick?.Invoke();

            Hide();
        }

        private void OnADSButtonClick()
        {
            Utils.ReworkPoint("Ads not implemented yet!");
            OnAdsClick?.Invoke();
        }
    }
}