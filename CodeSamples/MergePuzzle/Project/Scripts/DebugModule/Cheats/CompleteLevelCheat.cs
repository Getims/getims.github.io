using System;
using Project.Scripts.DebugModule.Core;
using Project.Scripts.Gameplay;

namespace Project.Scripts.DebugModule.Cheats
{
    public class CompleteLevelCheat : ICheat
    {
        private IGameFlowController _gameFlowController;

        public CheatGroupType GroupType => CheatGroupType.Gameplay;
        public string Name => "Complete Level";

        public CompleteLevelCheat(IGameFlowController gameFlowController)
        {
            _gameFlowController = gameFlowController;
        }

        public void Execute()
        {
            _gameFlowController.SetGameOver(true);
        }

        public void Initialize(Action restartScene)
        {
        }
    }
}