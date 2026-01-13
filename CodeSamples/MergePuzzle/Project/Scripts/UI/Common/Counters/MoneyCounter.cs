using Project.Scripts.Core.Events;
using Project.Scripts.Core.Utilities;
using Project.Scripts.Data;
using Zenject;

namespace Project.Scripts.UI.Common.Counters
{
    public class MoneyCounter : ACounterBase
    {
        [Inject] private GameplayEventProvider _gameplayEventProvider;
        [Inject] private ICurrencyDataService _currencyDataService;

        protected override void InitValue()
        {
            if (_currencyDataService == null)
                return;

            UpdateInfo(_currencyDataService.Money.Value);
        }

        protected override void SubscribeToEvents()
        {
            if (_gameplayEventProvider != null)
                _gameplayEventProvider.MoneyChangedEvent.AddListener(OnMoneyChangedEvent);
        }

        protected override void UnsubscribeFromEvents()
        {
            if (_gameplayEventProvider != null)
                _gameplayEventProvider?.MoneyChangedEvent.RemoveListener(OnMoneyChangedEvent);
        }

        protected override string FormatValue(long value)
        {
            if (value < 100)
                return $"<color=#00000000>0</color>{base.FormatValue(value)}<color=#00000000>0</color>";

            if (value < 1_000)
                return $"<color=#00000000>.</color>{base.FormatValue(value)}<color=#00000000>.</color>";

            return base.FormatValue(value);
        }

        protected void OnMoneyChangedEvent(long value = 0)
        {
            if (value < 0)
                value = _currencyDataService.Money.Value;
            UpdateInfo(value);
        }

        protected override MoneyConverter.ShortValueMode GetShortValueMode() =>
            MoneyConverter.ShortValueMode.All & ~MoneyConverter.ShortValueMode.Thousand &
            ~MoneyConverter.ShortValueMode.TenThousand;
    }
}