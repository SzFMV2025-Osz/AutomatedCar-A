namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using System;
    using SystemComponents;

    public class AutomatedCar : Car
    {
        private VirtualFunctionBus virtualFunctionBus;
        private CollisionDetectionService collisionDetectionService;
        private RadarSensor radarSensor;
        private CameraSensor cameraSensor;

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.collisionDetectionService = new (this.virtualFunctionBus);
            this.collisionDetectionService.OnCollided += (sender, o) =>
                Console.WriteLine($"{this.virtualFunctionBus.CurrentTick}: collided with {o.WorldObjectType}");
            this.ZIndex = 10;
            this.radarSensor = new (this.virtualFunctionBus, new Triangle(60, 0, 10000, global::AutomatedCar.Helpers.GeometryHelper.GetCarAbsolutePolygon().Points[34]));
            this.cameraSensor = new (this.virtualFunctionBus,
                                new Triangle(60, 0, 4000,
                                             global::AutomatedCar.Helpers.GeometryHelper.DividePoints(global::AutomatedCar.Helpers.GeometryHelper.GetCarAbsolutePolygon().Points[34],
                                             global::AutomatedCar.Helpers.GeometryHelper.GetCarAbsolutePolygon().Points[0],
                                             0.4)));
        }

        public VirtualFunctionBus VirtualFunctionBus { get => this.virtualFunctionBus; }

        public int Revolution { get; set; }

        public int Velocity { get; set; }

        public PolylineGeometry Geometry { get; set; }

        /// <summary>Starts the automated cor by starting the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Start()
        {
            this.virtualFunctionBus.Start();
        }

        /// <summary>Stops the automated cor by stopping the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Stop()
        {
            this.virtualFunctionBus.Stop();
        }
    }
}