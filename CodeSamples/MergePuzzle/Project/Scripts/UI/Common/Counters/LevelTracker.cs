using Project.Scripts.Core.Constants;
using Project.Scripts.Core.Events;
using Project.Scripts.Data;
using TMPro;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Common.Counters
{
    public class LevelTracker : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _valueTMP;

        [Inject] private GameplayEventProvider _gameplayEventProvider;
        [Inject] private ILevelsDataService _levelsDataService;

        private void Start()
        {
            _gameplayEventProvider.LevelSwitchEvent.AddListener(UpdateInfo);
            UpdateInfo();
        }

        private void OnDestroy()
        {
            _gameplayEventProvider?.LevelSwitchEvent.RemoveListener(UpdateInfo);
        }

        private void UpdateInfo()
        {
            UpdateInfo(_levelsDataService.CurrentLevelUI);
        }

        private void UpdateInfo(int levelNumber) =>
            _valueTMP.text = $"{LocalizationKeys.LEVEL} {levelNumber}";
    }
}