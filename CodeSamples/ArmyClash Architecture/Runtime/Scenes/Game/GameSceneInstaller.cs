using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.UI.Game;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Runtime.Scenes.Game
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private GameFlowController _gameFlowController;

        [SerializeField]
        private GameUIController _gameUIController;

        public override void InstallBindings()
        {
            BindSceneObjects();
            BindGameStateMachine();
        }

        public override void Start()
        {
            base.Start();

            GameSceneCore gameSceneCore = Container.Instantiate<GameSceneCore>();
            gameSceneCore.Initialize();
        }

        private void BindSceneObjects()
        {
            Container.Bind<IGameFlow>().FromInstance(_gameFlowController).AsSingle().NonLazy();
            Container.Bind<IGameUIController>().FromInstance(_gameUIController).AsSingle().NonLazy();

            Container.BindInterfacesTo<GameInfoService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<GameResultService>().AsSingle().NonLazy();
        }

        private void BindGameStateMachine()
        {
            Container.Bind<StateMachineFactory>().AsSingle().NonLazy();
            Container.Bind<GameStateMachine>().AsSingle().NonLazy();
        }
    }
}