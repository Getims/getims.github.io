using System;
using System.Collections.Generic;
using Project.Scripts.Infrastructure.Utilities;
using UnityEngine;

namespace Project.Scripts.Configs.Cars
{
    [Serializable]
    public class EnemySpawnRule
    {
        [SerializeField]
        private int _fromPlatform;

        [SerializeField]
        private int _toPlatform;

        [SerializeField]
        private List<CarConfig> _carConfigs = new List<CarConfig>();

        public string RuleName => $"from {_fromPlatform} to {_toPlatform} cars {_carConfigs.Count}";

        public bool HasPlatformNumber(int platformNumber)
        {
            if (platformNumber >= _fromPlatform && platformNumber <= _toPlatform)
                return true;

            return false;
        }

        public CarConfig GetRandomCar() => _carConfigs.GetRandomElement();
    }
}