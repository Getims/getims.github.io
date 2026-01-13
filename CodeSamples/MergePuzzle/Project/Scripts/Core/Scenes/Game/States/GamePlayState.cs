using Project.Scripts.Gameplay;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Infrastructure.StateMachines.States;

namespace Project.Scripts.Core.Scenes.Game.States
{
    public class GamePlayState : IEnterState, IExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IGameFlowController _gameFlowController;

        public GamePlayState(GameStateMachine stateMachine, IGameFlowController gameFlowController)
        {
            _stateMachine = stateMachine;
            _gameFlowController = gameFlowController;
        }

        public void Enter()
        {
            _gameFlowController.OnGameOver += OnGameOver;
        }

        public void Exit()
        {
            _gameFlowController.OnGameOver -= OnGameOver;
        }

        private void OnGameOver(bool isWin)
        {
            _stateMachine.Enter<GameOverState, bool>(isWin);
        }
    }
}