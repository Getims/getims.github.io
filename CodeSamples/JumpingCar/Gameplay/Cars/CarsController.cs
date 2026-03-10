using System.Collections.Generic;
using Project.Scripts.Configs.Cars;
using Project.Scripts.Configs.Platforms;
using Project.Scripts.Gameplay.Cars.Factory;
using Project.Scripts.Gameplay.Cars.Logic;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Cars
{
    public class CarsController : MonoBehaviour
    {
        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private GameEventsProvider _gameEventsProvider;

        private CarsMoveSystem _carsMoveSystem;
        private ICarsFactory _carsFactory;
        private readonly List<ICar> _cars = new List<ICar>();

        private bool _isActive;
        private bool _playerCreated;
        private int _createEnemyFromPlatFormNumber;

        public void Initialize(ICarsFactory carsFactory)
        {
            var carsConfigProvider = _configsProvider.GetConfig<CarsConfigProvider>();
            var platformsConfigProvider = _configsProvider.GetConfig<PlatformsConfigProvider>();
            _createEnemyFromPlatFormNumber = carsConfigProvider.CreateEnemyFromPlatFormNumber;

            _carsFactory = carsFactory;
            _carsMoveSystem = new CarsMoveSystem(platformsConfigProvider.PlatformWidth);
            _isActive = true;

            _gameEventsProvider.PlatformCreateEvent.AddListener(CreateCar);
        }

        private void CreateCar(IPlatform platform)
        {
            if (!_playerCreated)
            {
                var car = _carsFactory.GetPlayerCar(platform);
                _cars.Add(car);
                _playerCreated = true;
                return;
            }

            if (platform.Number >= _createEnemyFromPlatFormNumber)
            {
                var enemyCar = _carsFactory.GetEnemyCar(platform);
                _cars.Add(enemyCar);
            }
        }

        public void Stop()
        {
            _isActive = false;
        }

        private void LateUpdate()
        {
            if (!_isActive)
                return;

            for (int i = _cars.Count - 1; i >= 0; i--)
            {
                var car = _cars[i];

                if (car.CheckAlive() == false)
                {
                    RemoveCar(car);
                    continue;
                }

                _carsMoveSystem.Update(car);
                car.Update();
            }
        }

        private void RemoveCar(ICar car)
        {
            _cars.Remove(car);
            _carsFactory.ReturnCar(car);
        }

        private void OnDestroy()
        {
            _gameEventsProvider.PlatformCreateEvent.RemoveListener(CreateCar);
        }
    }
}