using Project.Scripts.Configs.Cars;
using Project.Scripts.Gameplay.Platforms.Logic;

namespace Project.Scripts.Gameplay.Cars.Logic
{
    public class EnemyCar : CarBase
    {
        private float _offsetFromPlatform;
        private IPlatform _platform;

        public EnemyCar(CarController carController, IPlatform platform) : base(carController)
        {
            _platform = platform;
        }

        public override void Initialize(CarConfig carConfig)
        {
            base.Initialize(carConfig);

            _offsetFromPlatform = _carInfo.CarSize.y * 0.5f;
        }

        public override void Update()
        {
            if (_platform == null || !_platform.IsActive)
                return;

            var targetY = _platform.TopY + _offsetFromPlatform;
            MoveVertical(targetY);
        }

        public override bool CheckAlive()
        {
            return _platform != null && _platform.IsActive;
        }
    }
}