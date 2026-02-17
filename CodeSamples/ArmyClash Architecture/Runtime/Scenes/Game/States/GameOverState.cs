using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.UI.Game;
using Project.Scripts.UI.Game.Settings;
using Project.Scripts.UI.Game.Top;

namespace Project.Scripts.Runtime.Scenes.Game.States
{
    public class GameOverState : IEnterState, IExitState
    {
        private readonly GameStateMachine _stateMachine;

        private LevelCompletePanel _levelCompletePanel;
        private IGameUIController _gameUIController;
        private IGameResultService _gameResultService;

        public GameOverState(GameStateMachine stateMachine, IGameUIController gameUIController,
            IGameResultService gameResultService)
        {
            _gameUIController = gameUIController;
            _stateMachine = stateMachine;
            _gameResultService = gameResultService;
        }

        public void Enter()
        {
            SaveData();
            HideGameHud();
            CreateLevelCompletePanel();
        }

        public void Exit()
        {
            if (_levelCompletePanel != null)
                _levelCompletePanel.OnClaimClick -= OnLevelComplete;
        }

        private void SaveData()
        {
            _gameResultService.SaveResult();
        }

        private void HideGameHud()
        {
            _gameUIController.GetPanel<TopGamePanel>().Hide();
            _gameUIController.GetPanel<GameSettingsPopup>().Hide();
        }

        private void CreateLevelCompletePanel()
        {
            if (_levelCompletePanel != null)
                return;

            _levelCompletePanel = _gameUIController.GetPanel<LevelCompletePanel>();
            _levelCompletePanel.OnClaimClick += OnLevelComplete;
            _levelCompletePanel.Show();
        }

        private void OnLevelComplete()
        {
            _stateMachine.Enter<ExitGameState>();
        }
    }
}