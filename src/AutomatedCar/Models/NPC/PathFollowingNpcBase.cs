namespace AutomatedCar.Models.NPC
{
    
    using Avalonia;
    using Avalonia.Media;
    using global::AutomatedCar.Helpers;
    using System;
    using System.Collections.Generic;

    public abstract class PathFollowingNpcBase : WorldObject, INPC
    {
        // made protected so subclasses (NPCCar) can access/modify during braking
        protected double posX;
        protected double posY;

        protected PathFollowingNpcBase(int x, int y, string sprite)
            : base(x, y, sprite)
        {
            posX = x;
            posY = y;
        }

        protected virtual double DefaultFps => 60.0;
        protected virtual double AngularSpeedDegPerSec => 180.0;
        protected virtual double SpeedMultiplier => 1.0;

        public List<NPCPath> MutablePoints { get; protected set; } = new();
        public IReadOnlyList<NPCPath> Points => MutablePoints;
        public bool Repeating { get; protected set; } = true;
        public int CurrentPoint { get; protected set; } = 0;
        public int Speed { get; protected set; } = 60;

        // allow subclasses to observe and manipulate the wait timer
        protected double waitLeftSec = 0.0;

        public void ConfigurePath(List<NPCPath> points, bool repeating, int startIndex = 0)
        {
            MutablePoints = points ?? new List<NPCPath>();
            Repeating = repeating;
            CurrentPoint = Math.Clamp(startIndex, 0, Math.Max(0, MutablePoints.Count - 1));
            if (MutablePoints.Count > 0) Speed = MutablePoints[CurrentPoint].Speed;

            posX = this.X;
            posY = this.Y;

            // --- HITBOX FELÉPÍTÉS ---
            EnsureNpcHitbox();
            UpdateHitboxTransform();
        }

        public void Move() => Move(1.0 / DefaultFps);

        // make virtual so NPCCar can override and implement braking behavior
        public virtual void Move(double dt)
        {
            if (MutablePoints == null || MutablePoints.Count == 0) return;
            if (CurrentPoint == MutablePoints.Count - 1 && !Repeating) return;

            int next = CurrentPoint + 1;
            if (next >= MutablePoints.Count) next = 0;

            var target = MutablePoints[next];

            if (waitLeftSec > 0)
            {
                waitLeftSec -= dt;
                if (waitLeftSec > 0) { RotateTowards(target.Rotation, dt); UpdateHitboxTransform(); return; }
            }

            double dx = target.X - this.X;
            double dy = target.Y - this.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

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

            UpdateHitboxTransform();
        }

        // allow subclasses to rotate NPC smoothly
        protected void RotateTowards(double targetDeg, double dt)
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
            => (to - from + 540.0) % 360.0 - 180.0;

        // ----------------- HITBOX -----------------

        private void EnsureNpcHitbox()
        {
            // Ha már van poligon, ne építsük újra
            if (this.Geometries.Count > 0 && this.RawGeometries.Count > 0)
            {
                this.Collideable = true;
                return;
            }

            // 1) próbáljuk a library-t
            if (NpcHitboxHelper.TryGetLibraryPolys(this.Filename, out var polys))
            {
                this.RawGeometries.Clear();
                this.Geometries.Clear();
                foreach (var p in polys)
                {
                    this.RawGeometries.Add(new PolylineGeometry(p.Points, false));
                    this.Geometries.Add(new PolylineGeometry(p.Points, false));
                }
            }
            else
            {
                // 2) hull a sprite-ból (jó a gyalogosokra)
                var hull = NpcHitboxHelper.BuildHullFromSprite(this.Filename, sampleStep: 3);
                this.RawGeometries.Clear();
                this.Geometries.Clear();
                this.RawGeometries.Add(new PolylineGeometry(hull.Points, false));
                this.Geometries.Add(new PolylineGeometry(hull.Points, false));

                // rp fallback: sprite közepe
                if (this.RotationPoint.X == 0 && this.RotationPoint.Y == 0)
                {
                    var c = NpcHitboxHelper.GetSpriteCenter(this.Filename);
                    this.RotationPoint = new System.Drawing.Point(c.x, c.y);
                }
            }

            this.Collideable = true; // <- ettől veszik figyelembe a szenzorok
        }

        // allow subclasses to update hitbox after moving / rotating
        protected void UpdateHitboxTransform()
        {
            if (this.RawGeometries.Count == 0 || this.Geometries.Count == 0) return;

            var src = this.RawGeometries[0].Points;
            var outPts = new Points();

            // ugyanaz a transzformálás, mint a GeometryHelper.PositionWorldObjectPointsToAbsolute-ben
            var rot = new RotateTransform
            {
                CenterX = this.RotationPoint.X,
                CenterY = this.RotationPoint.Y,
                Angle = this.Rotation
            };
            var tr = new TranslateTransform
            {
                X = this.X,
                Y = this.Y
            };

            foreach (var p in src)
                outPts.Add(p.Transform(rot.Value).Transform(tr.Value));

            this.Geometries[0].Points = outPts;
        }
    }
}
