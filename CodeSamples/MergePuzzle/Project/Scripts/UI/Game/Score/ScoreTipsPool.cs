using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.UI.Game.Score
{
    [Serializable]
    public class ScoreTipsPool
    {
        [SerializeField]
        private List<ScoreTip> _scoreTips;

        private int _currentIndex = 0;

        public ScoreTip GetScoreTip()
        {
            int startIndex = _currentIndex;

            do
            {
                var tip = _scoreTips[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _scoreTips.Count;

                if (!tip.IsAnimating)
                    return tip;
            } while (_currentIndex != startIndex);

            return _scoreTips[_currentIndex];
        }

        public void HideAllTips()
        {
            foreach (var tip in _scoreTips)
                tip.Hide();
        }
    }
}