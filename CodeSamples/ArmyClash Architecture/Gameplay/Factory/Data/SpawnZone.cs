using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Factory.Utils;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Factory.Data
{
    [Serializable]
    public class SpawnZone
    {
        [SerializeField]
        private UnitTeam _unitTeam;

        [SerializeField]
        private Transform _zoneTransform;

        private int _pointsCount;
        private Queue<Vector3> _points;

        public UnitTeam UnitTeam => _unitTeam;
        public Transform ZoneTransform => _zoneTransform;

        public SpawnZone(UnitTeam unitTeam, Transform zoneTransform)
        {
            _unitTeam = unitTeam;
            _zoneTransform = zoneTransform;
        }

        public void Initialize(int pointsCount)
        {
            _points = new Queue<Vector3>();
            _pointsCount = pointsCount;
            CreatePoints();
        }

        public Vector3 GetRandomPointInZone()
        {
            if (_points.Count == 0)
                CreatePoints();

            return _points.Dequeue();
        }

        private void CreatePoints()
        {
            _points.Clear();

            Vector3 center = _zoneTransform.position;
            Vector3 size = _zoneTransform.localScale;

            int count = _pointsCount > 0 ? _pointsCount : 1;

            var pointsList = SpawnZoneHelper.DistributeSquares(center, size, count);
            Infrastructure.Utilities.Utils.Shuffle(pointsList);
            _points = new Queue<Vector3>(pointsList);
        }
    }
}