using Project.Scripts.Data;
using Project.Scripts.Data.Audio;
using Project.Scripts.Infrastructure.Assets;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.Runtime.Events;
using Project.Scripts.UI.Common;
using Project.Scripts.UI.Common.Anchors;
using Project.Scripts.UI.LoadScreen;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Core
{
    public class GameCoreInstaller : MonoInstaller
    {
        [SerializeField]
        private SoundPlayer _soundPlayer;

        [SerializeField]
        private GameCoreBootstrapper _gameCoreBootstrapper;

        [SerializeField]
        private LoadingPanel _loadingPanel;

        public override void InstallBindings()
        {
            BindProviders();
            BindDatabase();

            BindServices();
            BindSoundPlayer();

            BindFactories();
            BindGameStateMachine();

            BindGameBootstrapper();
            BindSceneLoader();
        }

        private void BindProviders()
        {
            Container.BindInterfacesTo<AssetProvider>().AsSingle().NonLazy();
            Container.BindInterfacesTo<ConfigsProvider>().AsSingle().NonLazy();
            Container.Bind<GlobalEventProvider>().AsSingle().NonLazy();
            Container.Bind<GameplayEventProvider>().AsSingle();
            Container.Bind<MenuEventsProvider>().AsSingle();
            Container.BindInterfacesTo<UIAnchorsProvider>().AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<SoundDataService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<CurrencyDataService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelsDataService>().AsSingle().NonLazy();
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
            Container.BindInterfacesTo<IUIFactory>().AsSingle().NonLazy();
        }

        private void BindGameStateMachine() =>
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();

        private void BindGameBootstrapper() =>
            Container.Bind<ICoroutineRunner>().FromInstance(_gameCoreBootstrapper).AsSingle().NonLazy();

        private void BindSceneLoader()
        {
            Container.Bind<LoadingPanel>().FromInstance(_loadingPanel).AsSingle().NonLazy();
            Container.BindInterfacesTo<SceneLoader>().AsSingle().NonLazy();
        }

        private void BindSoundPlayer()
        {
            Container.Bind<SoundPlayer>().FromInstance(_soundPlayer).AsSingle().NonLazy();
            Container.BindInterfacesTo<SoundService>().AsSingle().NonLazy();
        }
    }
}