using Project.Scripts.Core.Events;
using Project.Scripts.Infrastructure.StateMachines.States;

namespace Project.Scripts.Core.Scenes.Game.States
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