using System;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Configs.Gameplay
{
    [Serializable]
    public class StatConfig
    {
        [SerializeField]
        private UnitStat _unitStat;

        [SerializeField]
        private int _value = 0;

        public UnitStat UnitStat => _unitStat;
        public int Value => _value;

        public StatConfig(UnitStat unitStat)
        {
            _unitStat = unitStat;
        }

        public StatConfig(UnitStat unitStat, int value)
        {
            _unitStat = unitStat;
            _value = value;
        }
    }
}