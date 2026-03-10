using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.UI.Common.Panels;
using UnityEngine.EventSystems;
using Zenject;

namespace Project.Scripts.UI.Game
{
    public class InputPanel : UIPanel, IPointerDownHandler, IPointerUpHandler
    {
        private IInputService _inputService;

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _inputService.PressJump();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _inputService.ReleaseJump();
        }
    }
}