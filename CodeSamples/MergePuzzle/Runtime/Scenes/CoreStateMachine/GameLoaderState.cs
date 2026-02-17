using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Events;
using UnityEngine;

namespace Project.Scripts.Runtime.Scenes.CoreStateMachine
{
    public class GameLoaderState : IEnterState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly GlobalEventProvider _globalEventsProvider;

        public GameLoaderState(IGameStateMachine stateMachine, GlobalEventProvider globalEventsProvider)
        {
            _globalEventsProvider = globalEventsProvider;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _globalEventsProvider.GameLoadCompleteEvent.AddListener(MoveToNextState);
        }

        private void MoveToNextState()
        {
            Resources.UnloadUnusedAssets();
            _globalEventsProvider.GameLoadCompleteEvent.RemoveListener(MoveToNextState);

            _stateMachine.Enter<LoadMainMenuState>();
        }
    }
}