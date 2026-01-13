using Project.Scripts.UI.Common.Panels;

namespace Project.Scripts.UI.MainMenu.Settings
{
    public class MainMenuSettingsPopup : PopupPanel
    {
        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            DestroySelfDelayed();
        }
    }
}