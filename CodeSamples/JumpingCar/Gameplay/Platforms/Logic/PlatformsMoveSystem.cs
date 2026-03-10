using System;
using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Configs.Platforms;
using UnityEngine;

namespace Project.Scripts.Gameplay.Platforms.Logic
{
    public class PlatformsMoveSystem
    {
        public event Action OnMoveComplete;

        private readonly float _moveDownDistance;
        private readonly float _jumpOnPlaceMovePercent;
        private readonly float _moveDuration;

        private Sequence _currentSequence;
        private int _samePlatformJumps = 0;

        public PlatformsMoveSystem(PlatformsConfigProvider platformsConfigProvider)
        {
            _moveDownDistance = platformsConfigProvider.PlatformSpacing;
            _jumpOnPlaceMovePercent = platformsConfigProvider.JumpOnPlaceMovePercent / 100f;
            _moveDuration = platformsConfigProvider.MoveDuration;
        }

        public void MoveOnNewPlatform(List<IPlatform> platforms)
        {
            if (_samePlatformJumps == 0)
            {
                MovePlatforms(platforms, _moveDownDistance);
                return;
            }

            var delta = _samePlatformJumps * _jumpOnPlaceMovePercent * _moveDownDistance;
            delta = Mathf.Min(delta, _moveDownDistance);
            _samePlatformJumps = 0;
            MovePlatforms(platforms, _moveDownDistance - delta);
        }

        public void MoveOnSamePlatform(List<IPlatform> platforms)
        {
            MovePlatforms(platforms, _moveDownDistance * _jumpOnPlaceMovePercent);
            _samePlatformJumps++;
        }

        public void StopMove()
        {
            _currentSequence?.Kill(false);
        }

        private void MovePlatforms(List<IPlatform> platforms, float distance)
        {
            _currentSequence?.Complete(false);

            _currentSequence = DOTween.Sequence();

            foreach (var platform in platforms)
            {
                if (platform != null && platform.gameObject.activeInHierarchy)
                {
                    Vector3 targetPosition = platform.Position - new Vector3(0, distance, 0);
                    _currentSequence.Join(platform.DOMove(targetPosition, _moveDuration)
                        .SetEase(Ease.Linear));
                }
            }

            _currentSequence.OnComplete(() =>
            {
                OnMoveComplete?.Invoke();
                _currentSequence = null;
            });

            _currentSequence.Play();
        }
    }
}