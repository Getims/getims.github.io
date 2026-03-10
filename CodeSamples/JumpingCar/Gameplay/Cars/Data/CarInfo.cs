using Project.Scripts.Runtime;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars.Data
{
    public interface ICarInfo
    {
        float Speed { get; }
        Direction MoveDirection { get; }
        Vector3 CarSize { get; }
    }

    public class CarInfo : ICarInfo
    {
        private float _initialSpeed;
        private float _acceleration;
        private Direction _moveDirection = Direction.Right;
        private Vector3 _carSize;

        public float Speed => _initialSpeed + _acceleration;
        public Direction MoveDirection => _moveDirection;
        public Vector3 CarSize => _carSize;

        public CarInfo()
        {
            _initialSpeed = 0;
            _acceleration = 0;
            _moveDirection = RuntimeUtils.RandomDirection(Direction.Left, Direction.Right);
        }

        public void SetSpeed(float speed)
        {
            _initialSpeed = speed;
        }

        public void SetDirection(Direction direction)
        {
            _moveDirection = direction;
        }

        public void SetAcceleration(float acceleration)
        {
            if (acceleration < 0)
                return;

            _acceleration = acceleration;
        }

        public void SetSize(Vector3 size)
        {
            _carSize = size;
        }
    }
}