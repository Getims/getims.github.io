using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using UnityEngine;

namespace Project.Scripts.Configs.Gameplay
{
    [ConfigCategory(ConfigCategory.Game)]
    public class UnitsConfigProvider : ScriptableConfig
    {
        [SerializeField]
        private UnitConfig _baseConfig;

        [SerializeField]
        private List<ShapeConfig> _shapes = new List<ShapeConfig>();

        [SerializeField]
        private List<SizeConfig> _sizes = new List<SizeConfig>();

        [SerializeField]
        private List<ColorConfig> _colors = new List<ColorConfig>();

        public UnitConfig BaseConfig => _baseConfig;
        public IReadOnlyCollection<ShapeConfig> Shapes => _shapes;
        public IReadOnlyCollection<SizeConfig> Sizes => _sizes;
        public IReadOnlyCollection<ColorConfig> Colors => _colors;
    }
}