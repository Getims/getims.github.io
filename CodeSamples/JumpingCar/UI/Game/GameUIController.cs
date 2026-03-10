using Project.Scripts.UI.Common;
using Project.Scripts.UI.Common.Panels;

namespace Project.Scripts.UI.Game
{
    public interface IGameUIController
    {
        T GetPanel<T>() where T : UIPanel;
        T ShowPanel<T>() where T : UIPanel;
        void Initialize();
    }

    public class GameUIController : AUIController, IGameUIController
    {
        public void Initialize()
        {
        }
    }
}