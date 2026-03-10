using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Cars
{
    [ConfigCategory(ConfigCategory.Game)]
    public class CarsConfigProvider : ScriptableConfig
    {
        [SerializeField]
        private PlayerConfig _playerConfig;

        [SerializeField, ListDrawerSettings(Expanded = false, ListElementLabelName = @"RuleName")]
        private List<EnemySpawnRule> _enemySpawnRules = new List<EnemySpawnRule>();

        [SerializeField, MinValue(1)]
        private int _createEnemyFromPlatFormNumber = 1;

        public PlayerConfig PlayerConfig => _playerConfig;
        public int CreateEnemyFromPlatFormNumber => _createEnemyFromPlatFormNumber;

        public CarConfig GetEnemyCar(int platformNumber)
        {
            foreach (var enemySpawnRule in _enemySpawnRules)
            {
                if (enemySpawnRule.HasPlatformNumber(platformNumber))
                    return enemySpawnRule.GetRandomCar();
            }

            return _enemySpawnRules[0].GetRandomCar();
        }
    }
}