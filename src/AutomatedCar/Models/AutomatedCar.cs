namespace AutomatedCar.Models
{
    using System;
    using SystemComponents;
    using Avalonia.Media;
    public class AutomatedCar : Car
    {
        private VirtualFunctionBus virtualFunctionBus;
        private CollisionDetectionService collisionDetectionService;
        
        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.collisionDetectionService = new (this.virtualFunctionBus);
            this.collisionDetectionService.OnCollided += (sender, o) =>
            Console.WriteLine($"{this.virtualFunctionBus.CurrentTick}: collided with {o.WorldObjectType}");
            this.ZIndex = 10;
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