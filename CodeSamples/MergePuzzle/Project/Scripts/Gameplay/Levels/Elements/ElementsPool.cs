using System.Collections.Generic;
using Lean.Pool;
using Project.Scripts.Configs;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Levels.Elements
{
    public class ElementsPool : MonoBehaviour
    {
        [SerializeField]
        private LeanGameObjectPool _pool;

        [SerializeField]
        private RectTransform _container;

        private List<LevelElement> _elements = new List<LevelElement>();
        private PuzzleConfig _puzzleConfig;

        public RectTransform Container => _container;

        [Inject]
        public void Construct(IConfigsProvider configsProvider)
        {
            _puzzleConfig = configsProvider.GetConfig<PuzzleConfig>();
        }

        public LevelElement GetElement(Vector3 localPosition)
        {
            GameObject instance = _pool.Spawn(localPosition, Quaternion.identity, _container);
            var levelElement = instance.GetComponent<LevelElement>();
            levelElement.SetupConfig(_puzzleConfig);
            levelElement.SetPosition(localPosition);

            _elements.Add(levelElement);
            return levelElement;
        }

        public void Clear()
        {
            foreach (LevelElement effect in _elements)
                _pool.Despawn(effect.gameObject);

            _elements.Clear();
        }
    }
}