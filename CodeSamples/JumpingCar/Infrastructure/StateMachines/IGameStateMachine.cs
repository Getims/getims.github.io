using System;

namespace Project.Scripts.Infrastructure.StateMachines
{
    public interface IGameStateMachine
    {
        void AddState<T>(T state) where T : IState;
        void ReplaceState<T>(T state) where T : IState;
        void RemoveState(Type type);
        void Enter<TState>() where TState : class, IEnterState;
        void Enter<TState, TParameter>(TParameter payload) where TState : class, IEnterState<TParameter>;
        void Tick();
    }
}