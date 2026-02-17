using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Enums;
using Project.Scripts.Runtime.Events;
using Project.Scripts.UI.Common;
using Project.Scripts.UI.MainMenu.Main;
using Zenject;

namespace Project.Scripts.UI.MainMenu
{
    public class MainMenuUIController : AUIController
    {
        [Inject] private GlobalEventProvider _globalEventProvider;
        [Inject] private IConfigsProvider _configsProvider;

        protected override void Start()
        {
            base.Start();
            ShowMainPanel();
        }

        private void ShowMainPanel()
        {
            var panel = ShowPanel<MenuPanel>();
            panel.OnStartLevelOpenRequest += ShowStartLevelPopup;
        }

        private void ShowStartLevelPopup()
        {
            StartLevel();
        }

        private void StartLevel()
        {
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Scenes.Game);
        }
    }
}