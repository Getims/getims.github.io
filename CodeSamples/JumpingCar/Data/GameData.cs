using System;
using System.Collections.Generic;

namespace Project.Scripts.Data
{
    [Serializable]
    public class GameData : Infrastructure.Data.GameData
    {
        public List<int> GameScores = new();
    }
}