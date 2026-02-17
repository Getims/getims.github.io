using System;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Data
{
    [Serializable]
    public class TeamData
    {
        public UnitTeam UnitTeam;
        public int WinsCount = 0;

        public TeamData(UnitTeam unitTeam)
        {
            UnitTeam = unitTeam;
        }
    }
}