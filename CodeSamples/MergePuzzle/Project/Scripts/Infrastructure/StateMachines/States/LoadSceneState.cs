using System;
using Project.Scripts.Core.Enums;
using Project.Scripts.Infrastructure.ScenesManager;

namespace Project.Scripts.Infrastructure.StateMachines.States
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