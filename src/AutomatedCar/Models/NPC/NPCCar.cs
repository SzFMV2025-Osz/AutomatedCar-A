namespace AutomatedCar.Models.NPC
{
    using System.Collections.Generic;

    public class NPCCar : PathFollowingNpcBase
    {
        public NPCCar(int x, int y, string sprite = "car_3_black")
            : base(x, y, sprite) { }

        // Car turns slower than pedestrian
        protected override double AngularSpeedDegPerSec => 120.0;

        // NEW: scale speeds from JSON to car-like px/s
        protected override double SpeedMultiplier => 20.0;

        public void Load(int speed, bool repeating, int currentPoint, List<NPCPath> points)
        {
            this.Speed = speed;
            this.ConfigurePath(points, repeating, currentPoint);
        }
    }
}
