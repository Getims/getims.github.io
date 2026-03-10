using Project.Scripts.Infrastructure.Utilities;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Loader
{
    public class GameLoaderSceneBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private float _completeMinDelay = 0.25f;

        [Inject] private GlobalEventProvider _globalEventProvider;

        private void Start()
        {
            Utils.InfoPoint("You can add load screen, login screen, analytics and other logic before load game scene");

            Utils.PerformWithDelay(this, _completeMinDelay, SendCompleteEvent);
        }

        private void SendCompleteEvent() =>
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Enums.Scenes.Game);
    }
}