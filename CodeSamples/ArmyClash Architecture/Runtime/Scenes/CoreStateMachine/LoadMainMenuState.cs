using Project.Scripts.Infrastructure.ScenesManager;
using Project.Scripts.Infrastructure.StateMachines;

namespace Project.Scripts.Runtime.Scenes.CoreStateMachine
{
    public class LoadMainMenuState : LoadSceneState, IEnterState
    {
        private readonly GameStateMachine _stateMachine;

        public LoadMainMenuState(GameStateMachine stateMachine, ISceneLoader sceneLoader) : base(sceneLoader) =>
            _stateMachine = stateMachine;

        public void Enter() =>
            base.Enter(Enums.Scenes.MainMenu, OnLoaded);

        private void OnLoaded() =>
            _stateMachine.Enter<WaitState>();
    }
}