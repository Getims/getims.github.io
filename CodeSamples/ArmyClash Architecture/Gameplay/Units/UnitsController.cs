using Project.Scripts.Configs;
using Project.Scripts.Gameplay.Factory.Logic;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Gameplay.Units.Systems.Global;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Units
{
    public class UnitsController : MonoBehaviour
    {
        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private IGameInfoService _gameInfoService;

        private UnitsSystem _unitsSystem;

        public void Initialize(IUnitsFactory unitsFactory)
        {
            var globalConfig = _configsProvider.GetConfig<GlobalConfig>();
            _unitsSystem = new UnitsSystem(unitsFactory, globalConfig.UnitsCountPerTeam, _gameInfoService);
        }

        public void CreateUnits()
        {
            if (_unitsSystem != null)
                _unitsSystem.CreateUnits();
        }

        public void StartBattle()
        {
            if (_unitsSystem != null)
                _unitsSystem.StartBattle();
        }

        public void StopBattle()
        {
            if (_unitsSystem != null)
                _unitsSystem.StopBattle();
        }

        private void Update()
        {
            if (_unitsSystem != null)
                _unitsSystem.Update();
        }
    }
}