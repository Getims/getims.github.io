using System;
using Project.Scripts.Gameplay.Units.Data;
using Project.Scripts.Runtime.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Gameplay.Units.Systems.Local
{
    [Serializable]
    public class UnitMoveSystemAI : IUnitMoveSystem
    {
        [SerializeField]
        private Transform _unitTransform;

        [SerializeField]
        private NavMeshAgent _navMeshAgent;

        private UnitInfo _unitInfo;
        private float _speed;

        public Transform Transform => _unitTransform;

        public void Initialize(UnitInfo unitInfo, float speedPointValue)
        {
            _unitInfo = unitInfo;
            _speed = speedPointValue * _unitInfo.GetStat(UnitStat.Speed);
            _navMeshAgent.speed = _speed;
            _navMeshAgent.radius = _unitInfo.Size * 0.5f;
        }

        public void MoveTowards(float deltaTime)
        {
            var targetPosition = _unitInfo.Target.Position;
            _navMeshAgent.SetDestination(targetPosition);
        }

        public void StopMoving()
        {
            if (_navMeshAgent.hasPath)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.ResetPath();
            }
        }
    }
}