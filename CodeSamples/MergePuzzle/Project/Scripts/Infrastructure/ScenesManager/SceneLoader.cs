using System;
using System.Collections;
using Project.Scripts.Core.Constants;
using Project.Scripts.Core.Enums;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.UI.LoadScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Infrastructure.ScenesManager
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly LoadingPanel _loadingPanel;

        public SceneLoader(ICoroutineRunner coroutineRunner, LoadingPanel loadingPanel)
        {
            _loadingPanel = loadingPanel;
            _coroutineRunner = coroutineRunner;
        }

        public void Load(string name, Action onLoaded = null) =>
            TryLoadScene(name, onLoaded);

        public void Load(Scenes scene, Action onLoaded = null) =>
            TryLoadScene(ConvertToString(scene), onLoaded);

        private void TryLoadScene(string name, Action onLoaded = null) =>
            _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));

        private IEnumerator LoadScene(string nextScene, Action onLoaded = null)
        {
            _loadingPanel.Show();
            yield return new WaitForSeconds(GameConstants.SCENE_LOAD_TIME);

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            while (!waitNextScene.isDone)
                yield return null;

            _loadingPanel.Hide();
            yield return new WaitForSeconds(GameConstants.SCENE_LOAD_TIME);
            onLoaded?.Invoke();
        }

        private string ConvertToString(Scenes scene) =>
            scene.ToString();
    }
}