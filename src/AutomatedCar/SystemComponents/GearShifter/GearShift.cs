namespace AutomatedCar.SystemComponents.GearShifter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Avalonia.Input;

    public class GearShift : SystemComponent, IGearShifter
    {
        public Gear CurrentGear { get; set; } = Gear.P;

        private int driveGear = 1;

        public int CurrentGearNumber => CurrentGear == Gear.D ? driveGear : CurrentGear == Gear.R ? -1 : 0;

        public double SpeedKph { get; private set; } = 0.0;

        public double RPM { get; private set; } = 900.0;

        private readonly double[] gearRatios = { 0.0, 4.17, 2.34, 1.52, 1.14, 0.87, 0.69 };
        private readonly double finalDrive = 3.42;
        private readonly double wheelRadius = 0.34; // m
        private readonly double mass = 1500.0;
        private readonly double transmissionEfficiency = 0.9;

        private readonly double dragCoefficient = 0.30;
        private readonly double frontalArea = 2.2;
        private readonly double rollingResistanceCoeff = 0.015;

        private readonly double idleRpm = 900.0;
        private readonly double redlineRpm = 6500.0;
        private readonly double maxTorque = 320.0;
        private readonly double peakTorqueRpm = 4000.0;

        private readonly int maxDriveGear = 6;
        private readonly double upshiftRpm = 4200.0;
        private readonly double downshiftRpm = 1500.0;

        private readonly double shiftDuration = 0.5;
        private bool isShifting = false;
        private double shiftTimer = 0.0;
        private int pendingGear = -1;

        private static double Lerp(double a, double b, double t) => a + ((b - a) * Math.Clamp(t, 0.0, 1.0));

        public GearShift(VirtualFunctionBus bus)
        : base(bus)
        {

        }

        public override void Process()
        {
            double throttle = 0.5;
            double deltaTime = 1.0 / 60.0;

            this.Update(throttle, deltaTime);

            //this.virtualFunctionBus.GearShifter = this; Ideiglenesen kikerült a vfb-ból mert jelenleg másik osztállyal tesztelek
        }

        public void Shift(Key key)
        {
            if (key == Key.E)
            {
                this.ShiftUp();
            }
            else if (key == Key.Q)
            {
                this.ShiftDown();
            }
        }

        public void ShiftUp()
        {
            if (this.CurrentGear == Gear.D && this.driveGear < this.maxDriveGear && !this.isShifting)
            {
                this.StartShift(this.driveGear + 1);
            }
        }

        public void ShiftDown()
        {
            if (this.CurrentGear == Gear.D && this.driveGear > 1 && !this.isShifting)
            {
                this.StartShift(this.driveGear - 1);
            }
        }

        private void StartShift(int newGear)
        {
            this.isShifting = true;
            this.shiftTimer = 0.0;
            this.pendingGear = newGear;
        }

        private void Update(double throttle, double deltaTime)
        {
            if (this.isShifting)
            {
                this.shiftTimer += deltaTime;
                if (this.shiftTimer >= this.shiftDuration)
                {
                    this.isShifting = false;
                    if (this.pendingGear > 0)
                    {
                        this.driveGear = this.pendingGear; 
                    }

                    this.pendingGear = -1;
                }
            }

            if (this.CurrentGear == Gear.P)
            {
                this.SpeedKph = 0;
                this.RPM = Lerp(this.RPM, this.idleRpm + (throttle * 2000.0), 5.0 * deltaTime);
                return;
            }

            double speedMps = this.SpeedKph / 3.6;
            double wheelRpm = (speedMps * 60.0) / (2.0 * Math.PI * this.wheelRadius);

            if (this.CurrentGear == Gear.N)
            {
                this.RPM = Lerp(this.RPM, this.idleRpm + (throttle * 3000.0), 10.0 * deltaTime);
            }
            else if (this.CurrentGear == Gear.R)
            {
                double reverseRatio = 3.5 * this.finalDrive;
                double fromWheels = Math.Max(this.idleRpm, wheelRpm * reverseRatio);
                if (speedMps < 0.5 && throttle > 0.05)
                {
                    fromWheels = Math.Max(fromWheels, this.idleRpm + (throttle * 2000.0));
                }

                this.RPM = Lerp(this.RPM, fromWheels, 10.0 * deltaTime);
            }
            else if (this.CurrentGear == Gear.D)
            {
                double gearRatio = this.gearRatios[this.driveGear] * this.finalDrive;
                double rpmFromWheels = wheelRpm * gearRatio;
                double clutchSlipRpm = this.idleRpm + (throttle * 2000.0);
                double targetRpm = Math.Max(rpmFromWheels, clutchSlipRpm);
                this.RPM = Math.Max(this.idleRpm, Lerp(this.RPM, targetRpm, 8.0 * deltaTime));
            }

            this.RPM = Math.Clamp(this.RPM, this.idleRpm, this.redlineRpm);

            double engineTorque = this.EngineTorqueAtRpM(this.RPM) * throttle;
            double wheelForce = 0.0;

            if (!this.isShifting)
            {
                if (this.CurrentGear == Gear.D && this.driveGear >= 1)
                {
                    double overallRatio = this.gearRatios[this.driveGear] * this.finalDrive;
                    double wheelTorque = engineTorque * overallRatio * this.transmissionEfficiency;
                    wheelForce = wheelTorque / this.wheelRadius;
                }
                else if (this.CurrentGear == Gear.R)
                {
                    double overallRatio = 3.5 * this.finalDrive;
                    double wheelTorque = engineTorque * overallRatio * this.transmissionEfficiency;
                    wheelForce = -wheelTorque / this.wheelRadius;
                }
            }

            double dragForce = 0.5 * 1.225 * this.dragCoefficient * this.frontalArea * speedMps * speedMps;
            double rollingForce = this.rollingResistanceCoeff * this.mass * 9.81;

            double netForce = wheelForce - dragForce - (rollingForce * Math.Sign(speedMps));
            double acceleration = netForce / this.mass;

            speedMps += acceleration * deltaTime;

            if (this.CurrentGear == Gear.D && speedMps < 0)
            {
                speedMps = 0;
            }
            else if (this.CurrentGear == Gear.P)
            {
                speedMps = 0;
            }

            this.SpeedKph = Math.Max(0, speedMps * 3.6);

            if (!this.isShifting && this.CurrentGear == Gear.D)
            {
                if (this.RPM > this.upshiftRpm && this.driveGear < this.maxDriveGear)
                {
                    this.StartShift(this.driveGear + 1);
                }
                else if (this.RPM < this.downshiftRpm && this.driveGear > 1)
                {
                    this.StartShift(this.driveGear - 1);
                }
            }
        }

        private double EngineTorqueAtRpM(double rpm)
        {
            double x = (rpm - this.peakTorqueRpm) / this.peakTorqueRpm;
            double torque = this.maxTorque * (1.0 - (x * x));
            return Math.Max(0.0, torque);
        }
    }
}