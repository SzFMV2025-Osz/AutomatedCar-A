namespace AutomatedCar.Models.NPC
{
    using System.Collections.Generic;

    public class Pedestrian : PathFollowingNpcBase
    {
        public Pedestrian(int x, int y, string sprite = "pedestrian")
            : base(x, y, sprite) { }

        // Gyalogos fordulhat gyorsabban
        protected override double AngularSpeedDegPerSec => 300.0;

        public void Load(int speed, bool repeating, int currentPoint, List<NPCPath> points)
        {
            this.Speed = speed;
            this.ConfigurePath(points, repeating, currentPoint);
        }
    }
}
