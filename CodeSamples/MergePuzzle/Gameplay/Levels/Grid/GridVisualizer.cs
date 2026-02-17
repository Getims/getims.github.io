using System.Collections.Generic;
using Project.Scripts.Gameplay.Levels.Elements;
using UnityEngine;

namespace Project.Scripts.Gameplay.Levels.Grid
{
    public class GridVisualizer
    {
        private GridPool _gridPool;

        public void Initialize(GridPool gridPool)
        {
            _gridPool = gridPool;
        }

        public void CreateGrid(List<LevelElement> elements)
        {
            foreach (var element in elements)
            {
                var position = element.transform.localPosition;
                var gridCell = _gridPool.GetElement(position);
                var rectTransform = gridCell.GetComponent<RectTransform>();
                rectTransform.sizeDelta = element.GetComponent<RectTransform>().sizeDelta;
            }
        }

        public void ClearGrid()
        {
            _gridPool.Clear();
        }
    }
}