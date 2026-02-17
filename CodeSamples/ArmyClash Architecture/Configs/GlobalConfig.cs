using System;
using Project.Scripts.Infrastructure.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs
{
    [Serializable]
    public class GlobalConfig : ScriptableConfig
    {
        [SerializeField, MinValue(1)]
        private int _unitsCountPerTeam = 20;

        [Title("Stats Balance")]
        [SerializeField]
        private float _speedPointValue = 1;

        [SerializeField]
        private float _attackSpeedPointValue = 1;

        public float SpeedPointValue => _speedPointValue;
        public float AttackSpeedPointValue => _attackSpeedPointValue;
        public int UnitsCountPerTeam => _unitsCountPerTeam;
    }
}