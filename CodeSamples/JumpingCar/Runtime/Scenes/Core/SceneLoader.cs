using Project.Scripts.Infrastructure.ScenesManager;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.UI.LoadScreen;

namespace Project.Scripts.Runtime.Scenes.Core
{
    public class SceneLoader : ASceneLoader
    {
        private readonly LoadingPanel _loadingPanel;

        public SceneLoader(ICoroutineRunner coroutineRunner, LoadingPanel loadingPanel) : base(coroutineRunner)
        {
            _loadingPanel = loadingPanel;
        }

        protected override void OnLoadingStart()
        {
            _loadingPanel.Show();
        }

        protected override void OnLoadingEnd()
        {
            _loadingPanel.Hide();
        }
    }
}