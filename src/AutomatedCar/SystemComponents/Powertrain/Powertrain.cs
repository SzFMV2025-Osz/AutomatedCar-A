namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Engine;
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
            var aebPacket = virtualFunctionBus.AEBInputPacket;

            if (khp != null)
            {
                if (inputPacket == null)
                {
                    inputPacket = new InputDevicePacket();
                }

                // Check for AEB priority - emergency brake takes precedence
                int brakePercentage;
                int throttlePercentage;
                double wheelPercentage;
                ShiftDir shiftUpOrDown;

                if (aebPacket != null && aebPacket.BrakePercentage > 0)
                {
                    // AEB is active - use AEB brake, but allow wheel control
                    brakePercentage = aebPacket.BrakePercentage ?? 0;
                    throttlePercentage = 0; // No throttle during emergency braking
                    wheelPercentage = khp.WheelPercentage ?? 0;
                    shiftUpOrDown = ShiftDir.Nothing; // No gear shifting during emergency braking
                }
                else
                {
                    // Normal keyboard control
                    brakePercentage = khp.BrakePercentage ?? 0;
                    throttlePercentage = khp.ThrottlePercentage ?? 0;
                    wheelPercentage = khp.WheelPercentage ?? 0;
                    shiftUpOrDown = khp.ShiftUpOrDown ?? ShiftDir.Nothing;
                }

                // Billentyűzet értékek átemelése
                inputPacket.BrakePercentage = brakePercentage;
                inputPacket.ThrottlePercentage = throttlePercentage;
                inputPacket.WheelPercentage = wheelPercentage;
                inputPacket.ShiftUpOrDown = shiftUpOrDown;

                Throttle.SetThrottle(throttlePercentage);
                Brake.SetBrake(brakePercentage);
                Engine.CalculateRPM();
                GearBox.ShiftingGear(shiftUpOrDown);

                PowertrainPacket.GearStage = GearBox.GearStage;
                PowertrainPacket.RPM = Engine.Revolution;
                PowertrainPacket.Speed = (int)GearBox.Speed;

                MovementCalculator.Process(brakePercentage, (int)wheelPercentage, GearBox);
            }
        }
    }
}
