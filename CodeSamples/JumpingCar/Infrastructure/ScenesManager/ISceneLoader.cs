using System;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Infrastructure.ScenesManager
{
    public interface ISceneLoader
    {
        void Load(string name, Action onLoaded = null);
        void Load(Scenes scene, Action onLoaded = null);
    }
}