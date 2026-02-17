using UnityEngine;

namespace Project.Scripts.Gameplay.Units.Systems.Global
{
    public class MovementSystem
    {
        public void Update(IUnit unit)
        {
            if (!unit.IsAlive)
                return;

            if (!unit.HasTarget || unit.CanAttack())
            {
                unit.StopMoving();
                return;
            }

            unit.MoveTowards(Time.deltaTime);
        }
    }
}