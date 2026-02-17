using System;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Scenes.Game.States;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Game
{
    public class GameSceneBootstrapper : IDisposable
    {
        private readonly DiContainer _container;
        private GameStateMachine _gameLoopStateMachine;

        public GameSceneBootstrapper(DiContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _gameLoopStateMachine = SetupStateMachine();
            _gameLoopStateMachine.Enter<PrepareGamePlayState>();
        }

        public void Dispose()
        {
            _gameLoopStateMachine.ExitCurrentState();
        }

        private GameStateMachine SetupStateMachine()
        {
            StateMachineFactory stateMachineFactory = _container.Instantiate<StateMachineFactory>();
            GameStateMachine gameLoopStateMachine = _container.Instantiate<GameStateMachine>();
            _container.BindInstance(gameLoopStateMachine);

            stateMachineFactory.BindState<PrepareGamePlayState>(gameLoopStateMachine);
            stateMachineFactory.BindState<GamePlayState>(gameLoopStateMachine);
            stateMachineFactory.BindState<ResetGameState>(gameLoopStateMachine);
            stateMachineFactory.BindState<ExitGameState>(gameLoopStateMachine);
            stateMachineFactory.BindState<GameOverState>(gameLoopStateMachine);
            return gameLoopStateMachine;
        }
    }
}