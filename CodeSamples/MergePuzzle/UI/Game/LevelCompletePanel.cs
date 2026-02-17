using Project.Scripts.Runtime.Audio;
using Project.Scripts.UI.Common.Panels;
using Zenject;

namespace Project.Scripts.UI.Game
{
    public class LevelCompletePanel : PopupPanel
    {
        [Inject] private ISoundService _soundService;

        public override void Show()
        {
            base.Show();
            _soundService.PlayWinSound();
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
    }
}