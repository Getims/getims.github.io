using Project.Scripts.Configs.Combo;
using Project.Scripts.Runtime.Events;
using UnityEngine;

namespace Project.Scripts.Gameplay.GameFlow
{
    public class ComboCounter
    {
        private readonly ComboConfigsProvider _comboConfigsProvider;
        private readonly GameplayEventProvider _gameplayEventProvider;
        private int _comboLevel;

        public ComboCounter(ComboConfigsProvider comboConfigsProvider, GameplayEventProvider gameplayEventProvider)
        {
            _comboConfigsProvider = comboConfigsProvider;
            _gameplayEventProvider = gameplayEventProvider;
        }

        public void CheckCombo(int newGroupsCount, int previousGroupsCount)
        {
            if (newGroupsCount > previousGroupsCount)
            {
                _comboLevel = -1;
                return;
            }

            if (newGroupsCount == previousGroupsCount)
            {
                _comboLevel = Mathf.Max(0, _comboLevel - 1);
                return;
            }

            if (_comboLevel >= 0)
            {
                var comboConfig = _comboConfigsProvider.GetComboConfig(_comboLevel);
                _gameplayEventProvider.ComboTriggeredEvent.Invoke(comboConfig);
            }

            _comboLevel++;
        }

        public void Reset() =>
            _comboLevel = 0;
    }
}