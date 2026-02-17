using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Events;

namespace Project.Scripts.Runtime.Scenes.Game.States
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