using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.UI.Game;
using Project.Scripts.UI.Game.Settings;
using Project.Scripts.UI.Game.Top;

namespace Project.Scripts.Runtime.Scenes.Game.States
{
    public class GamePlayState : IEnterState, IExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IGameFlow _gameFlowController;
        private readonly IGameUIController _gameUIController;

        public GamePlayState(GameStateMachine stateMachine, IGameFlow gameFlowController,
            IGameUIController gameUIController)
        {
            _stateMachine = stateMachine;
            _gameFlowController = gameFlowController;
            _gameUIController = gameUIController;
        }

        public void Enter()
        {
            _gameFlowController.StartBattle();
            _gameFlowController.OnGameOver += OnGameOver;
            SubscribeUI();
        }

        public void Exit()
        {
            _gameFlowController.OnGameOver -= OnGameOver;
            UnsubscribeUI();
        }

        private void OnGameOver()
        {
            _stateMachine.Enter<GameOverState>();
        }

        private void SubscribeUI()
        {
            var topGamePanel = _gameUIController.ShowPanel<TopGamePanel>();
            topGamePanel.OnSettingsClick += ShowSettings;

            var settingsPopup = _gameUIController.GetPanel<GameSettingsPopup>();
            settingsPopup.OnRestartClick += RestartLevel;
            settingsPopup.OnExitClick += ExitLevel;
        }

        private void UnsubscribeUI()
        {
            var topGamePanel = _gameUIController.GetPanel<TopGamePanel>();
            topGamePanel.OnSettingsClick -= ShowSettings;

            var settingsPopup = _gameUIController.GetPanel<GameSettingsPopup>();
            settingsPopup.OnRestartClick -= RestartLevel;
            settingsPopup.OnExitClick -= ExitLevel;
        }

        private void ShowSettings()
        {
            _gameUIController.ShowPanel<GameSettingsPopup>();
        }

        private void RestartLevel() =>
            _stateMachine.Enter<ResetGameState>();

        private void ExitLevel()
        {
            _stateMachine.Enter<ExitGameState>();
        }
    }
}