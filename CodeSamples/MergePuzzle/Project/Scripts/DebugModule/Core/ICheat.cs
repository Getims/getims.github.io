using System;

namespace Project.Scripts.DebugModule.Core
{
    public interface ICheat
    {
        string Name { get; }
        CheatGroupType GroupType { get; }
        void Execute();
        void Initialize(Action restartScene);
    }

    public interface ICheat<T> : ICheat
    {
        void Execute(T value);
    }
}