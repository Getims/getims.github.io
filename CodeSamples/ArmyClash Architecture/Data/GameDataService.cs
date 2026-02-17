using System.Collections.Generic;
using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Data.Experimental;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Data
{
    public interface IGameDataService
    {
        public IReadOnlyCollection<TeamData> TeamsInfo { get; }
        public void AddTeamWin(UnitTeam team);
        public DataValue<GameData, int> BattlesCount { get; }
    }

    public class GameDataService : ADataService<GameData>, IGameDataService
    {
        public IReadOnlyCollection<TeamData> TeamsInfo => _data.TeamsInfo;
        public DataValue<GameData, int> BattlesCount { get; }

        public GameDataService(IDatabase database) : base(database)
        {
            BattlesCount = CreateValue(
                data => data.BattlesCount,
                (data, value) => data.BattlesCount = value);
        }

        public void AddTeamWin(UnitTeam team)
        {
            TeamData teamData = null;
            foreach (var data in _data.TeamsInfo)
            {
                if (data.UnitTeam == team)
                    teamData = data;
            }

            if (teamData == null)
            {
                teamData = new TeamData(team);
                _data.TeamsInfo.Add(teamData);
            }

            teamData.WinsCount += 1;

            TryToSave(true);
        }
    }
}