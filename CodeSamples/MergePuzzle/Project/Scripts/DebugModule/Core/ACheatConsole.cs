using System.Collections.Generic;
using Project.Scripts.Core.Enums;
using Project.Scripts.Core.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.DebugModule.Core
{
    public abstract class ACheatConsole : MonoBehaviour
    {
        [Inject] private GlobalEventProvider _globalEventProvider;
        protected List<ICheat> _cheats;

        public abstract List<ICheat> CreateCheats();

        protected void AddCheat(ICheat cheat)
        {
            _cheats.Add(cheat);
            cheat.Initialize(RestartScene);
        }

        private void RestartScene()
        {
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Scenes.NULL);
        }
    }
}