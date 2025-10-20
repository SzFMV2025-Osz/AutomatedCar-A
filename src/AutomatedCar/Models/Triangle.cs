namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Triangle : WorldObject, ITriangle
    {
        private double angle;
        private double distance;
        private readonly World world;
        private readonly WorldObject parentObject;
        private List<Avalonia.Point> trianglePoints;

        public Triangle(int x, int y, string filename, double angle, double distance, World world, WorldObject parentObject = null)
            : base(x, y, filename, zindex: 5, collideable: false, WorldObjectType.Other)
        {
            this.angle = angle;
            this.distance = distance;
            this.world = world;
            this.parentObject = parentObject;
            this.trianglePoints = new List<Avalonia.Point>();

            InitializeGeometry();
            UpdateRelativePosition();
        }


        public double Angle => this.angle;

        public double Distance => this.distance;

        private void InitializeGeometry()
        {
            this.Geometries.Clear();
            this.RawGeometries.Clear();

            var geom = new PolylineGeometry();

            var originPoint = new Avalonia.Point(0, 0);

            double halfAngleRad = (this.angle / 2) * Math.PI / 180;

            double leftX = this.distance * Math.Sin(-halfAngleRad);
            double leftY = -this.distance * Math.Cos(-halfAngleRad);
            var leftPoint = new Avalonia.Point(leftX, leftY);

            double rightX = this.distance * Math.Sin(halfAngleRad);
            double rightY = -this.distance * Math.Cos(halfAngleRad);
            var rightPoint = new Avalonia.Point(rightX, rightY);

            geom.Points.Add(originPoint);
            geom.Points.Add(leftPoint);
            geom.Points.Add(rightPoint);
            geom.Points.Add(originPoint);

            this.trianglePoints.Clear();
            this.trianglePoints.Add(originPoint);
            this.trianglePoints.Add(leftPoint);
            this.trianglePoints.Add(rightPoint);

            this.Geometries.Add(geom);
            this.RawGeometries.Add(geom);
        }

        public List<WorldObject> GetIntersections()
        {
            if (this.world == null)
            {
                return new List<WorldObject>();
            }

            var intersectingObjects = new List<WorldObject>();

            var allObjects = this.world.WorldObjects;

            foreach (var worldObject in allObjects)
            {
                if (worldObject == this || worldObject == this.parentObject)
                {
                    continue;
                }

                if (IsObjectInTriangle(worldObject))
                {
                    intersectingObjects.Add(worldObject);
                }
            }

            intersectingObjects = intersectingObjects.OrderBy(obj =>
                Math.Sqrt(Math.Pow(obj.X - this.X, 2) + Math.Pow(obj.Y - this.Y, 2))
            ).ToList();

            return intersectingObjects;
        }


        private bool IsObjectInTriangle(WorldObject worldObject)
        {
            double relativeX = worldObject.X - this.X;
            double relativeY = worldObject.Y - this.Y;

            if (Math.Abs(this.Rotation) > 0.001)
            {
                double rotationRad = -this.Rotation * Math.PI / 180;
                double rotatedX = relativeX * Math.Cos(rotationRad) - relativeY * Math.Sin(rotationRad);
                double rotatedY = relativeX * Math.Sin(rotationRad) + relativeY * Math.Cos(rotationRad);
                relativeX = rotatedX;
                relativeY = rotatedY;
            }

            var objectPoint = new Avalonia.Point(relativeX, relativeY);

            return IsPointInTriangle(objectPoint);
        }


        private bool IsPointInTriangle(Avalonia.Point point)
        {
            if (this.trianglePoints.Count < 3)
            {
                return false;
            }

            var p0 = this.trianglePoints[0];
            var p1 = this.trianglePoints[1];
            var p2 = this.trianglePoints[2];

            double denominator = ((p1.Y - p2.Y) * (p0.X - p2.X) + (p2.X - p1.X) * (p0.Y - p2.Y));

            if (Math.Abs(denominator) < 0.0001)
            {
                return false;
            }

            double a = ((p1.Y - p2.Y) * (point.X - p2.X) + (p2.X - p1.X) * (point.Y - p2.Y)) / denominator;
            double b = ((p2.Y - p0.Y) * (point.X - p2.X) + (p0.X - p2.X) * (point.Y - p2.Y)) / denominator;
            double c = 1 - a - b;

            return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
        }

        public void UpdateRelativePosition()
        {
            if (this.parentObject != null)
            {
                this.X = this.parentObject.X;
                this.Y = this.parentObject.Y;

                this.Rotation = this.parentObject.Rotation;

                InitializeGeometry();
            }
        }

        public void Tick()
        {
            UpdateRelativePosition();

            var intersections = GetIntersections();

        }


        public void SetAngle(double newAngle)
        {
            this.angle = newAngle;
            InitializeGeometry();
        }


        public void SetDistance(double newDistance)
        {
            this.distance = newDistance;
            InitializeGeometry();
        }
    }
}
