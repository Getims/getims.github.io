using Lean.Pool;
using Project.Scripts.Configs.Platforms;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Platforms.Factory
{
    public interface IPlatformsFactory
    {
        IPlatform Get(Vector3 position, int platformNumber);
        void Return(IPlatform platform);
    }

    public class PlatformsFactory : IPlatformsFactory
    {
        private readonly LeanGameObjectPool _platformsPool;
        private readonly PlatformsConfigProvider _platformsConfigProvider;
        private readonly Transform _platformsContainer;
        private readonly IPlatformsInfoService _platformsInfoService;

        public PlatformsFactory(PlatformsConfigProvider platformsConfigProvider, LeanGameObjectPool platformsPool,
            Transform platformsContainer, IPlatformsInfoService platformsInfoService)
        {
            _platformsConfigProvider = platformsConfigProvider;
            _platformsPool = platformsPool;
            _platformsContainer = platformsContainer;
            _platformsInfoService = platformsInfoService;
        }

        public IPlatform Get(Vector3 position, int platformNumber)
        {
            var platform = _platformsPool.Spawn(position, Quaternion.identity, _platformsContainer)
                .GetComponent<Platform>();

            platform.Initialize(_platformsConfigProvider.ColorsConfig, platformNumber);
            platform.SetWidth(_platformsConfigProvider.PlatformWidth);
            platform.SetState(PlatformsState.Next);

            var info = _platformsInfoService.GetInfo(platformNumber);
            if (!string.IsNullOrEmpty(info))
            {
                platform.SetState(PlatformsState.Special);
                platform.SetInfo(info);
            }

            platform.gameObject.name = $"Platform_{platformNumber}";

            return platform;
        }

        public void Return(IPlatform platform)
        {
            _platformsPool.Despawn(platform.gameObject);
            platform.ResetState();
        }
    }
}