using Project.Scripts.Core.Events;
using Project.Scripts.Infrastructure.StateMachines.States;

namespace Project.Scripts.Core.Scenes.Game.States
{
    public class ResetGameState : IEnterState
    {
        private GlobalEventProvider _globalEventProvider;

        public ResetGameState(GlobalEventProvider globalEventProvider)
        {
            _globalEventProvider = globalEventProvider;
        }

        public void Enter()
        {
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Enums.Scenes.Game);
        }
    }
}