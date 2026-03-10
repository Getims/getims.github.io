using Lean.Pool;
using Project.Scripts.Configs.Cars;
using Project.Scripts.Gameplay.Cars.Logic;
using Project.Scripts.Gameplay.Cars.Logic.Player;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Runtime.Events;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars.Factory
{
    public interface ICarsFactory
    {
        public ICar GetEnemyCar(IPlatform platform);
        public ICar GetPlayerCar(IPlatform platform);
        void ReturnCar(ICar car);
    }

    public class CarsFactory : ICarsFactory
    {
        private readonly LeanGameObjectPool _carsPool;
        private readonly CarsConfigProvider _carsConfigProvider;
        private readonly Transform _carsContainer;
        private readonly PlayerCarController _playerCarController;
        private readonly GameEventsProvider _gameEventsProvider;
        private readonly IInputService _inputService;

        public CarsFactory(LeanGameObjectPool carsPool, CarsConfigProvider carsConfigProvider, Transform carsContainer,
            PlayerCarController playerCarController, GameEventsProvider gameEventsProvider, IInputService inputService)
        {
            _gameEventsProvider = gameEventsProvider;
            _playerCarController = playerCarController;
            _carsContainer = carsContainer;
            _carsConfigProvider = carsConfigProvider;
            _carsPool = carsPool;
            _inputService = inputService;
        }

        public ICar GetEnemyCar(IPlatform platform)
        {
            var carConfig = _carsConfigProvider.GetEnemyCar(platform.Number);

            GameObject instance = GetCarFromPool(platform);
            var carController = GetController(instance, carConfig, "Enemy");

            var car = new EnemyCar(carController, platform);
            car.Initialize(carConfig);

            return car;
        }

        public ICar GetPlayerCar(IPlatform platform)
        {
            PlayerCarController carController = GameObject.Instantiate(_playerCarController, _carsContainer);

            var car = new PlayerCar(carController, platform, _carsConfigProvider.PlayerConfig, _gameEventsProvider,
                _inputService);
            car.Initialize(_carsConfigProvider.PlayerConfig.CarConfig);

            return car;
        }

        public void ReturnCar(ICar car)
        {
            _carsPool.Despawn(car.GameObject);
            car.Dispose();
        }

        private CarController GetController(GameObject instance, CarConfig carConfig, string tag)
        {
            string name = $"{tag} car {carConfig.name}";
            instance.name = name;

            return instance.GetComponent<CarController>();
        }

        private GameObject GetCarFromPool(IPlatform platform)
        {
            var position = platform.Position;
            return _carsPool.Spawn(position, Quaternion.identity, _carsContainer);
        }
    }
}