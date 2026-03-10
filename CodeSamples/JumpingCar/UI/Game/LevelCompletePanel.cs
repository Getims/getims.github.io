using System.Linq;
using Project.Scripts.Data;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Runtime.Constants;
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

        [Inject] private IPlatformsInfoService _platformsInfoService;
        [Inject] private IGameDataService _gameDataService;

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
            int max = _gameDataService.GameScores.Max();

            if (max <= 1)
                _winText.text = $"{_platformsInfoService.CurrentPlatform}";
            else
                _winText.text = $"{_platformsInfoService.CurrentPlatform}\n {GameConstants.RECORD_TITLE}: {max}";
        }
    }
}