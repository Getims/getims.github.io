using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Configs.Combo;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Data;
using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Gameplay.Levels.Elements;
using Project.Scripts.Gameplay.Levels.Grid;
using Project.Scripts.Gameplay.Puzzle;
using Project.Scripts.Gameplay.Puzzle.Data;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.Runtime.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Levels
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField]
        private ElementsPool _elementsPool;

        [SerializeField]
        private GridPool _gridPool;

        [SerializeField]
        private LevelResult _levelResult;

        [Inject] private ILevelsDataService _levelsDataService;
        [Inject] private GameplayEventProvider _gameplayEventProvider;
        [Inject] private ICurrencyDataService _currencyDataService;
        [Inject] private ISoundService _soundService;

        private LevelConfig _levelConfig;
        private LevelsConfigProvider _levelsConfigProvider;
        private PuzzleGenerator _puzzleGenerator;
        private PuzzleGroupManager _groupManager;
        private PuzzleGrid _puzzleGrid;
        private List<LevelElement> _createdElements;
        private ElementsFactory _elementsFactory;
        private GridVisualizer _gridVisualizer;
        private ComboCounter _comboCounter;
        private HintSearcher _hintSearcher;

        public event Action OnLevelCompleteEvent;

        [Inject]
        public void Construct(IConfigsProvider configsProvider, GameplayEventProvider gameplayEventProvider)
        {
            _levelsConfigProvider = configsProvider.GetConfig<LevelsConfigProvider>();
            var comboConfigsProvider = configsProvider.GetConfig<ComboConfigsProvider>();
            _comboCounter = new ComboCounter(comboConfigsProvider, gameplayEventProvider);

            _puzzleGenerator = new PuzzleGenerator();
        }

        public void Initialize()
        {
            _elementsFactory = new ElementsFactory();
            _elementsFactory.Initialize(_elementsPool);

            _gridVisualizer = new GridVisualizer();
            _gridVisualizer.Initialize(_gridPool);
        }

        public void GenerateLevel()
        {
            int currentLevel = _levelsDataService.CurrentLevel.Value;
            _levelConfig = _levelsConfigProvider.GetLevel(currentLevel);
            _levelResult.Initialize(_levelConfig.Image);

            var puzzlePieces = _puzzleGenerator.GeneratePieces(_levelConfig);

            CreatePuzzleElements(puzzlePieces);

            ShuffleElements();
            CreateGrid();
            UpdateGroupsAndCheckCompletion();

            _hintSearcher = new HintSearcher(_createdElements, _groupManager, _puzzleGrid);
        }

        public void ClearLevel()
        {
            _elementsPool.Clear();
            _gridVisualizer.ClearGrid();
        }

        public void ShowResult()
        {
            _levelResult.Show();
        }

        private void Start()
        {
            _gameplayEventProvider.HintActivateRequest.AddListener(ShowHint);
        }

        private void OnDestroy()
        {
            _gameplayEventProvider.HintActivateRequest.RemoveListener(ShowHint);
        }

        private void CreatePuzzleElements(List<PuzzlePieceData> puzzlePieces)
        {
            _createdElements = _elementsFactory.CreateElements(puzzlePieces, _levelConfig, out _puzzleGrid);
            _groupManager = new PuzzleGroupManager(_puzzleGrid, _createdElements);

            for (int i = 0; i < _createdElements.Count; i++)
                _createdElements[i]
                    .Initialize(puzzlePieces[i], _puzzleGrid, _groupManager, OnElementDropped, OnBeginDrag);
        }

        private void ShuffleElements()
        {
            var shuffler = new PuzzleShuffler();
            shuffler.Shuffle(_createdElements, _puzzleGrid, _levelConfig.LevelWidth, _levelConfig.LevelHeight);
        }

        private void CreateGrid()
        {
            _gridVisualizer.CreateGrid(_createdElements);
        }

        private void UpdateGroupsAndCheckCompletion()
        {
            int groupsCount = _groupManager.GetTotalGroups();
            var updatedGroups = _groupManager.RebuildGroups();

            foreach (var element in _createdElements)
                element.UpdateValidationState();

            int newGroupsCount = _groupManager.GetTotalGroups();

            if (groupsCount != 0 && newGroupsCount != 1 && newGroupsCount < groupsCount)
                _soundService.PlayMergeSound();

            foreach (var updatedGroup in updatedGroups)
                AnimateGroupMerge(updatedGroup);

            _comboCounter.CheckCombo(newGroupsCount, groupsCount);

            if (newGroupsCount == 1)
                OnLevelComplete();
        }

        private void AnimateGroupMerge(PuzzleGroup group)
        {
            if (group.Elements.Count <= 1)
                return;

            var element = group.Elements.First();
            element.BounceScaleElementGroup();
        }

        private void ShowHint()
        {
            OnBeginDrag();

            if (_hintSearcher.TryFindHint(out var hintedElements))
            {
                foreach (var element in hintedElements)
                {
                    var group = _groupManager.GetGroupFor(element);
                    foreach (var groupElement in group.Elements)
                    {
                        groupElement.SetHint(true);
                    }
                }

                _currencyDataService.HintsCount.Spend(1);
            }
        }

        private void OnElementDropped()
        {
            _soundService.PlayDropSound();
            UpdateGroupsAndCheckCompletion();
        }

        private void OnLevelComplete()
        {
            OnBeginDrag();
            OnLevelCompleteEvent?.Invoke();
        }

        private void OnBeginDrag()
        {
            foreach (var element in _createdElements)
                element.SetHint(false);
        }
    }
}