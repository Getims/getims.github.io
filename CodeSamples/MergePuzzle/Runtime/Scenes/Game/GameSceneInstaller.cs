using Project.Scripts.Configs;
using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.UI.Common;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Game
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private UIContainerProvider _uiContainerProvider;

        [SerializeField]
        private GameFlowController _gameFlowController;

        public override void InstallBindings()
        {
            BindSceneObjects();
            BindFactories();
            CreateSceneBootstrapper();
        }

        private void BindSceneObjects()
        {
            Container.BindInstance(_uiContainerProvider).AsSingle().NonLazy();
            Container.Bind<IGameFlowController>().FromInstance(_gameFlowController).AsSingle().NonLazy();
        }

        private void BindFactories()
        {
            BindUIFactory();
        }

        private void BindUIFactory()
        {
            var uiConfig = Container.Resolve<IConfigsProvider>().GetConfig<UIConfig>();
            UIFactory uiFactory = new UIFactory(Container, uiConfig, _uiContainerProvider.MenuContainer);
            Container.Bind<IUIFactory>().FromInstance(uiFactory).AsSingle().NonLazy();
        }

        private void CreateSceneBootstrapper()
        {
            GameSceneBootstrapper bootstrapper = Container.Instantiate<GameSceneBootstrapper>();
            bootstrapper.Initialize();
        }
    }
}