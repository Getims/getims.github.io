using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Events;

namespace Project.Scripts.Runtime.Scenes.Game.States
{
    public class ExitGameState : IEnterState
    {
        private readonly GlobalEventProvider _globalEventProvider;

        public ExitGameState(GlobalEventProvider globalEventProvider)
        {
            _globalEventProvider = globalEventProvider;
        }

        public void Enter()
        {
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Enums.Scenes.MainMenu);
        }
    }
}