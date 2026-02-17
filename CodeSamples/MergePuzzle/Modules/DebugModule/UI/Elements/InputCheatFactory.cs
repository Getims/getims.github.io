using System;
using Project.Scripts.Modules.DebugModule.Core;
using UnityEngine;

namespace Project.Scripts.Modules.DebugModule.UI.Elements
{
    [Serializable]
    public class InputCheatFactory : ICheatElementFactory
    {
        [SerializeField]
        private InputCheatElement _prefab;

        public bool CanHandle(ICheat cheat) => cheat is ICheat<int>;

        public void Create(ICheat cheat, Transform container)
        {
            var typedCheat = (ICheat<int>)cheat;
            var element = GameObject.Instantiate(_prefab, container);
            element.Initialize(typedCheat);
        }
    }
}