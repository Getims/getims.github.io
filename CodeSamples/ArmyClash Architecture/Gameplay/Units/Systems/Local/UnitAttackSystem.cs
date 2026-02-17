using System;
using Project.Scripts.Gameplay.Units.Data;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Units.Systems.Local
{
    [Serializable]
    public class UnitAttackSystem
    {
        private float _attackDelay = 0;
        private float _lastAttackTime = 0;
        private UnitInfo _unitInfo;

        public event Action OnAttack;

        public UnitAttackSystem(UnitInfo unitInfo, float attackSpeedPointValue)
        {
            _unitInfo = unitInfo;
            _attackDelay = unitInfo.GetStat(UnitStat.AttackSpeed) * attackSpeedPointValue;
        }

        public void Attack()
        {
            float currentTime = Time.time;
            if (currentTime < _lastAttackTime + _attackDelay)
                return;

            _lastAttackTime = currentTime;
            var target = _unitInfo.Target;
            if (target == null)
                return;

            target.Hit(_unitInfo.GetStat(UnitStat.Attack));
            OnAttack?.Invoke();

            if (target.IsAlive == false)
                _unitInfo.SetTarget(null);
        }

        public bool ReachAttackDistance(Vector3 currentPosition)
        {
            float distance = Vector3.Distance(currentPosition, _unitInfo.Target.Position);
            float attackRange = (_unitInfo.Size + _unitInfo.Target.UnitInfo.Size) * 0.55f;

            return distance <= attackRange;
        }
    }
}