namespace AutomatedCar.Models.NPC
{
    using System;
    using System.Collections.Generic;

    public abstract class PathFollowingNpcBase : WorldObject, INPC
    {
        private double posX;
        private double posY;

        protected PathFollowingNpcBase(int x, int y, string sprite)
            : base(x, y, sprite)
        {
            posX = x;
            posY = y;
        }

        protected virtual double DefaultFps => 60.0;
        protected virtual double AngularSpeedDegPerSec => 180.0;

        // NEW: allows derived types to scale path speeds (px/s)
        protected virtual double SpeedMultiplier => 1.0;

        public List<NPCPath> MutablePoints { get; protected set; } = new List<NPCPath>();
        public IReadOnlyList<NPCPath> Points => MutablePoints;
        public bool Repeating { get; protected set; } = true;
        public int CurrentPoint { get; protected set; } = 0;
        public int Speed { get; protected set; } = 60;

        private double waitLeftSec = 0.0;

        public void ConfigurePath(List<NPCPath> points, bool repeating, int startIndex = 0)
        {
            MutablePoints = points ?? new List<NPCPath>();
            Repeating = repeating;
            CurrentPoint = Math.Clamp(startIndex, 0, Math.Max(0, MutablePoints.Count - 1));
            if (MutablePoints.Count > 0) Speed = MutablePoints[CurrentPoint].Speed;

            posX = this.X;
            posY = this.Y;
        }

        public void Move() => Move(1.0 / DefaultFps);

        public void Move(double dt)
        {
            if (MutablePoints == null || MutablePoints.Count == 0) return;
            if (CurrentPoint == MutablePoints.Count - 1 && !Repeating) return;

            int next = CurrentPoint + 1;
            if (next >= MutablePoints.Count) next = 0;

            var target = MutablePoints[next];

            if (waitLeftSec > 0)
            {
                waitLeftSec -= dt;
                if (waitLeftSec > 0) { RotateTowards(target.Rotation, dt); return; }
            }

            double dx = target.X - this.X;
            double dy = target.Y - this.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // CHANGED: apply multiplier
            double step = (Speed * SpeedMultiplier) * dt;

            if (dist <= step || dist < 1e-6)
            {
                this.X = target.X;
                this.Y = target.Y;
                posX = target.X;
                posY = target.Y;

                this.Speed = target.Speed;
                this.CurrentPoint = next;

                waitLeftSec = target.WaitMs / 1000.0;
                RotateTowards(target.Rotation, dt);
            }
            else
            {
                double ux = dx / dist;
                double uy = dy / dist;

                posX += ux * step;
                posY += uy * step;

                int newX = (int)Math.Round(posX);
                int newY = (int)Math.Round(posY);

                if (newX != this.X) this.X = newX;
                if (newY != this.Y) this.Y = newY;

                RotateTowards(target.Rotation, dt);
            }
        }

        private void RotateTowards(double targetDeg, double dt)
        {
            double current = Normalize360(this.Rotation);
            double target = Normalize360(targetDeg);
            double delta = ShortestDeltaDeg(current, target);

            double step = AngularSpeedDegPerSec * dt;
            if (Math.Abs(delta) <= step)
            {
                this.Rotation = Normalize360(target);
            }
            else
            {
                this.Rotation = Normalize360(current + Math.Sign(delta) * step);
            }
        }

        private static double Normalize360(double a)
        {
            a %= 360.0;
            return (a < 0) ? a + 360.0 : a;
        }

        private static double ShortestDeltaDeg(double from, double to)
        {
            double d = (to - from + 540.0) % 360.0 - 180.0;
            return d;
        }
    }
}