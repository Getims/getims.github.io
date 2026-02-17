using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Assets;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Events;
using Project.Scripts.UI.LoadScreen;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Core
{
    public class GameCoreInstaller : MonoInstaller, ICoroutineRunner
    {
        [SerializeField]
        private GameCore _gameCore;

        [SerializeField]
        private LoadingPanel _loadingPanel;

        public override void InstallBindings()
        {
            BindProviders();
            BindDatabase();

            BindServices();

            BindFactories();
            BindGameStateMachine();
            BindSceneLoader();
        }

        public override void Start()
        {
            base.Start();

            var gameCore = new GameCore();
            Container.Inject(gameCore);
            gameCore.Initialize();
        }

        private void BindProviders()
        {
            Container.BindInterfacesTo<AssetProvider>().AsSingle().NonLazy();
            Container.BindInterfacesTo<ConfigsProvider>().AsSingle().NonLazy();
            Container.Bind<GlobalEventProvider>().AsSingle().NonLazy();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<GameDataService>().AsSingle().NonLazy();
        }

        private void BindDatabase()
        {
            IDatabase database = new GameDatabase();
            Container.BindInstance(database).AsSingle().NonLazy();

#if UNITY_EDITOR
            DataEditorMediator.SetDatabase(database);
#endif
        }

        private void BindFactories()
        {
            Container.Bind<StateMachineFactory>().AsSingle().NonLazy();
        }

        private void BindGameStateMachine() =>
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();

        private void BindSceneLoader()
        {
            Container.Bind<ICoroutineRunner>().FromInstance(this).AsSingle().NonLazy();
            Container.Bind<LoadingPanel>().FromInstance(_loadingPanel).AsSingle().NonLazy();
            Container.BindInterfacesTo<SceneLoader>().AsSingle().NonLazy();
        }
    }
}