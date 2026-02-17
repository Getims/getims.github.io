using System;
using Project.Scripts.Infrastructure.Data;

namespace Project.Scripts.Data
{
    [Serializable]
    public class CurrencyData : GameData
    {
        public long Money = 5000;
        public int LivesCount = 1;
        public int HintsCount = 3;
        public long PendingReward = 0;
    }
}