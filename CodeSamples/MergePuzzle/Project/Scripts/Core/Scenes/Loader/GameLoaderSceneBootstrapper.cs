using Project.Scripts.Core.Events;
using Project.Scripts.Core.Sounds;
using Project.Scripts.Core.Utilities;
using Project.Scripts.UI.LoadScreen;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Core.Scenes.Loader
{
    public class GameLoaderSceneBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private LoadingProgressBarPanel _loadingProgressBarPanel;

        [Inject] private GlobalEventProvider _globalEventsProvider;
        [Inject] private ISoundService _soundService;

        private void Start()
        {
            Utils.InfoPoint("You can add load screen, login screen, analytics and other logic before load game scene");

            _soundService.PlayGameBackgroundMusic();

            _loadingProgressBarPanel.Fill(SendCompleteEvent);
        }

        private void SendCompleteEvent() =>
            _globalEventsProvider.GameLoadCompleteEvent.Invoke();
    }
}