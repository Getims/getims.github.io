using System;
using Project.Scripts.Configs.Cars;
using Project.Scripts.Gameplay.Cars.Data;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars.Logic
{
    public interface ICar : IDisposable
    {
        ICarInfo CarInfo { get; }
        Vector3 Position { get; }
        GameObject GameObject { get; }

        void Flip();
        void Initialize(CarConfig carConfig);
        void Update();

        void MoveHorizontal(float value);
        void MoveVertical(float value);
        bool CheckAlive();
    }

    public abstract class CarBase : ICar
    {
        protected readonly CarInfo _carInfo;
        protected readonly CarController _carController;

        public ICarInfo CarInfo => _carInfo;
        public Vector3 Position => _carController.transform.position;
        public GameObject GameObject => _carController.GameObject;

        public CarBase(CarController carController)
        {
            _carController = carController;
            _carInfo = new CarInfo();
        }

        public virtual void Initialize(CarConfig carConfig)
        {
            _carController.Initialize(carConfig);
            _carController.SetDirection(_carInfo.MoveDirection);

            _carInfo.SetSize(_carController.GetSpriteSize());
            _carInfo.SetSpeed(carConfig.Speed);
        }

        public void Flip()
        {
            _carInfo.SetDirection(_carInfo.MoveDirection == Direction.Left ? Direction.Right : Direction.Left);
            _carController.SetDirection(_carInfo.MoveDirection);
        }

        public void MoveHorizontal(float value)
        {
            var position = Position;
            position.x = value;
            _carController.Transform.position = position;
        }

        public void MoveVertical(float value)
        {
            var position = Position;
            position.y = value;
            _carController.Transform.position = position;
        }

        public abstract bool CheckAlive();
        public abstract void Update();

        public virtual void Dispose()
        {
        }
    }
}