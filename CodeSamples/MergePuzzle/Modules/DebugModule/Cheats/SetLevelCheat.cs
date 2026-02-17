using System;
using Project.Scripts.Data;
using Project.Scripts.Modules.DebugModule.Core;

namespace Project.Scripts.Modules.DebugModule.Cheats
{
    public class SetLevelCheat : ICheat<int>
    {
        private readonly ILevelsDataService _levelsDataService;
        private event Action OnNeedSceneRestart;

        public CheatGroupType GroupType => CheatGroupType.Gameplay;
        public string Name => "Level";

        public SetLevelCheat(ILevelsDataService levelsDataService)
        {
            _levelsDataService = levelsDataService;
        }

        public void Execute(int value)
        {
            _levelsDataService.CurrentLevel.Set(value - 1, true);
            _levelsDataService.LastPassedLevel.Set(value - 2, true);
            OnNeedSceneRestart?.Invoke();
        }

        public void Execute()
        {
            Execute(1);
        }

        public void Initialize(Action restartScene)
        {
            OnNeedSceneRestart = restartScene;
        }
    }
}