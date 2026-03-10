using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Configs.Platforms;
using Project.Scripts.Gameplay.Platforms.Factory;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Enums;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Platforms
{
    public class PlatformsController : MonoBehaviour
    {
        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private GameEventsProvider _gameEventsProvider;
        [Inject] private IPlatformsInfoService _platformsInfoService;

        private PlatformsConfigProvider _platformsConfigProvider;
        private IPlatformsFactory _platformsFactory;
        private PlatformsMoveSystem _platformsMoveSystem;

        private List<IPlatform> _platforms = new List<IPlatform>();
        private int _platformCounter = 0;
        private float _screenHeight;
        private bool _isActive;

        public void Initialize(IPlatformsFactory platformsFactory)
        {
            _platformsFactory = platformsFactory;
            _platformsConfigProvider = _configsProvider.GetConfig<PlatformsConfigProvider>();

            _screenHeight = Camera.main.orthographicSize * 2f;

            CreateMoveSystem();
            CreatePlatforms();

            _gameEventsProvider.PlayerLandedNewPlatformEvent.AddListener(OnPlayerLandedNewPlatform);
            _gameEventsProvider.PlayerLandedSamePlatformEvent.AddListener(OnPlayerLandedSamePlatform);

            _isActive = true;
        }

        public void Stop()
        {
            _isActive = false;
            _platformsMoveSystem.StopMove();
        }

        private void OnDestroy()
        {
            _platformsMoveSystem.OnMoveComplete -= CheckAndManagePlatforms;

            _gameEventsProvider.PlayerLandedNewPlatformEvent.RemoveListener(OnPlayerLandedNewPlatform);
            _gameEventsProvider.PlayerLandedSamePlatformEvent.RemoveListener(OnPlayerLandedSamePlatform);
        }

        private void CreateMoveSystem()
        {
            _platformsMoveSystem = new PlatformsMoveSystem(_platformsConfigProvider);
            _platformsMoveSystem.OnMoveComplete += CheckAndManagePlatforms;
        }

        private void CreatePlatforms()
        {
            var bottomScreenY = -_screenHeight / 2f;
            var initialY = bottomScreenY + _platformsConfigProvider.PlatformSpacing;

            var platformsOnScreen = Mathf.CeilToInt(_screenHeight / _platformsConfigProvider.PlatformSpacing) + 1;
            for (int i = 0; i < platformsOnScreen; i++)
            {
                var position = new Vector3(0, initialY + i * _platformsConfigProvider.PlatformSpacing, 0);
                CreateAndSetupPlatform(position);
            }
        }

        private void CreateAndSetupPlatform(Vector3 position)
        {
            var newPlatform = _platformsFactory.Get(position, _platformCounter);
            _platforms.Add(newPlatform);

            _gameEventsProvider.PlatformCreateEvent.Invoke(newPlatform);
            _platformCounter++;
        }

        private void CreateAndSetupPlatform()
        {
            var spawnY = _platforms.LastOrDefault()?.Position.y + _platformsConfigProvider.PlatformSpacing ?? 0;
            CreateAndSetupPlatform(new Vector3(0, spawnY, 0));
        }

        private void CheckAndManagePlatforms()
        {
            TryCreateNewPlatforms();
            RecyclePlatforms();
        }

        private void RecyclePlatforms()
        {
            if (Camera.main == null) return;

            var bottomY = -Camera.main.orthographicSize;

            for (int i = _platforms.Count - 1; i >= 0; i--)
            {
                var platform = _platforms[i];
                if (platform.Position.y < bottomY - _platformsConfigProvider.PlatformSpacing) //Небольшой запас
                {
                    _platforms.RemoveAt(i);
                    _platformsFactory.Return(platform);
                }
            }
        }

        private void TryCreateNewPlatforms()
        {
            if (Camera.main == null) return;

            var topY = Camera.main.orthographicSize;
            var highestPlatformY = _platforms.LastOrDefault()?.Position.y ?? -Mathf.Infinity;

            while (highestPlatformY < topY + _platformsConfigProvider.PlatformSpacing) //Небольшой запас
            {
                CreateAndSetupPlatform();
                highestPlatformY = _platforms.Last().Position.y;
            }
        }

        private void OnPlayerLandedSamePlatform(IPlatform platform)
        {
            if (!_isActive)
                return;

            _platformsMoveSystem.MoveOnSamePlatform(_platforms);
        }

        private void OnPlayerLandedNewPlatform(IPlatform platform)
        {
            if (!_isActive)
                return;

            _platformsMoveSystem.MoveOnNewPlatform(_platforms);
            platform.SetState(PlatformsState.Current);
            _platformsInfoService.SetCurrentPlatform(platform.Number);
        }
    }
}