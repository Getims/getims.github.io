using System.Collections.Generic;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Modules.DebugModule.Cheats;
using Project.Scripts.Modules.DebugModule.Core;
using Zenject;

namespace Project.Scripts.Modules.DebugModule
{
    public class MenuCheatConsole : ACheatConsole
    {
        [Inject] private ICurrencyDataService _currencyDataService;
        [Inject] private IDatabase _database;
        [Inject] private ILevelsDataService _levelsDataService;

        public override List<ICheat> CreateCheats()
        {
            _cheats = new List<ICheat>();
            AddCheat(new SetFPSCheat());
            AddCheat(new SetMoneyCheat(_currencyDataService));
            AddCheat(new ResetProgressionCheat(_database, _levelsDataService));
            AddCheat(new SetLevelCheat(_levelsDataService));
            AddCheat(new SetLevelAnimatedCheat(_levelsDataService));
            return _cheats;
        }
    }
}