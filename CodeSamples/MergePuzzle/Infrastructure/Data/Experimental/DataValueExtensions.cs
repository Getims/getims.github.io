using Project.Scripts.Data;

namespace Project.Scripts.Infrastructure.Data.Experimental
{
    public static class DataValueExtensions
    {
        public static void Add(this DataValue<CurrencyData, long> money, long amount, bool autoSave = true)
        {
            money.Set(money.Value + amount, autoSave);
        }

        public static void Spend(this DataValue<CurrencyData, long> money, long amount, bool autoSave = true)
        {
            money.Set(money.Value - amount, autoSave);
        }
    }
}