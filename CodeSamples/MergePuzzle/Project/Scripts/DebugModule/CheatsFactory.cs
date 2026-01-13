using System.Collections.Generic;
using System.Linq;
using Project.Scripts.DebugModule.Core;
using Project.Scripts.DebugModule.Elements;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.DebugModule
{
    public class CheatsFactory : MonoBehaviour
    {
        [SerializeField, Required]
        private ACheatConsole _cheatConsole;

        [SerializeField]
        private InputCheatFactory _inputCheatFactory;

        [SerializeField]
        private ButtonCheatFactory _buttonCheatFactory;

        [SerializeField]
        private SliderCheatFactory _sliderCheatFactory;

        [SerializeField]
        private CheatGroupSeparator _groupSeparatorPrefab;

        [SerializeField]
        private GameObject _emptySeparatorPrefab;

        [SerializeField]
        private CheatGroupBlock _groupBlockPrefab;

        [SerializeField]
        private Transform _cheatsContainer;

        private List<ICheatElementFactory> _factories = new List<ICheatElementFactory>();
        private List<ICheat> _cheats = new List<ICheat>();
        private Dictionary<CheatGroupType, CheatGroupBlock> _groupBlocks = new();

        private void Start()
        {
            CreateFactories();
            CreateCheats();
            CreateElements();
        }

        private void CreateCheats()
        {
            _cheats = _cheatConsole.CreateCheats();
        }

        private void CreateFactories()
        {
            _factories.Add(_buttonCheatFactory);
            _factories.Add(_inputCheatFactory);
            _factories.Add(_sliderCheatFactory);
        }

        private void CreateElements()
        {
            CheatGroupType lastGroup = CheatGroupType.Base;

            foreach (var cheat in _cheats)
            {
                var factory = _factories.FirstOrDefault(f => f.CanHandle(cheat));
                if (factory == null)
                {
                    Debug.LogWarning($"Нет подходящей фабрики для чита {cheat.GetType().Name}");
                    continue;
                }

                if (cheat.GroupType != lastGroup)
                {
                    if (cheat.GroupType != CheatGroupType.Base)
                        CreateGroupSeparator(cheat.GroupType);
                    else
                        CreateEmptySeparator();
                }

                lastGroup = cheat.GroupType;

                factory.Create(cheat, _cheatsContainer);
            }
        }

        private void CreateElements0()
        {
            foreach (var cheat in _cheats)
            {
                var groupType = cheat.GroupType;
                if (!_groupBlocks.ContainsKey(groupType))
                {
                    var block = Instantiate(_groupBlockPrefab, _cheatsContainer);
                    block.Initialize(groupType.ToString());
                    _groupBlocks[groupType] = block;
                }

                var factory = _factories.FirstOrDefault(f => f.CanHandle(cheat));
                if (factory == null) continue;

                var parent = _groupBlocks[groupType].GetCheatsContainer();
                factory.Create(cheat, parent);
            }
        }


        private void CreateGroupSeparator(CheatGroupType groupName)
        {
            var element = Instantiate(_groupSeparatorPrefab, _cheatsContainer);
            element.Initialize(groupName);
        }

        private void CreateEmptySeparator()
        {
            Instantiate(_emptySeparatorPrefab, _cheatsContainer);
        }
    }
}