using System.Collections.Generic;
using Project.Scripts.Configs.Background;
using Project.Scripts.Configs.Platforms;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Infrastructure.Utilities;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Background
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField]
        private List<BackgroundChunk> _backgroundChunks;

        [SerializeField]
        private SpriteRenderer _gradientSprite;

        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private GameEventsProvider _gameEventsProvider;

        private BackgroundMoveSystem _backgroundMoveSystem;
        private bool _isActive;

        public void Initialize()
        {
            var platformsConfigProvider = _configsProvider.GetConfig<PlatformsConfigProvider>();
            var backgroundConfigProvider = _configsProvider.GetConfig<BackgroundConfigProvider>();

            _backgroundMoveSystem =
                new BackgroundMoveSystem(_backgroundChunks, platformsConfigProvider, backgroundConfigProvider);

            var gradient = backgroundConfigProvider.BackgroundGradients.GetRandomElement();
            if (gradient != null)
                _gradientSprite.sprite = gradient;

            _gameEventsProvider.PlayerLandedNewPlatformEvent.AddListener(OnPlayerLandedNewPlatform);
            _gameEventsProvider.PlayerLandedSamePlatformEvent.AddListener(OnPlayerLandedSamePlatform);

            _isActive = true;
        }

        public void Stop()
        {
            _isActive = false;
            _backgroundMoveSystem.StopMove();
        }

        private void OnDestroy()
        {
            _gameEventsProvider.PlayerLandedNewPlatformEvent.RemoveListener(OnPlayerLandedNewPlatform);
            _gameEventsProvider.PlayerLandedSamePlatformEvent.RemoveListener(OnPlayerLandedSamePlatform);
        }

        private void OnPlayerLandedSamePlatform(IPlatform platform)
        {
            if (!_isActive)
                return;

            _backgroundMoveSystem.MoveOnSamePlatform();
        }

        private void OnPlayerLandedNewPlatform(IPlatform platform)
        {
            if (!_isActive)
                return;

            _backgroundMoveSystem.MoveOnNewPlatform();
        }
    }
}