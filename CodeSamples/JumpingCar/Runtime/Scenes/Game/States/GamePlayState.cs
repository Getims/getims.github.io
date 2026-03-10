using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.UI.Game;

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
            _gameFlowController.OnGameOver += OnGameOver;
            InitializeUI();
        }

        public void Exit()
        {
            _gameFlowController.OnGameOver -= OnGameOver;
            DisposeUI();
        }

        private void OnGameOver()
        {
            _stateMachine.Enter<GameOverState>();
        }

        private void InitializeUI()
        {
            var topGamePanel = _gameUIController.ShowPanel<TopGamePanel>();
            topGamePanel.OnRestartClick += RestartGame;

            _gameUIController.ShowPanel<InputPanel>();
        }

        private void DisposeUI()
        {
            var topGamePanel = _gameUIController.GetPanel<TopGamePanel>();
            topGamePanel.OnRestartClick -= RestartGame;
        }

        private void RestartGame() =>
            _stateMachine.Enter<ResetGameState>();
    }
}