using System.Collections.Generic;
using Project.Scripts.Configs;
using Project.Scripts.Gameplay.Factory.Data;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Factory.Logic
{
    public class SpawnZoneService
    {
        private readonly List<SpawnZone> _spawnZones;
        private readonly GlobalConfig _globalConfig;
        private readonly Transform _transform;

        public SpawnZoneService(List<SpawnZone> spawnZones, GlobalConfig globalConfig, Transform transform)
        {
            _transform = transform;
            _globalConfig = globalConfig;
            _spawnZones = spawnZones;

            foreach (SpawnZone spawnZone in _spawnZones)
                spawnZone.Initialize(_globalConfig.UnitsCountPerTeam);
        }

        public SpawnZone GetOrCreateSpawnZone(UnitTeam unitTeam)
        {
            SpawnZone zone = null;
            foreach (var spawnZone in _spawnZones)
            {
                if (spawnZone.UnitTeam == unitTeam && spawnZone.ZoneTransform != null)
                {
                    zone = spawnZone;
                    break;
                }
            }

            if (zone == null)
            {
                UnityEngine.Debug.LogWarning($"No zones for {unitTeam}! Creating a zone!");
                zone = new SpawnZone(unitTeam, _transform);
                zone.Initialize(_globalConfig.UnitsCountPerTeam);
                _spawnZones.Add(zone);
            }

            return zone;
        }
    }
}