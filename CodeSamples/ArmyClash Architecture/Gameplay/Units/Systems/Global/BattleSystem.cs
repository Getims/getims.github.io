namespace Project.Scripts.Gameplay.Units.Systems.Global
{
    public class BattleSystem
    {
        public void Update(IUnit unit)
        {
            if (!unit.IsAlive)
                return;

            if (!unit.HasTarget)
                return;

            if (!unit.CanAttack())
                return;

            unit.Attack();
        }
    }
}