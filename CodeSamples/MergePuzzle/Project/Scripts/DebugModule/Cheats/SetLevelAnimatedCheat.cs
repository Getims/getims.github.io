using System;
using Project.Scripts.Core.Constants;
using Project.Scripts.Data;
using Project.Scripts.DebugModule.Core;

namespace Project.Scripts.DebugModule.Cheats
{
    public class SetLevelAnimatedCheat : ICheat<int>
    {
        private readonly ILevelsDataService _levelsDataService;
        private event Action OnNeedSceneRestart;

        public CheatGroupType GroupType => CheatGroupType.Gameplay;
        public string Name => "Level(animated)";

        public SetLevelAnimatedCheat(ILevelsDataService levelsDataService)
        {
            _levelsDataService = levelsDataService;
        }

        public void Execute(int value)
        {
            if (value < 1)
                value = 1;

            _levelsDataService.CollectionsUnlocked.Set((value - 2) / GameConstants.LEVELS_PER_COLLECTION, true);
            _levelsDataService.CurrentLevel.Set(value - 2, true);
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