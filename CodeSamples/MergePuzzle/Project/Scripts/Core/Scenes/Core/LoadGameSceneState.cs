using Project.Scripts.Core.Scenes.Loader;
using Project.Scripts.Infrastructure.ScenesManager;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Infrastructure.StateMachines.States;

namespace Project.Scripts.Core.Scenes.Core
{
    public class LoadGameLoaderSceneState : LoadSceneState, IEnterState
    {
        private readonly GameStateMachine _stateMachine;

        public LoadGameLoaderSceneState(GameStateMachine stateMachine, ISceneLoader sceneLoader) : base(sceneLoader) =>
            _stateMachine = stateMachine;

        public void Enter() =>
            base.Enter(Enums.Scenes.GameLoader, OnLoaded);

        private void OnLoaded() =>
            _stateMachine.Enter<GameLoaderState>();
    }

    public class LoadGameSceneState : LoadSceneState, IEnterState
    {
        private readonly GameStateMachine _stateMachine;

        public LoadGameSceneState(GameStateMachine stateMachine, ISceneLoader sceneLoader) : base(sceneLoader) =>
            _stateMachine = stateMachine;

        public void Enter() =>
            base.Enter(Enums.Scenes.Game, OnLoaded);

        private void OnLoaded() =>
            _stateMachine.Enter<GameLoopState>();
    }

    public class LoadMainMenuState : LoadSceneState, IEnterState
    {
        private readonly GameStateMachine _stateMachine;

        public LoadMainMenuState(GameStateMachine stateMachine, ISceneLoader sceneLoader) : base(sceneLoader) =>
            _stateMachine = stateMachine;

        public void Enter() =>
            base.Enter(Enums.Scenes.MainMenu, OnLoaded);

        private void OnLoaded() =>
            _stateMachine.Enter<GameLoopState>();
    }
}