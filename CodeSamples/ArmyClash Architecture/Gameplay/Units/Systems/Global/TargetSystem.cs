using System.Collections.Generic;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Units.Systems.Global
{
    public class TargetSystem
    {
        public void Update(IUnit unit, UnitTeam team, Dictionary<UnitTeam, List<IUnit>> unitsDictionary)
        {
            if (!unit.IsAlive)
                return;

            if (unit.HasTarget)
                return;

            var enemyUnit = FindClosestEnemy(unit, team, unitsDictionary);
            unit.SetTarget(enemyUnit);
        }

        private IUnit FindClosestEnemy(IUnit unit, UnitTeam team, Dictionary<UnitTeam, List<IUnit>> unitsDictionary)
        {
            IUnit closest = null;
            float minDist = float.MaxValue;

            foreach (var kvp in unitsDictionary)
            {
                if (kvp.Key == team)
                    continue;

                foreach (var enemy in kvp.Value)
                {
                    if (!enemy.IsAlive)
                        continue;

                    float distance = Vector3.Distance(unit.Position, enemy.Position);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        closest = enemy;
                    }
                }
            }

            return closest;
        }
    }
}