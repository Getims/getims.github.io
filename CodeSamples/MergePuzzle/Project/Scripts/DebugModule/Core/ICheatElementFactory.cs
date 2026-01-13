using UnityEngine;

namespace Project.Scripts.DebugModule.Core
{
    public interface ICheatElementFactory
    {
        bool CanHandle(ICheat cheat);
        void Create(ICheat cheat, Transform container);
    }
}