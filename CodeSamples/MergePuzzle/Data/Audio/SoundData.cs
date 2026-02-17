using System;
using Project.Scripts.Infrastructure.Data;

namespace Project.Scripts.Data.Audio
{
    [Serializable]
    public class SoundData : GameData
    {
        public bool IsSoundOn = true;
        public bool IsMusicOn = true;
    }
}