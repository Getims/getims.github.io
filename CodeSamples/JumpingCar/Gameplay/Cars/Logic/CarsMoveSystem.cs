using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars.Logic
{
    public class CarsMoveSystem
    {
        private readonly float _moveAccelerationPerPlatform;
        private float _platformWidth;

        public CarsMoveSystem(float platformWidth)
        {
            _platformWidth = platformWidth;
        }

        public void Update(ICar car)
        {
            var carInfo = car.CarInfo;
            var moveDirection = carInfo.MoveDirection;

            var directionMultiplier = moveDirection == Direction.Left ? -1 : 1;
            float delta = carInfo.Speed * directionMultiplier * Time.deltaTime;
            float newX = car.Position.x + delta;

            car.MoveHorizontal(newX);

            float carSize = carInfo.CarSize.x / 2;
            float rightEdge = _platformWidth / 2 - carSize;
            float leftEdge = -_platformWidth / 2 + carSize;

            if ((moveDirection == Direction.Right && car.Position.x >= rightEdge) ||
                (moveDirection == Direction.Left && car.Position.x <= leftEdge))
            {
                car.Flip();
            }
        }
    }
}