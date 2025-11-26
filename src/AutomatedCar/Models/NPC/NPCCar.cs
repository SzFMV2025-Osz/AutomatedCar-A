namespace AutomatedCar.Models.NPC
{
    using System.Collections.Generic;
    using System;
    

    public class NPCCar : PathFollowingNpcBase
    {
        // braking state fields
        private bool braking = false;
        private double currentSpeedM = 0.0; // speed in "m/s" as given in JSON
        private const double BrakingDecelMPerS2 = 9.0; // 9 m/s^2 deceleration requirement

        public NPCCar(int x, int y, string sprite = "car_3_black")
            : base(x, y, sprite)
        {
            this.WorldObjectType = WorldObjectType.Car;
        }

        // Car turns slower than pedestrian
        protected override double AngularSpeedDegPerSec => 120.0;

        // NEW: scale speeds from JSON to car-like px/s
        protected override double SpeedMultiplier => 20.0;

        public void Load(int speed, bool repeating, int currentPoint, List<NPCPath> points)
        {
            this.Speed = speed;
            this.currentSpeedM = speed; // initial speed (m/s)
            this.ConfigurePath(points, repeating, currentPoint);
        }

        // override movement to support "hard braking after one lap" while keeping normal path-following otherwise
        public override void Move(double dt)
        {
            if (this.MutablePoints == null || this.MutablePoints.Count == 0) return;

            // If not braking yet, use base movement to follow path naturally.
            if (!braking)
            {
                int prevPoint = this.CurrentPoint;
                base.Move(dt);

                // detect lap completion: wrapped back to 0 from non-zero -> start braking
                if (this.Repeating && this.CurrentPoint == 0 && prevPoint != 0)
                {
                    braking = true;
                    // start braking from current numeric speed (Speed is an int m/s)
                    currentSpeedM = this.Speed;
                }

                return;
            }

            // braking: continue following path but reduce speed by 9 m/s^2 until stopped
            if (this.CurrentPoint == this.MutablePoints.Count - 1 && !this.Repeating) return;

            int next = this.CurrentPoint + 1;
            if (next >= this.MutablePoints.Count) next = 0;

            var target = this.MutablePoints[next];

            // respect any existing wait timer (inherited protected waitLeftSec)
            if (this.waitLeftSec > 0)
            {
                this.waitLeftSec -= dt;
                if (this.waitLeftSec > 0) { this.RotateTowards(target.Rotation, dt); this.UpdateHitboxTransform(); return; }
            }

            double dx = target.X - this.X;
            double dy = target.Y - this.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // actual pixels per second movement using currentSpeedM (m/s) * SpeedMultiplier => px/s
            double step = (currentSpeedM * this.SpeedMultiplier) * dt;

            if (dist <= step || dist < 1e-6)
            {
                this.X = target.X;
                this.Y = target.Y;
                this.posX = target.X;
                this.posY = target.Y;

                // don't overwrite currentSpeedM here (we're braking)
                this.CurrentPoint = next;

                this.waitLeftSec = target.WaitMs / 1000.0;
                this.RotateTowards(target.Rotation, dt);
            }
            else
            {
                double ux = dx / dist;
                double uy = dy / dist;

                this.posX += ux * step;
                this.posY += uy * step;

                int newX = (int)Math.Round(this.posX);
                int newY = (int)Math.Round(this.posY);

                if (newX != this.X) this.X = newX;
                if (newY != this.Y) this.Y = newY;

                this.RotateTowards(target.Rotation, dt);
            }

            this.UpdateHitboxTransform();

            // apply deceleration (m/s^2)
            currentSpeedM -= BrakingDecelMPerS2 * dt;
            if (currentSpeedM <= 0.0)
            {
                currentSpeedM = 0.0;
                this.Speed = 0;
                // stop repeating to keep the NPC stopped after braking
                this.Repeating = false;
            }
        }
    }
}
