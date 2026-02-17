using Project.Scripts.Infrastructure.Utilities;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Loader
{
    public class GameLoaderSceneBootstrapper : MonoBehaviour
    {
        [Inject] private GlobalEventProvider _globalEventProvider;

        private void Start()
        {
            Utils.InfoPoint("You can add load screen, login screen, analytics and other logic before load game scene");

            SendCompleteEvent();
        }

        private void SendCompleteEvent() =>
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Enums.Scenes.MainMenu);
    }
}