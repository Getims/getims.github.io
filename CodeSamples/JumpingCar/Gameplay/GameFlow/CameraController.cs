using Project.Scripts.Configs.Platforms;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.GameFlow
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _referenceOrthoSize = 12f;

        [SerializeField]
        private float _referenceWidthPixels = 1080f;

        [SerializeField]
        private float _referenceHeightPixels = 1920f;

        [Inject] private IConfigsProvider _configsProvider;

        private PlatformsConfigProvider _platformsConfigProvider;

        public void Initialize()
        {
            _platformsConfigProvider = _configsProvider.GetConfig<PlatformsConfigProvider>();

            ApplyCameraSettings();
        }

        private void ApplyCameraSettings()
        {
            var minSize = (_platformsConfigProvider.PlatformCountOnScreen + 0.5f) *
                _platformsConfigProvider.PlatformSpacing / 2;

            float referenceAspect = _referenceWidthPixels / _referenceHeightPixels;
            float currentAspect = (float)Screen.width / Screen.height;

            var newSize = _referenceOrthoSize * referenceAspect / currentAspect;
            if (newSize < minSize)
                newSize = minSize;

            _camera.orthographicSize = newSize;
        }
    }
}