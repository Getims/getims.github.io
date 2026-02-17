using Project.Scripts.Gameplay.Units.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Units.Systems.Local
{
    public interface IUnitMoveSystem
    {
        Transform Transform { get; }
        void Initialize(UnitInfo unitInfo, float speedPointValue);
        void MoveTowards(float deltaTime);
        void StopMoving();
    }
}