using System.Collections.Generic;
using Project.Scripts.Configs.Gameplay;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Gameplay.Units.Data
{
    public class UnitInfo
    {
        private Dictionary<UnitStat, int> _stats = new Dictionary<UnitStat, int>();
        private IUnit _target;
        private string _name;
        private float _size;

        public IUnit Target => _target;
        public string Name => _name;
        public float Size => _size;

        public void AddStat(UnitStat unitStat, int value)
        {
            if (_stats.TryGetValue(unitStat, out var stat))
                _stats[unitStat] += value;
            else
                _stats.Add(unitStat, value);
        }

        public void AddStats(IReadOnlyCollection<StatConfig> statConfigs)
        {
            foreach (var statConfig in statConfigs)
                AddStat(statConfig.UnitStat, statConfig.Value);
        }

        public int GetStat(UnitStat unitStat)
        {
            if (_stats.TryGetValue(unitStat, out var stat))
                return stat;
            else
                return 0;
        }

        public void SetTarget(IUnit target)
        {
            _target = target;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public void SetSize(float size)
        {
            _size = size;
        }
    }
}