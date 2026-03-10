using Lean.Pool;
using Project.Scripts.Configs.Platforms;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Platforms.Factory
{
    public class PlatformsFactoryController : MonoBehaviour, IPlatformsFactory
    {
        [SerializeField]
        private LeanGameObjectPool _platformsPool;

        [SerializeField]
        private Transform _platformsContainer;

        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private IPlatformsInfoService _platformsInfoService;

        private IPlatformsFactory _platformsFactory;

        public void Initialize()
        {
            var platformsConfigProvider = _configsProvider.GetConfig<PlatformsConfigProvider>();
            _platformsFactory = new PlatformsFactory(platformsConfigProvider, _platformsPool, _platformsContainer,
                _platformsInfoService);
        }

        public IPlatform Get(Vector3 position, int platformNumber) => _platformsFactory.Get(position, platformNumber);

        public void Return(IPlatform platform) => _platformsFactory.Return(platform);
    }
}