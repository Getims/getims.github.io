using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Cars
{
    [ConfigCategory(ConfigCategory.Game)]
    public class CarConfig : ScriptableConfig
    {
        [SerializeField]
        private Color _color = Color.white;

        [SerializeField, MinValue(0)]
        private float _speed = 1f;

        public Color Color => _color;
        public float Speed => _speed;
    }
}