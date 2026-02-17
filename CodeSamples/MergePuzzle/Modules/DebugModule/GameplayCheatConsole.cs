using System.Collections.Generic;
using Project.Scripts.Data;
using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Modules.DebugModule.Cheats;
using Project.Scripts.Modules.DebugModule.Core;
using Zenject;

namespace Project.Scripts.Modules.DebugModule
{
    public class GameplayCheatConsole : ACheatConsole
    {
        [Inject] private IDatabase _database;
        [Inject] private ILevelsDataService _levelsDataService;
        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private GameStateMachine _stateMachine;
        [Inject] private IGameFlowController _gameFlowController;
        [Inject] private ICurrencyDataService _currencyDataService;

        public override List<ICheat> CreateCheats()
        {
            _cheats = new List<ICheat>();
            AddCheat(new SetFPSCheat());
            AddCheat(new SetHintsCheat(_currencyDataService));
            AddCheat(new SetLevelCheat(_levelsDataService));
            AddCheat(new SetMoneyCheat(_currencyDataService));
            AddCheat(new CompleteLevelCheat(_gameFlowController));
            return _cheats;
        }
    }
}