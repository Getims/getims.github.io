using Project.Scripts.Configs;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.UI.Common;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Menu
{
    public class MenuSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private UIContainerProvider _uiContainerProvider;

        public override void InstallBindings()
        {
            BindSceneObjects();
            BindUIFactory();
        }

        private void BindSceneObjects()
        {
            Container.BindInstance(_uiContainerProvider).AsSingle().NonLazy();
        }

        private void BindUIFactory()
        {
            var uiConfig = Container.Resolve<IConfigsProvider>().GetConfig<UIConfig>();
            UIFactory uiFactory = new UIFactory(Container, uiConfig, _uiContainerProvider.MenuContainer);
            Container.Bind<IUIFactory>().FromInstance(uiFactory).AsSingle().NonLazy();
        }
    }
}