using Project.Scripts.Data;
using Project.Scripts.Runtime.Events;
using TMPro;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Common.Counters
{
    public class HintsTracker : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _valueTMP;

        [Inject] private GameplayEventProvider _gameplayEventProvider;
        [Inject] private ICurrencyDataService _currencyDataService;

        private void Start()
        {
            _gameplayEventProvider?.HintsChangedEvent.AddListener(UpdateInfo);
            UpdateInfo();
        }

        private void OnDestroy()
        {
            _gameplayEventProvider?.HintsChangedEvent.RemoveListener(UpdateInfo);
        }

        private void UpdateInfo()
        {
            UpdateInfo(_currencyDataService.HintsCount.Value);
        }

        private void UpdateInfo(int count) =>
            _valueTMP.text = $"{count}";
    }
}