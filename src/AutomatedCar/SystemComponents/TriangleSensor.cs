namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TriangleSensor : SystemComponent
    {
        private readonly AutomatedCar car;
        private readonly Triangle sensorFieldOfView;
        private readonly World world;
        private List<WorldObject> detectedObjects;


        public TriangleSensor(VirtualFunctionBus virtualFunctionBus, AutomatedCar car, double angle = 60, double distance = 300)
            : base(virtualFunctionBus)
        {
            this.car = car;
            this.world = World.Instance;

            this.sensorFieldOfView = new Triangle(
                car.X,
                car.Y,
                "sensor_fov.png",
                angle,
                distance,
                this.world,
                car
            );

            this.world.AddObject(this.sensorFieldOfView);

            this.detectedObjects = new List<WorldObject>();
        }


        public override void Process()
        {
            this.sensorFieldOfView.Tick();

            this.detectedObjects = this.sensorFieldOfView.GetIntersections();

            ProcessDetectedObjects();
        }

        private void ProcessDetectedObjects()
        {
            foreach (var obj in this.detectedObjects)
            {
                double distance = CalculateDistance(obj);

                if (IsCollisionThreat(obj, distance))
                {
                    HandlePotentialCollision(obj, distance);
                }


            }
        }


        private double CalculateDistance(WorldObject obj)
        {
            double dx = obj.X - this.car.X;
            double dy = obj.Y - this.car.Y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }


        private bool IsCollisionThreat(WorldObject obj, double distance)
        {
            return obj.Collideable && distance < 50;
        }

        private void HandlePotentialCollision(WorldObject obj, double distance)
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Potential collision with {obj.GetType().Name} at distance {distance:F2}");


        }


        public List<WorldObject> GetDetectedObjects()
        {
            return new List<WorldObject>(this.detectedObjects);
        }


        public void SetFieldOfViewAngle(double newAngle)
        {
            this.sensorFieldOfView.SetAngle(newAngle);
        }


        public void SetDetectionDistance(double newDistance)
        {
            this.sensorFieldOfView.SetDistance(newDistance);
        }
    }
}
