using Lean.Pool;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Gameplay;
using Project.Scripts.Gameplay.Units;
using Project.Scripts.Infrastructure.Utilities;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Factory.Logic
{
    public interface IUnitsFactory
    {
        public IUnit GetUnit(UnitTeam team);
        public IUnit GetUnit(Vector3 position);
        void ReturnUnit(IUnit unit);
    }

    public class UnitsFactory : IUnitsFactory
    {
        private readonly LeanGameObjectPool _unitsPool;
        private readonly SpawnZoneService _spawnZoneService;
        private readonly GlobalConfig _globalConfig;
        private readonly UnitsConfigProvider _unitsConfigProvider;
        private readonly Transform _unitsContainer;

        public UnitsFactory(LeanGameObjectPool unitsPool, SpawnZoneService spawnZoneService, GlobalConfig globalConfig,
            UnitsConfigProvider unitsConfigProvider, Transform unitsContainer)
        {
            _unitsContainer = unitsContainer;
            _unitsConfigProvider = unitsConfigProvider;
            _globalConfig = globalConfig;
            _spawnZoneService = spawnZoneService;
            _unitsPool = unitsPool;
        }

        public IUnit GetUnit(UnitTeam unitTeam)
        {
            var zone = _spawnZoneService.GetOrCreateSpawnZone(unitTeam);

            return GetUnit(zone.GetRandomPointInZone());
        }

        public IUnit GetUnit(Vector3 position)
        {
            GameObject unitInstance = GetUnitFromPool(position);
            var unit = unitInstance.GetComponent<Unit>();
            InitializeUnit(unit);

            return unit;
        }

        public void ReturnUnit(IUnit unit)
        {
            _unitsPool.Despawn(unit.GameObject);
        }

        private void InitializeUnit(Unit unit)
        {
            var unitConfig = _unitsConfigProvider.BaseConfig;
            var shapeConfig = _unitsConfigProvider.Shapes.GetRandomElement();
            var sizeConfig = _unitsConfigProvider.Sizes.GetRandomElement();
            var colorConfig = _unitsConfigProvider.Colors.GetRandomElement();

            string name = $"Unit {shapeConfig.name} {sizeConfig.name} {colorConfig.name}";
            unit.Initialize(unitConfig, shapeConfig, sizeConfig, colorConfig, _globalConfig, name);
        }

        private GameObject GetUnitFromPool(Vector3 position)
        {
            return _unitsPool.Spawn(position, Quaternion.identity, _unitsContainer);
        }
    }
}