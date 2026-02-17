using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace Project.Scripts.Gameplay.Levels.Grid
{
    public class GridPool : MonoBehaviour
    {
        [SerializeField]
        private LeanGameObjectPool _pool;

        [SerializeField]
        private RectTransform _container;

        private List<GridCell> _elements = new List<GridCell>();

        public GridCell GetElement(Vector3 localPosition)
        {
            GameObject instance = _pool.Spawn(localPosition, Quaternion.identity, _container);
            var gridCell = instance.GetComponent<GridCell>();
            gridCell.SetPosition(localPosition);

            _elements.Add(gridCell);
            return gridCell;
        }

        public void Clear()
        {
            foreach (GridCell element in _elements)
                _pool.Despawn(element.gameObject);

            _elements.Clear();
        }
    }
}