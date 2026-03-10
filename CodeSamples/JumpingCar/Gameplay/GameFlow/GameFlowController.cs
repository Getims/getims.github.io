using System;
using Project.Scripts.Gameplay.Background;
using Project.Scripts.Gameplay.Cars;
using Project.Scripts.Gameplay.Cars.Factory;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Gameplay.Platforms;
using Project.Scripts.Gameplay.Platforms.Factory;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.GameFlow
{
    public class GameFlowController : MonoBehaviour, IGameFlow
    {
        [SerializeField]
        private BackgroundController _backgroundController;

        [SerializeField]
        private CarsFactoryController _carsFactory;

        [SerializeField]
        private CarsController _carsController;

        [SerializeField]
        private PlatformsFactoryController _platformsFactoryController;

        [SerializeField]
        private PlatformsController _platformsController;

        [SerializeField]
        private CameraController _cameraController;

        [Inject] private GameEventsProvider _gameEventsProvider;
        [Inject] private ISoundService _soundService;

        private bool _isGameComplete = false;
        private bool _isLoadComplete = false;

        public bool IsLoadComplete => _isLoadComplete;
        public event Action OnGameOver;

        public void Initialize()
        {
            _gameEventsProvider.PlayerDeadEvent.AddListener(OnPlayerDead);
            _gameEventsProvider.PlayerLandedNewPlatformEvent.AddListener(OnPlayerLanded);
            _gameEventsProvider.PlayerLandedSamePlatformEvent.AddListener(OnPlayerLanded);
            _gameEventsProvider.PlayerJumpEvent.AddListener(OnJumpPressed);

            _backgroundController.Initialize();
            _cameraController.Initialize();
            _carsFactory.Initialize();
            _carsController.Initialize(_carsFactory);

            _platformsFactoryController.Initialize();
            _platformsController.Initialize(_platformsFactoryController);

            _isGameComplete = false;
            _isLoadComplete = true;
        }

        private void OnDestroy()
        {
            _gameEventsProvider.PlayerDeadEvent.RemoveListener(OnPlayerDead);
            _gameEventsProvider.PlayerLandedNewPlatformEvent.RemoveListener(OnPlayerLanded);
            _gameEventsProvider.PlayerLandedSamePlatformEvent.RemoveListener(OnPlayerLanded);
            _gameEventsProvider.PlayerJumpEvent.RemoveListener(OnJumpPressed);
        }

        private void OnPlayerDead()
        {
            if (_isGameComplete)
                return;

            _soundService.PlayCarHitSound();

            _carsController.Stop();
            _platformsController.Stop();
            _backgroundController.Stop();

            _isGameComplete = true;
            OnGameOver?.Invoke();
        }

        private void OnPlayerLanded(IPlatform obj)
        {
            _soundService.PlayCarLandedSound();
        }

        private void OnJumpPressed()
        {
            _soundService.PlayCarJumpSound();
        }
    }
}