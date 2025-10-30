namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Enginee;
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.GearBox_test;
    using AutomatedCar.SystemComponents.InputHandling.Brake;
    using AutomatedCar.SystemComponents.InputHandling.Throttle;
    using AutomatedCar.SystemComponents.InputHandling.Wheel;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.Input_Packets;

    public class Powertrain : SystemComponent
    {
        public IWheel Wheel { get; set; }

        public IThrottle Throttle { get; set; }

        public IEngine Engine { get; set; }

        public IGearBox GearBox { get; set; }

        public IBrake Brake { get; set; }

        public MovementCalculator MovementCalculator { get; set; }

        public PowertrainPacket PowertrainPacket { get; set; }

        InputDevicePacket inputPacket;

        public Powertrain(VirtualFunctionBus virtualFunctionBus, AutomatedCar car)
            : base(virtualFunctionBus)
        {
            this.MovementCalculator = new MovementCalculator(car);
            this.Wheel = new Wheel();
            this.Throttle = new Throttle();
            this.GearBox = new GearBox();
            this.Brake = new Brake();
            this.Engine = new Engine(GearBox, Throttle);
            this.PowertrainPacket = new PowertrainPacket();
            this.virtualFunctionBus.PowertrainPacket = PowertrainPacket;
        }

        public override void Process()
        {
            var khp = virtualFunctionBus.KeyboardHandlerPacket;
            if (khp != null)
            {
                if (inputPacket == null)
                {
                    inputPacket = new InputDevicePacket();
                }

                // Billentyűzet értékek átemelése
                inputPacket.BrakePercentage = khp.BrakePercentage ?? 0;
                inputPacket.ThrottlePercentage = khp.ThrottlePercentage ?? 0;
                inputPacket.WheelPercentage = khp.WheelPercentage ?? 0;
                inputPacket.ShiftUpOrDown = khp.ShiftUpOrDown ?? ShiftDir.Nothing;

                int brakePercentage = inputPacket.BrakePercentage ?? 0;
                int throttlePercentage = inputPacket.ThrottlePercentage ?? 0;
                int wheelPercentage = (int)(inputPacket.WheelPercentage ?? 0);

                var shiftUpOrDown = inputPacket.ShiftUpOrDown ?? ShiftDir.Nothing;

                Throttle.SetThrottle(throttlePercentage);
                Brake.SetBrake(brakePercentage);
                Engine.CalculateRPM();
                GearBox.ShiftingGear(shiftUpOrDown);

                PowertrainPacket.GearStage = GearBox.GearStage;
                PowertrainPacket.RPM = Engine.Revolution;
                PowertrainPacket.Speed = (int)GearBox.Speed;

                MovementCalculator.Process(brakePercentage, wheelPercentage, GearBox);
            }
        }
    }

}
