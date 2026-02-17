using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.Levels.Elements;
using Random = System.Random;

namespace Project.Scripts.Gameplay.Puzzle
{
    public class PuzzleShuffler
    {
        private List<LevelElement> _elements;
        private PuzzleGrid _grid;
        private int _width;
        private int _height;

        public void Shuffle(List<LevelElement> elements, PuzzleGrid grid, int width, int height)
        {
            _elements = elements;
            _grid = grid;
            _width = width;
            _height = height;

            FisherYatesShuffle();

            /*
            var rnd = new Random();
            int choice = rnd.Next(0, 7);
            Utils.Log(this, $"Shuffle combination {choice}");

            switch (choice)
            {
                case 0:
                    InvertVertically();
                    InvertHorizontally();
                    break;

                case 1:
                    InvertVertically();
                    ShuffleDiagonally();
                    break;

                case 2:
                    InvertHorizontally();
                    ShuffleByQuarters();
                    SwapCorners();
                    break;

                case 3:
                    ShuffleDiagonally();
                    ShuffleByQuarters();
                    SwapCorners();
                    break;
                case 4:
                    InvertVertically();
                    ShuffleByQuarters();
                    SwapCorners();
                    break;
                case 5:
                    InvertHorizontally();
                    ShuffleDiagonally();
                    break;
                case 6:
                    ShiftElements(false);
                    ShuffleByQuarters();
                    ShuffleDiagonally();
                    break;
                case 7:
                    ShiftElements(true);
                    ShuffleByQuarters();
                    ShuffleDiagonally();
                    break;
            }*/
        }

        private void FisherYatesShuffle()
        {
            var rnd = new Random();
            var positions = _elements.Select(e => e.transform.localPosition).ToList();

            // Алгоритм тасования Фишера-Йейтса для списка позиций
            int n = positions.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (positions[k], positions[n]) = (positions[n], positions[k]);
            }

            // Атомарно обновляем сетку
            foreach (var element in _elements)
            {
                _grid.UnregisterElement(element.transform.localPosition);
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                var newPosition = positions[i];
                _elements[i].SetPosition(newPosition);
                _grid.RegisterElement(newPosition, _elements[i]);
            }
        }

        private void InvertVertically()
        {
            for (int y = 0; y < _height / 2; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int topIndex = y * _width + x;
                    int bottomIndex = (_height - 1 - y) * _width + x;
                    SwapElements(_elements[topIndex], _elements[bottomIndex]);
                }
            }
        }

        private void InvertHorizontally()
        {
            if (_width < 2) return;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width / 2; x++)
                {
                    int leftIndex = y * _width + x;
                    int rightIndex = y * _width + (_width - 1 - x);
                    SwapElements(_elements[leftIndex], _elements[rightIndex]);
                }
            }
        }

        private void SwapCorners()
        {
            if (_width < 2 || _height < 2) return;

            var topLeft = _elements.First();
            var topRight = _elements[_width - 1];
            var bottomLeft = _elements[_width * (_height - 1)];
            var bottomRight = _elements.Last();

            SwapElements(topLeft, bottomRight);
            SwapElements(topRight, bottomLeft);
        }

        private void ShuffleDiagonally()
        {
            if (_width < 2 || _height < 2) return;

            // Меняем главную диагональ с побочной
            for (int i = 0; i < _width; i++)
            {
                int mainDiagIndex = i * _width + i;
                int antiDiagIndex = i * _width + (_width - 1 - i);
                if (mainDiagIndex < _elements.Count && antiDiagIndex < _elements.Count)
                {
                    SwapElements(_elements[mainDiagIndex], _elements[antiDiagIndex]);
                }
            }
        }

        private void ShiftElements(bool shiftLeft = false)
        {
            if (_elements.Count < 2) return;

            if (shiftLeft)
            {
                var first = _elements.First();
                for (int i = 0; i < _elements.Count - 1; i++)
                {
                    SwapElements(_elements[i], _elements[i + 1]);
                }
            }
            else // Shift Right
            {
                var last = _elements.Last();
                for (int i = _elements.Count - 1; i > 0; i--)
                {
                    SwapElements(_elements[i], _elements[i - 1]);
                }
            }
        }

        private void ShuffleByQuarters()
        {
            if (_width < 2 || _height < 2) return;

            int midX = _width / 2;
            int midY = _height / 2;

            for (int y = 0; y < midY; y++)
            {
                for (int x = 0; x < midX; x++)
                {
                    // Меняем верхний левый с правым нижним
                    int tlIndex = y * _width + x;
                    int brIndex = (y + midY) * _width + (x + midX);
                    SwapElements(_elements[tlIndex], _elements[brIndex]);

                    // Меняем верхний правый с левым нижним
                    int trIndex = y * _width + (x + midX);
                    int blIndex = (y + midY) * _width + x;
                    SwapElements(_elements[trIndex], _elements[blIndex]);
                }
            }
        }

        private void SwapElements(LevelElement elementA, LevelElement elementB)
        {
            if (elementA == elementB) return;

            var posA = elementA.transform.localPosition;
            var posB = elementB.transform.localPosition;

            _grid.UnregisterElement(posA);
            _grid.UnregisterElement(posB);

            elementA.SetPosition(posB);
            elementB.SetPosition(posA);

            _grid.RegisterElement(posB, elementA);
            _grid.RegisterElement(posA, elementB);
        }
    }
}