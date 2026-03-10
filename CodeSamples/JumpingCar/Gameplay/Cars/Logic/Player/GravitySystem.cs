using System;
using Project.Scripts.Configs.Cars;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars.Logic.Player
{
    public class GravitySystem
    {
        private readonly PlayerConfig _playerConfig;
        private readonly Action<float> _moveVertical;

        private bool _isJumping;
        private bool _jumpButtonHeld;
        private float _jumpStartTime;
        private float _verticalVelocity;

        public float VerticalVelocity => _verticalVelocity;
        public bool IsJumping => _isJumping;

        public GravitySystem(PlayerConfig playerConfig, Action<float> moveVertical)
        {
            _playerConfig = playerConfig;
            _moveVertical = moveVertical;
        }

        public void ApplyGravity(float currentY)
        {
            if (_jumpButtonHeld)
            {
                float holdTime = Time.time - _jumpStartTime;
                if (holdTime >= _playerConfig.MaxJumpHoldTime)
                    _jumpButtonHeld = false;
            }

            float gravity = _playerConfig.GravityForce;
            if (!_jumpButtonHeld && _verticalVelocity > 0)
                gravity *= 1.1f;

            _verticalVelocity -= gravity * Time.deltaTime;

            _moveVertical(currentY + _verticalVelocity * Time.deltaTime);
        }

        public void Jump()
        {
            if (_isJumping)
                return;

            _isJumping = true;
            _jumpButtonHeld = true;
            _jumpStartTime = Time.time;

            _verticalVelocity = _playerConfig.JumpVelocity;
        }

        public void ReleaseJump()
        {
            if (_verticalVelocity > _playerConfig.MinJumpHeight)
                _verticalVelocity = _playerConfig.MinJumpHeight;

            _jumpButtonHeld = false;
        }

        public void Land()
        {
            _isJumping = false;
            _verticalVelocity = 0;
        }
    }
}