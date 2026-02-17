using UnityEngine;

namespace Project.Scripts.Modules.DebugModule.Core
{
    public interface ICheatElementFactory
    {
        bool CanHandle(ICheat cheat);
        void Create(ICheat cheat, Transform container);
    }
}