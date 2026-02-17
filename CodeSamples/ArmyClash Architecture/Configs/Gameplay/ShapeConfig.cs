using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Gameplay
{
    [ConfigCategory(ConfigCategory.Game)]
    public class ShapeConfig : ScriptableConfig
    {
        [SerializeField]
        private List<StatConfig> _statConfigs = new List<StatConfig>();

        [Title("Visual")]
        [SerializeField]
        private Mesh _shapeMesh;

        [SerializeField]
        private Vector3 _shapeBaseScale = Vector3.one;

        public Mesh ShapeMesh => _shapeMesh;
        public Vector3 ShapeBaseScale => _shapeBaseScale;
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