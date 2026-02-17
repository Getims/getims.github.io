using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Runtime.Enums;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Game
{
    public class LevelCompletePanel : PopupPanel
    {
        [SerializeField]
        private TMP_Text _winText;

        [Inject] private IGameResultService _gameResultService;

        public override void Show()
        {
            base.Show();
            UpdateWinText();
        }

        public override void Hide()
        {
            base.Hide();
        }

        protected override void OnClaimButtonClick()
        {
            base.OnClaimButtonClick();
            Hide();
        }

        private void UpdateWinText()
        {
            UnitTeam winner = _gameResultService.GetWinner();

            _winText.text = $"Team {winner} won the battle!";
        }
    }
}