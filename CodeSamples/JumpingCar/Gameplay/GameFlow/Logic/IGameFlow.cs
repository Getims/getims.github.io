using System;

namespace Project.Scripts.Gameplay.GameFlow.Logic
{
    public interface IGameFlow
    {
        bool IsLoadComplete { get; }

        void Initialize();
        event Action OnGameOver;
    }
}