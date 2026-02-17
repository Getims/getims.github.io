using System.Collections.Generic;
using Project.Scripts.Gameplay.Factory.Logic;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Gameplay.Units.Systems.Global
{
    public class UnitsSystem
    {
        private readonly IGameInfoService _gameInfoService;
        private readonly IUnitsFactory _unitsFactory;
        private readonly BattleSystem _battleSystem;
        private readonly TargetSystem _targetSystem;
        private readonly MovementSystem _movementSystem;

        private readonly Dictionary<UnitTeam, List<IUnit>> _unitsDictionary;
        private readonly int _unitsCountPerTeam;

        private State _state;

        public UnitsSystem(IUnitsFactory unitsFactory, int unitsCountPerTeam, IGameInfoService gameInfoService)
        {
            _gameInfoService = gameInfoService;
            _unitsCountPerTeam = unitsCountPerTeam;
            _unitsFactory = unitsFactory;

            _unitsDictionary = new Dictionary<UnitTeam, List<IUnit>>();
            _state = State.Initialization;

            _battleSystem = new BattleSystem();
            _targetSystem = new TargetSystem();
            _movementSystem = new MovementSystem();
        }

        public void CreateUnits()
        {
            ClearUnits();
            CreateUnits(UnitTeam.Team1, _unitsCountPerTeam);
            CreateUnits(UnitTeam.Team2, _unitsCountPerTeam);
        }

        public void StartBattle()
        {
            _state = State.Battle;
        }

        public void StopBattle()
        {
            foreach (var kvp in _unitsDictionary)
            {
                foreach (var unit in kvp.Value)
                    unit.StopMoving();
            }

            _state = State.Stop;
        }

        public void Update()
        {
            if (_state != State.Battle)
                return;

            foreach (var kvp in _unitsDictionary)
            {
                UpdateUnitsTeam(kvp.Key, kvp.Value);
                _gameInfoService.UpdateTeamInfo(kvp.Key, kvp.Value.Count);
            }
        }

        private void CreateUnits(UnitTeam unitTeam, int unitsCountPerTeam)
        {
            bool hasTeam = _unitsDictionary.TryGetValue(unitTeam, out var units);
            if (units == null)
                units = new List<IUnit>();

            for (int i = 0; i < unitsCountPerTeam; i++)
            {
                var unit = _unitsFactory.GetUnit(unitTeam);
                units.Add(unit);
            }

            if (hasTeam)
                _unitsDictionary[unitTeam] = units;
            else
                _unitsDictionary.Add(unitTeam, units);
        }

        private enum State
        {
            Initialization,
            Battle,
            Stop
        }

        private void ClearUnits()
        {
            foreach (var kvp in _unitsDictionary)
            {
                foreach (var unit in kvp.Value)
                    _unitsFactory.ReturnUnit(unit);
            }

            _unitsDictionary.Clear();
        }

        private void UpdateUnitsTeam(UnitTeam team, List<IUnit> units)
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                var unit = units[i];

                if (!unit.IsAlive)
                {
                    RemoveUnit(unit, team);
                    continue;
                }

                _targetSystem.Update(unit, team, _unitsDictionary);
                _movementSystem.Update(unit);
                _battleSystem.Update(unit);
            }
        }

        private void RemoveUnit(IUnit unit, UnitTeam team)
        {
            if (_unitsDictionary.TryGetValue(team, out var units))
                units.Remove(unit);

            _unitsFactory.ReturnUnit(unit);
        }
    }
}