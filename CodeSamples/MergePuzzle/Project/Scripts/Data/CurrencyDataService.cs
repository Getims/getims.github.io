using Project.Scripts.Core.Events;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Data.Values;

namespace Project.Scripts.Data
{
    public interface ICurrencyDataService
    {
        LongDataValue<CurrencyData> Money { get; }
        IntDataValue<CurrencyData> LivesCount { get; }
        IntDataValue<CurrencyData> HintsCount { get; }
        LongDataValue<CurrencyData> PendingReward { get; }
    }

    public class CurrencyDataService : ADataService<CurrencyData>, ICurrencyDataService
    {
        public LongDataValue<CurrencyData> Money { get; private set; }
        public IntDataValue<CurrencyData> LivesCount { get; private set; }
        public IntDataValue<CurrencyData> HintsCount { get; }
        public LongDataValue<CurrencyData> PendingReward { get; private set; }

        public CurrencyDataService(IDatabase database, GameplayEventProvider gameplayEventProvider) : base(database)
        {
            Money = CreateLongValue(
                data => data.Money,
                (data, value) => data.Money = value,
                gameplayEventProvider.MoneyChangedEvent.Invoke);
            PendingReward = CreateLongValue(
                data => data.PendingReward,
                (data, value) => data.PendingReward = value);
            LivesCount = CreateIntValue(
                data => data.LivesCount,
                (data, value) => data.LivesCount = value,
                gameplayEventProvider.LivesChangedEvent.Invoke);
            LivesCount = CreateIntValue(
                data => data.LivesCount,
                (data, value) => data.LivesCount = value,
                gameplayEventProvider.LivesChangedEvent.Invoke);
            HintsCount = CreateIntValue(
                data => data.HintsCount,
                (data, value) => data.HintsCount = value,
                gameplayEventProvider.HintsChangedEvent.Invoke);
        }
    }
}