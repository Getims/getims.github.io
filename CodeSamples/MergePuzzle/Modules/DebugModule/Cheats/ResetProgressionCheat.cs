using System;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Modules.DebugModule.Core;

namespace Project.Scripts.Modules.DebugModule.Cheats
{
    public class ResetProgressionCheat : ICheat
    {
        private ILevelsDataService _levelsDataService;
        private static IDatabase _database;
        private event Action OnNeedSceneRestart;

        public CheatGroupType GroupType => CheatGroupType.Base;

        public string Name => "Reset progression";

        public ResetProgressionCheat(IDatabase database, ILevelsDataService levelsDataService)
        {
            _levelsDataService = levelsDataService;
            _database = database;
        }

        public void Execute()
        {
            _database.DeleteData();
            _database.SaveData();
            _database.ReloadData();

            OnNeedSceneRestart?.Invoke();
        }

        public void Initialize(Action restartScene)
        {
            OnNeedSceneRestart = restartScene;
        }
    }
}