using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Cars
{
    [ConfigCategory(ConfigCategory.Game)]
    public class PlayerConfig : ScriptableConfig
    {
        [SerializeField, PropertyRange(1, 100), SuffixLabel("%")]
        private int _minJumpHeight = 20;

        [SerializeField, MinValue(0)]
        private float _jumpHeight = 5f;

        [SerializeField, MinValue(0.1f)]
        private float _timeToPeak = 0.5f;

        [SerializeField]
        private CarConfig _carConfig;

        [SerializeField, MinValue(0), SuffixLabel("%")]
        private float _moveAccelerationPerPlatform = 1;

        public float MinJumpHeight => JumpVelocity * _minJumpHeight / 100f;
        public float MaxJumpHoldTime => _timeToPeak;
        public float GravityForce => 2 * _jumpHeight / (_timeToPeak * _timeToPeak);
        public float JumpVelocity => 2 * _jumpHeight / _timeToPeak;

        public float MoveAccelerationPerPlatform => _moveAccelerationPerPlatform;
        public CarConfig CarConfig => _carConfig;
    }
}