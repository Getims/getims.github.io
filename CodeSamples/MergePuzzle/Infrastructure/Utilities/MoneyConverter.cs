using System;

namespace Project.Scripts.Infrastructure.Utilities
{
    public static class MoneyConverter
    {
        [Flags]
        public enum ShortValueMode
        {
            None = 0, // Без сокращений
            Thousand = 1 << 0, // Сокращение от 1K до 10K
            TenThousand = 1 << 1, // Сокращение от 10K до 100K
            HundredThousand = 1 << 2, // Сокращение от 100K до 1M
            Million = 1 << 3, // Сокращение от 1M и выше
            All = Thousand | TenThousand | HundredThousand | Million // Включает все уровни сокращений
        }

        public static string ConvertToShortValue(long money, ShortValueMode mode = ShortValueMode.All)
        {
            if (mode == ShortValueMode.None || money < 1_000)
                return money.ToString();

            if (mode.HasFlag(ShortValueMode.Thousand) && money < 10_000)
                return $"{money / 1_000f:0.##}K";

            if (mode.HasFlag(ShortValueMode.TenThousand) && money >= 10_000 && money < 100_000)
                return $"{money / 1_000f:0.#}K";

            if (mode.HasFlag(ShortValueMode.HundredThousand) && money >= 100_000 && money < 1_000_000)
                return $"{money / 1_000f:0.#}K";

            if (mode.HasFlag(ShortValueMode.HundredThousand) && money >= 1_000_000)
                return $"{money / 1_000_000f:0.##}M";

            return money.ToString();
        }
    }
}