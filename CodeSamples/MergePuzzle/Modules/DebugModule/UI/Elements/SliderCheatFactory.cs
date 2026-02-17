using System;
using Project.Scripts.Modules.DebugModule.Core;
using UnityEngine;

namespace Project.Scripts.Modules.DebugModule.UI.Elements
{
    [Serializable]
    public class SliderCheatFactory : ICheatElementFactory
    {
        [SerializeField]
        private SliderCheatElement _prefab;

        public bool CanHandle(ICheat cheat) => cheat is ICheat<float>;

        public void Create(ICheat cheat, Transform container)
        {
            var typedCheat = (ICheat<float>)cheat;
            var element = GameObject.Instantiate(_prefab, container);
            element.Initialize(typedCheat);
        }
    }
}