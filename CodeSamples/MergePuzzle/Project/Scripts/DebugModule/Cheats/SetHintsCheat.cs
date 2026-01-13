using System;
using Project.Scripts.Data;
using Project.Scripts.DebugModule.Core;

namespace Project.Scripts.DebugModule.Cheats
{
    public class SetHintsCheat : ICheat<int>
    {
        private readonly ICurrencyDataService _currencyDataService;

        public CheatGroupType GroupType => CheatGroupType.Gameplay;
        public string Name => "Hints";

        public SetHintsCheat(ICurrencyDataService currencyDataService)
        {
            _currencyDataService = currencyDataService;
        }

        public void Execute(int value)
        {
            _currencyDataService.HintsCount.Set(value);
        }

        public void Execute()
        {
            _currencyDataService.HintsCount.Set(0);
        }

        public void Initialize(Action restartScene)
        {
        }
    }
}