using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Scenes.Game.States;

namespace Project.Scripts.Runtime.Scenes.Game
{
    public class GameSceneCore
    {
        private GameStateMachine _gameLoopStateMachine;
        private StateMachineFactory _stateMachineFactory;

        public GameSceneCore(GameStateMachine gameLoopStateMachine, StateMachineFactory stateMachineFactory)
        {
            _stateMachineFactory = stateMachineFactory;
            _gameLoopStateMachine = gameLoopStateMachine;
        }

        public void Initialize()
        {
            SetupStateMachine();
            _gameLoopStateMachine.Enter<PrepareGamePlayState>();
        }

        private void SetupStateMachine()
        {
            _stateMachineFactory.BindState<PrepareGamePlayState>(_gameLoopStateMachine);
            _stateMachineFactory.BindState<GamePlayState>(_gameLoopStateMachine);
            _stateMachineFactory.BindState<ResetGameState>(_gameLoopStateMachine);
            _stateMachineFactory.BindState<ExitGameState>(_gameLoopStateMachine);
            _stateMachineFactory.BindState<GameOverState>(_gameLoopStateMachine);
        }
    }
}