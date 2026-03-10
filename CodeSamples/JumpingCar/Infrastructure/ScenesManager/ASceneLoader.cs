using System;
using System.Collections;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Infrastructure.ScenesManager
{
    public abstract class ASceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public ASceneLoader(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void Load(string name, Action onLoaded = null) =>
            TryLoadScene(name, onLoaded);

        public void Load(Scenes scene, Action onLoaded = null) =>
            TryLoadScene(ConvertToString(scene), onLoaded);

        protected abstract void OnLoadingStart();
        protected abstract void OnLoadingEnd();

        protected virtual void TryLoadScene(string name, Action onLoaded = null) =>
            _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));

        protected virtual IEnumerator LoadScene(string nextScene, Action onLoaded = null)
        {
            OnLoadingStart();
            yield return new WaitForSeconds(GameConstants.SCENE_LOAD_TIME);

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            while (!waitNextScene.isDone)
                yield return null;

            OnLoadingEnd();
            yield return new WaitForSeconds(GameConstants.SCENE_LOAD_TIME);
            onLoaded?.Invoke();
        }

        protected virtual string ConvertToString(Scenes scene) =>
            scene.ToString();
    }
}