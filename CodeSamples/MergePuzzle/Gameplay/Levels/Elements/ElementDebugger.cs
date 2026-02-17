using System;
using Project.Scripts.Gameplay.Puzzle.Data;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Levels.Elements
{
    [Serializable]
    public class ElementDebugger
    {
        [SerializeField]
        private bool _enableDebug;

        [SerializeField]
        private TMP_Text _text;

        public void Initialize(PuzzlePieceData puzzlePiece)
        {
            if (_enableDebug == false)
                return;

            _text.gameObject.SetActive(true);

            string info = string.Empty;
            info += $"Id: {puzzlePiece.Id}\n";
            info += $"T: {puzzlePiece.TopNeighborId}\n";
            info += $"B: {puzzlePiece.BottomNeighborId}\n";
            info += $"L: {puzzlePiece.LeftNeighborId}\n";
            info += $"R: {puzzlePiece.RightNeighborId}\n";
            _text.text = info;
        }
    }
}