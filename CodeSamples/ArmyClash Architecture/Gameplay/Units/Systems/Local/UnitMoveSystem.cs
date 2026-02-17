using System;
using Project.Scripts.Gameplay.Units.Data;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Units.Systems.Local
{
    [Serializable]
    public class UnitMoveSystem : IUnitMoveSystem
    {
        [SerializeField]
        private Transform _unitTransform;

        private UnitInfo _unitInfo;
        private float _speed;

        public Transform Transform => _unitTransform;

        public void Initialize(UnitInfo unitInfo, float speedPointValue)
        {
            _unitInfo = unitInfo;
            _speed = speedPointValue * _unitInfo.GetStat(UnitStat.Speed);
        }

        public void MoveTowards(float deltaTime)
        {
            var currentPosition = _unitTransform.position;
            var targetPosition = _unitInfo.Target.Position;
            var direction = (targetPosition - currentPosition).normalized;

            _unitTransform.position = Vector3.MoveTowards(currentPosition, targetPosition, _speed * deltaTime);
            _unitTransform.rotation = Quaternion.LookRotation(direction);
        }

        public void StopMoving()
        {
            // For this controller, movement is handled each frame, so no special stop action is needed.
        }
    }
}