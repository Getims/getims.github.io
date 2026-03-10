using Lean.Pool;
using Project.Scripts.Configs.Cars;
using Project.Scripts.Gameplay.Cars.Logic;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Cars.Factory
{
    public class CarsFactoryController : MonoBehaviour, ICarsFactory
    {
        [SerializeField]
        private LeanGameObjectPool _carsPool;

        [SerializeField]
        private PlayerCarController _playerCarController;

        [SerializeField]
        private Transform _carsContainer;

        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private GameEventsProvider _gameEventsProvider;
        [Inject] private IInputService _inputService;

        private CarsFactory _carsFactory;

        public void Initialize()
        {
            var unitsConfigProvider = _configsProvider.GetConfig<CarsConfigProvider>();

            _carsFactory = new CarsFactory(_carsPool, unitsConfigProvider, _carsContainer, _playerCarController,
                _gameEventsProvider, _inputService);
        }

        public ICar GetEnemyCar(IPlatform platform) => _carsFactory.GetEnemyCar(platform);
        public ICar GetPlayerCar(IPlatform platform) => _carsFactory.GetPlayerCar(platform);
        public void ReturnCar(ICar car) => _carsFactory.ReturnCar(car);
    }
}