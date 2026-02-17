using Project.Scripts.Configs;
using Project.Scripts.Configs.Gameplay;
using Project.Scripts.Gameplay.Units.Data;
using Project.Scripts.Gameplay.Units.Systems.Local;
using Project.Scripts.Gameplay.Units.Visual;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Gameplay.Units
{
    public interface IUnit
    {
        GameObject GameObject { get; }
        UnitInfo UnitInfo { get; }
        bool IsAlive { get; }
        bool HasTarget { get; }
        Vector3 Position { get; }

        void SetTarget(IUnit target);
        bool CanAttack();
        void Attack();
        void MoveTowards(float deltaTime);
        void Hit(int damage);
        void StopMoving();
    }

    public class Unit : MonoBehaviour, IUnit
    {
        [SerializeField]
        private UnitVisual _unitVisual;

        [SerializeField]
        private UnitMoveSystemAI _unitMoveSystem;

        private UnitHealthSystem _unitHealthSystem;
        private UnitAttackSystem _unitAttackSystem;
        private UnitInfo _unitInfo;

        public GameObject GameObject => gameObject;
        public UnitInfo UnitInfo => _unitInfo;
        public bool IsAlive => _unitHealthSystem.IsAlive;
        public bool HasTarget => _unitInfo.Target != null;
        public Vector3 Position => _unitMoveSystem.Transform.position;

        [Button]
        public void Initialize(UnitConfig unitConfig, ShapeConfig shapeConfig, SizeConfig sizeConfig,
            ColorConfig colorConfig, GlobalConfig globalConfig, string name)
        {
            gameObject.name = name;

            _unitVisual.SetMesh(shapeConfig.ShapeMesh, shapeConfig.ShapeBaseScale);
            _unitVisual.SetColor(colorConfig.Color);
            _unitVisual.SetModelSize(sizeConfig.ModelSize);

            _unitInfo = new UnitInfo();
            _unitInfo.SetName(name);
            _unitInfo.AddStats(unitConfig.GetAllStats());
            _unitInfo.AddStats(shapeConfig.StatConfigs);
            _unitInfo.AddStats(sizeConfig.StatConfigs);
            _unitInfo.AddStats(colorConfig.StatConfigs);
            _unitInfo.SetSize(sizeConfig.ModelSize);

            _unitHealthSystem = new UnitHealthSystem(_unitInfo);
            _unitHealthSystem.OnHealthChanged += OnUnitHealthChanged;
            OnUnitHealthChanged();

            _unitMoveSystem.Initialize(_unitInfo, globalConfig.SpeedPointValue);
            _unitAttackSystem = new UnitAttackSystem(_unitInfo, globalConfig.AttackSpeedPointValue);
        }

        public void Hit(int damage) => _unitHealthSystem.Hit(damage);

        public void StopMoving() => _unitMoveSystem.StopMoving();

        public void SetTarget(IUnit target) => _unitInfo.SetTarget(target);

        public bool CanAttack() => _unitInfo.Target != null && _unitAttackSystem.ReachAttackDistance(Position);

        public void Attack() => _unitAttackSystem.Attack();

        public void MoveTowards(float deltaTime) => _unitMoveSystem.MoveTowards(deltaTime);

        private void OnUnitHealthChanged()
        {
            _unitVisual.UpdateHealth(_unitHealthSystem.Health);
        }
    }
}