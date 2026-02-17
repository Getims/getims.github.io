using System;
using Project.Scripts.Gameplay.Units.Data;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Gameplay.Units.Systems.Local
{
    [Serializable]
    public class UnitHealthSystem
    {
        private int _health;

        public bool IsAlive => _health > 0;
        public int Health => _health;
        public event Action OnHealthChanged;

        public UnitHealthSystem(UnitInfo unitInfo)
        {
            _health = unitInfo.GetStat(UnitStat.HP);
            if (_health == 0)
                UnityEngine.Debug.LogWarning($"Dead on start {unitInfo.Name}");
        }

        public void Hit(int damage)
        {
            if (_health == 0)
                return;

            _health -= damage;
            if (_health <= 0)
                _health = 0;
            OnHealthChanged?.Invoke();
        }
    }
}