using UnityEngine;

namespace Project.Scripts.Gameplay.Levels.Elements
{
    public class HintView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hintImage;

        public void SetHint(bool isHinting)
        {
            if (_hintImage)
            {
                _hintImage.SetActive(isHinting);
            }
        }
    }
}