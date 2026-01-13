using Project.Scripts.Core.Events;
using Project.Scripts.Core.Scenes.Core;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Infrastructure.StateMachines.States;
using UnityEngine;

namespace Project.Scripts.Core.Scenes.Loader
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