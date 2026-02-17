using System;
using System.Linq;
using Project.Scripts.Modules.DebugModule.Core;
using UnityEngine;

namespace Project.Scripts.Modules.DebugModule.UI.Elements
{
    [Serializable]
    public class ButtonCheatFactory : ICheatElementFactory
    {
        [SerializeField]
        private ButtonCheatElement _prefab;

        public bool CanHandle(ICheat cheat)
        {
            var type = cheat.GetType();
            var interfaces = type.GetInterfaces();

            bool isGenericCheat = interfaces.Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICheat<>));

            return !isGenericCheat;
        }

        public void Create(ICheat cheat, Transform container)
        {
            var element = GameObject.Instantiate(_prefab, container);
            element.Initialize(cheat);
        }
    }
}