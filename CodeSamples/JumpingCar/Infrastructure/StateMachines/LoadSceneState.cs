using System;
using Project.Scripts.Infrastructure.ScenesManager;
using Project.Scripts.Runtime.Enums;

namespace Project.Scripts.Infrastructure.StateMachines
{
    public abstract class LoadSceneState
    {
        private readonly ISceneLoader _sceneLoader;

        protected LoadSceneState(ISceneLoader sceneLoader) =>
            _sceneLoader = sceneLoader;

        protected void Enter(string scene, Action onLoaded) =>
            _sceneLoader.Load(scene, onLoaded);

        protected void Enter(Scenes scene, Action onLoaded) =>
            _sceneLoader.Load(scene, onLoaded);
    }
}