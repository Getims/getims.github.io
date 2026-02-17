using Project.Scripts.Runtime.Events;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.MainMenu.Levels
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField]
        private Image _lockedIcon;

        [SerializeField]
        private Image _unlockedIcon;

        [SerializeField]
        private Image _unlockedBackground;

        [SerializeField]
        private Image _selectedFrame;

        [SerializeField]
        private Animation _animation;

        [Inject] private MenuEventsProvider _menuEventsProvider;

        private int _levelNumber;
        private LevelState _currentState;

        public void Initialize(int number)
        {
            _levelNumber = number;
        }

        public void SetImage(Sprite sprite)
        {
            _unlockedBackground.sprite = sprite;
        }

        public void UpdateState(int currentLevelIndex)
        {
            var previousState = _currentState;

            if (_levelNumber > currentLevelIndex)
                _currentState = LevelState.Locked;
            else if (_levelNumber == currentLevelIndex)
                _currentState = LevelState.Selected;
            else
                _currentState = LevelState.Unlocked;

            HandleStateTransitions(previousState, _currentState);
        }

        private void ApplyStateVisuals()
        {
            _selectedFrame.enabled = _currentState == LevelState.Selected;
            _lockedIcon.enabled = _currentState == LevelState.Locked;
            _unlockedIcon.enabled = _currentState == LevelState.Selected;
            _unlockedBackground.enabled = _currentState == LevelState.Unlocked;
        }

        private void HandleStateTransitions(LevelState from, LevelState to)
        {
            if (from == LevelState.Selected && to == LevelState.Unlocked)
            {
                PlayCompletionAnimation();
                return;
            }

            ApplyStateVisuals();
        }

        private void PlayCompletionAnimation()
        {
            _menuEventsProvider.MoneySpawnRequest.Invoke(transform.position);
            _animation.enabled = true;
            _animation.Play();
        }
    }

    public enum LevelState
    {
        Locked,
        Unlocked,
        Selected
    }
}