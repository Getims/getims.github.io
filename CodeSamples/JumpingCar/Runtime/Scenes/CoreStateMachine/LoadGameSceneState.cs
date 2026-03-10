using Project.Scripts.Infrastructure.ScenesManager;
using Project.Scripts.Infrastructure.StateMachines;

namespace Project.Scripts.Runtime.Scenes.CoreStateMachine
{
    public class LoadGameSceneState : LoadSceneState, IEnterState
    {
        private readonly GameStateMachine _stateMachine;

        public LoadGameSceneState(GameStateMachine stateMachine, ISceneLoader sceneLoader) : base(sceneLoader) =>
            _stateMachine = stateMachine;

        public void Enter() =>
            base.Enter(Enums.Scenes.Game, OnLoaded);

        private void OnLoaded() =>
            _stateMachine.Enter<WaitState>();
    }
}