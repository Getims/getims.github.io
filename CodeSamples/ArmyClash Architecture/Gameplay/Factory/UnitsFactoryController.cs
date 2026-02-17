using System.Collections.Generic;
using Lean.Pool;
using Project.Scripts.Configs;
using Project.Scripts.Configs.Gameplay;
using Project.Scripts.Gameplay.Factory.Data;
using Project.Scripts.Gameplay.Factory.Logic;
using Project.Scripts.Gameplay.Units;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Enums;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Factory
{
    public class UnitsFactoryController : MonoBehaviour, IUnitsFactory
    {
        [SerializeField]
        private LeanGameObjectPool _unitsPool;

        [SerializeField]
        private Transform _unitsContainer;

        [SerializeField]
        private List<SpawnZone> _spawnZones = new List<SpawnZone>();

        [Inject] private IConfigsProvider _configsProvider;

        private UnitsFactory _unitsFactory;
        private SpawnZoneService _spawnZoneService;

        public void Initialize()
        {
            var unitsConfigProvider = _configsProvider.GetConfig<UnitsConfigProvider>();
            var globalConfig = _configsProvider.GetConfig<GlobalConfig>();

            _spawnZoneService = new SpawnZoneService(_spawnZones, globalConfig, transform);
            _unitsFactory = new UnitsFactory(_unitsPool, _spawnZoneService, globalConfig, unitsConfigProvider,
                _unitsContainer);
        }

        public IUnit GetUnit(UnitTeam team) => _unitsFactory.GetUnit(team);
        public IUnit GetUnit(Vector3 position) => _unitsFactory.GetUnit(position);
        public void ReturnUnit(IUnit unit) => _unitsFactory.ReturnUnit(unit);
    }
}