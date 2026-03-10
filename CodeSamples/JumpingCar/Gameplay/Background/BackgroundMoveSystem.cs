using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Configs.Background;
using Project.Scripts.Configs.Platforms;
using UnityEngine;

namespace Project.Scripts.Gameplay.Background
{
    public class BackgroundMoveSystem
    {
        private readonly List<BackgroundChunk> _chunks;
        private readonly float _moveDownDistance;
        private readonly float _jumpOnPlaceMovePercent;
        private readonly float _parallaxFactor;
        private readonly float _moveDuration;

        private Sequence _sequence;

        public BackgroundMoveSystem(List<BackgroundChunk> chunks, PlatformsConfigProvider platformsConfigProvider,
            BackgroundConfigProvider backgroundConfigProvider)
        {
            _chunks = chunks;

            _moveDuration = platformsConfigProvider.MoveDuration;
            _moveDownDistance = platformsConfigProvider.PlatformSpacing;
            _jumpOnPlaceMovePercent = platformsConfigProvider.JumpOnPlaceMovePercent / 100f;
            _parallaxFactor = backgroundConfigProvider.BackgroundParallaxFactor;
        }

        public void MoveOnNewPlatform()
        {
            MoveBackground(_moveDownDistance * _parallaxFactor);
        }

        public void MoveOnSamePlatform()
        {
            MoveBackground(_moveDownDistance * _jumpOnPlaceMovePercent * _parallaxFactor);
        }


        public void StopMove()
        {
            _sequence?.Kill(false);
        }

        private void MoveBackground(float distance)
        {
            _sequence?.Complete(false);
            _sequence = DOTween.Sequence();

            foreach (var chunk in _chunks)
            {
                Vector3 target = chunk.Transform.position - new Vector3(0, distance, 0);
                _sequence.Join(chunk.Transform.DOMove(target, _moveDuration).SetEase(Ease.Linear));
            }

            _sequence.OnComplete(RepositionIfNeeded);
        }

        private void RepositionIfNeeded()
        {
            float totalHeight = _chunks[0].Height * _chunks.Count;

            foreach (var chunk in _chunks)
            {
                if (chunk.Transform.position.y < -chunk.Height)
                {
                    chunk.Transform.position += new Vector3(0, totalHeight, 0);
                }
            }
        }
    }
}