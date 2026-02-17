using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Gameplay
{
    [ConfigCategory(ConfigCategory.Game)]
    public class UnitConfig : ScriptableConfig
    {
        [SerializeField, LabelText("HP"), MinValue(0)]
        private int _health = 100;

        [SerializeField, LabelText("ATK"), MinValue(0)]
        private int _attack = 10;

        [SerializeField, LabelText("SPEED"), MinValue(0)]
        private int _speed = 10;

        [SerializeField, LabelText("ATKSPD"), MinValue(0)]
        private int _attackSpeed = 1;

        public List<StatConfig> GetAllStats()
        {
            return new List<StatConfig>
            {
                new StatConfig(UnitStat.HP, _health),
                new StatConfig(UnitStat.Attack, _attack),
                new StatConfig(UnitStat.Speed, _speed),
                new StatConfig(UnitStat.AttackSpeed, _attackSpeed)
            };
        }

        public int GetStat(UnitStat unitStat)
        {
            switch (unitStat)
            {
                case UnitStat.HP:
                    return _health;
                case UnitStat.Attack:
                    return _attack;
                case UnitStat.Speed:
                    return _speed;
                case UnitStat.AttackSpeed:
                    return _attackSpeed;
                default:
                    return 0;
            }
        }
    }
}