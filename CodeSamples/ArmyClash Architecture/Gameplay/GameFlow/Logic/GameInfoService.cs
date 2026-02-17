using System;
using System.Collections.Generic;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Gameplay.GameFlow.Logic
{
    public interface IGameInfoService
    {
        void UpdateTeamInfo(UnitTeam unitTeam, int count);
        event Action OnOneTeamAlive;
        event Action OnUnitsCountUpdate;
        IReadOnlyDictionary<UnitTeam, int> UnitsDictionary { get; }
    }

    public class GameInfoService : IGameInfoService
    {
        private readonly Dictionary<UnitTeam, int> _unitsDictionary = new();

        public event Action OnOneTeamAlive;
        public event Action OnUnitsCountUpdate;
        public IReadOnlyDictionary<UnitTeam, int> UnitsDictionary => _unitsDictionary;

        public void UpdateTeamInfo(UnitTeam unitTeam, int count)
        {
            _unitsDictionary[unitTeam] = count;

            if (count == 0)
                CheckTeamsCount();

            OnUnitsCountUpdate?.Invoke();
        }

        private void CheckTeamsCount()
        {
            var aliveTeamsCount = 0;
            foreach (var count in _unitsDictionary.Values)
            {
                if (count > 0)
                    aliveTeamsCount++;
            }

            if (aliveTeamsCount == 1)
            {
                OnOneTeamAlive?.Invoke();
            }
        }
    }
}