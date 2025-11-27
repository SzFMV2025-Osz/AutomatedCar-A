namespace AutomatedCar.Models
{
    using SystemComponents.Powertrain;
    using Avalonia.Media;
    using global::AutomatedCar.SystemComponents.Powertrain;
    using System;
    using SystemComponents;
    using Avalonia.Media;
    using Helpers;

    public class AutomatedCar : Car
    {
        private VirtualFunctionBus virtualFunctionBus;
        private Powertrain powertrain;
        private CollisionDetectionService collisionDetectionService;
        private RadarSensor radarSensor;
        private CameraSensor cameraSensor;

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.powertrain=new Powertrain(this.virtualFunctionBus,this);
            this.collisionDetectionService = new (this.virtualFunctionBus);
            this.collisionDetectionService.OnCollided += (sender, o) =>
            Console.WriteLine($"{this.virtualFunctionBus.CurrentTick}: collided with {o.WorldObjectType}");
            this.ZIndex = 10;
            
            this.radarSensor = new RadarSensor(
                this.virtualFunctionBus,
                new Triangle(
                    angle: SensorValues.Radar.FoV,
                    facingAngle: SensorValues.Radar.RelativeAngle,
                    distance: SensorValues.Radar.Distance));
            
            this.cameraSensor = new CameraSensor(
                this.virtualFunctionBus,
                new Triangle(
                    angle: SensorValues.Camera.FoV,
                    facingAngle: SensorValues.Camera.RelativeAngle,
                    distance: SensorValues.Camera.Distance));
        }

        public VirtualFunctionBus VirtualFunctionBus { get => this.virtualFunctionBus; }

        public Powertrain Powertrain { get => this.powertrain; }

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