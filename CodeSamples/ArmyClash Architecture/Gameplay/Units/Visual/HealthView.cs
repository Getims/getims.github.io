using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Units.Visual
{
    public class HealthView : MonoBehaviour
    {
        private const string HEALTH = "HP:";

        [SerializeField]
        private TMP_Text _text;

        private Camera _mainCamera;

        public void SetHealth(int amount)
        {
            _text.text = $"{HEALTH} {amount}";
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera != null)
            {
                transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                    _mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}