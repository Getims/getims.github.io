using System;
using Project.Scripts.Data;
using Project.Scripts.Modules.DebugModule.Core;

namespace Project.Scripts.Modules.DebugModule.Cheats
{
    public class SetMoneyCheat : ICheat<int>
    {
        private readonly ICurrencyDataService _currencyDataService;

        public CheatGroupType GroupType => CheatGroupType.Base;
        public string Name => "Money";

        public SetMoneyCheat(ICurrencyDataService currencyDataService)
        {
            _currencyDataService = currencyDataService;
        }

        public void Execute(int value)
        {
            _currencyDataService.Money.Set(value);
        }

        public void Execute()
        {
            _currencyDataService.Money.Set(0);
        }

        public void Initialize(Action restartScene)
        {
        }
    }
}