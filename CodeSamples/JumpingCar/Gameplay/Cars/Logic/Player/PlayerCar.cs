using System;
using Project.Scripts.Configs.Cars;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Events;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars.Logic.Player
{
    public class PlayerCar : CarBase
    {
        private readonly IInputService _inputService;
        private readonly GameEventsProvider _gameEventsProvider;
        private readonly GravitySystem _gravitySystem;
        private readonly PlayerConfig _playerConfig;

        private IPlatform _currentPlatform;

        private bool _isAlive = true;

        private float _bottomY;
        private float _offsetFromPlatform;
        private Action _showExplosion;

        public PlayerCar(PlayerCarController carController, IPlatform platform, PlayerConfig playerConfig,
            GameEventsProvider gameEventsProvider, IInputService inputService) : base(
            carController)
        {
            _playerConfig = playerConfig;
            _inputService = inputService;
            _gameEventsProvider = gameEventsProvider;
            _bottomY = -Camera.main.orthographicSize;

            _gravitySystem = new GravitySystem(playerConfig, MoveVertical);

            ChangePlatform(platform);

            _inputService.OnJumpPressed += OnJumpPressed;
            _inputService.OnJumpReleased += OnJumpReleased;
            carController.OnCollision += OnCollisionEnter2D;
            _showExplosion = carController.ShowExplosion;
        }

        public override bool CheckAlive() => _isAlive;

        public override void Initialize(CarConfig carConfig)
        {
            base.Initialize(carConfig);
            _offsetFromPlatform = _carInfo.CarSize.y * 0.5f;
        }

        public override void Update()
        {
            if (_gravitySystem.IsJumping)
            {
                _gravitySystem.ApplyGravity(Position.y);
                CheckPlatformLanded();
            }
            else
            {
                FollowPlatform();
            }

            if (Position.y < _bottomY)
                GameOver();
        }

        public override void Dispose()
        {
            base.Dispose();
            _inputService.OnJumpPressed -= OnJumpPressed;
            _inputService.OnJumpReleased -= OnJumpReleased;
        }

        private void CheckPlatformLanded()
        {
            if (_gravitySystem.VerticalVelocity > 0 || !CheckGroundedByRaycast(out IPlatform platform))
                return;

            if (_currentPlatform != platform)
            {
                ChangePlatform(platform);
                _gameEventsProvider.PlayerLandedNewPlatformEvent.Invoke(_currentPlatform);
            }
            else
                _gameEventsProvider.PlayerLandedSamePlatformEvent.Invoke(_currentPlatform);

            LandOnPlatform();
        }

        private void LandOnPlatform()
        {
            _gravitySystem.Land();

            float targetY = _currentPlatform.TopY + _offsetFromPlatform;
            MoveVertical(targetY);
        }

        private void FollowPlatform()
        {
            if (_currentPlatform == null)
                return;

            float targetY = _currentPlatform.TopY + _offsetFromPlatform;
            MoveVertical(targetY);
        }

        private bool CheckGroundedByRaycast(out IPlatform platform)
        {
            platform = null;

            float rayDistance = 0.2f;

            int mask = LayerMask.GetMask(GameTags.PLATFORM);
            var raycastPosition = Position;
            raycastPosition.y -= _carController.GetSpriteSize().y * 0.5f;
            RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, rayDistance, mask);

            if (hit.collider != null)
            {
                platform = hit.collider.GetComponent<IPlatform>();
                return platform != null;
            }

            return false;
        }

        private void ChangePlatform(IPlatform platform)
        {
            _currentPlatform = platform;
            _carInfo.SetAcceleration(_currentPlatform.Number * _playerConfig.MoveAccelerationPerPlatform / 100f);
        }

        private void GameOver()
        {
            _showExplosion?.Invoke();
            _gameEventsProvider.PlayerDeadEvent.Invoke();
            _isAlive = false;
        }

        private void OnJumpPressed()
        {
            _gravitySystem.Jump();
            _gameEventsProvider.PlayerJumpEvent.Invoke();
        }

        private void OnJumpReleased()
        {
            _gravitySystem.ReleaseJump();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out CarController enemyCar))
                GameOver();
        }
    }
}