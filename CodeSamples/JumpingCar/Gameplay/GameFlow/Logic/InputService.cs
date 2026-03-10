using System;

namespace Project.Scripts.Gameplay.GameFlow.Logic
{
    public interface IInputService
    {
        event Action OnJumpPressed;
        event Action OnJumpReleased;
        void PressJump();
        void ReleaseJump();
    }

    public class InputService : IInputService
    {
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;

        public void PressJump() => OnJumpPressed?.Invoke();

        public void ReleaseJump() => OnJumpReleased?.Invoke();
    }
}