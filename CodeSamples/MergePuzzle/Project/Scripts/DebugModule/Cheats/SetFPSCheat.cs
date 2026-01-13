using System;
using Project.Scripts.DebugModule.Core;
using UnityEngine;

namespace Project.Scripts.DebugModule.Cheats
{
    public class SetFPSCheat : ICheat<int>
    {
        public string Name => "Set FPS";
        public CheatGroupType GroupType => CheatGroupType.Base;

        public void Execute(int value)
        {
            value = Mathf.Clamp(value, 1, 120);
            Application.targetFrameRate = value;
        }

        public void Execute()
        {
            Execute(60);
        }

        public void Initialize(Action restartScene)
        {
        }
    }
}