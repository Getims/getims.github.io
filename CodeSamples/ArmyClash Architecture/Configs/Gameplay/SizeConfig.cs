using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Gameplay
{
    [ConfigCategory(ConfigCategory.Game)]
    public class SizeConfig : ScriptableConfig
    {
        [SerializeField, MinValue(0)]
        private float _modelSize = 1;

        [SerializeField]
        private List<StatConfig> _statConfigs = new List<StatConfig>();

        public float ModelSize => _modelSize;
        public IReadOnlyCollection<StatConfig> StatConfigs => _statConfigs;

        public int GetStat(UnitStat unitStat)
        {
            var value = 0;
            foreach (var statConfig in _statConfigs)
            {
                if (statConfig.UnitStat == unitStat)
                    value += statConfig.Value;
            }

            return value;
        }
    }
}