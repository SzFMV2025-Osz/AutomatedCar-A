namespace AutomatedCar.SystemComponents.ACC
{
    using System;
    using System.Linq;
    using AutomatedCar.Helpers;
    using AutomatedCar.Models.NPC;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.Input_Packets;

    public class AdaptiveCruiseControl : SystemComponent
    {
        private const int MinSpeedKmh = 30;
        private const int MaxSpeedKmh = 160;

        private readonly AccPacket accPacket;

        private bool isActive;
        private int targetSpeedKmh;
        private double timeGapSec = 1.0;

        private readonly double[] timeGapValues = new[] { 0.8, 1.0, 1.2, 1.4 };

        public AdaptiveCruiseControl(VirtualFunctionBus vfb)
            : base(vfb)
        {
            this.accPacket = new AccPacket();
            this.virtualFunctionBus.AccPacket = this.accPacket;
        }

        public override void Process()
        {
            var khp = this.virtualFunctionBus.KeyboardHandlerPacket;
            if (khp == null)
            {
                return;
            }

            HandleButtons(khp);
            HandleDeactivation(khp);

            if (!this.isActive)
            {
                this.accPacket.IsActive = false;
                this.accPacket.ThrottlePercentage = 0;
                this.accPacket.BrakePercentage = 0;
                return;
            }

            int currentSpeedKmh = this.virtualFunctionBus.PowertrainPacket?.Speed ?? 0;
            double currentSpeedMps = currentSpeedKmh / 3.6;

            int speedLimit = GetSpeedLimitFromCamera();
            int targetByUserAndLimit = Math.Min(this.targetSpeedKmh, speedLimit);

            int finalTarget = ApplyFrontCarLogic(targetByUserAndLimit, currentSpeedMps);

            ApplySpeedControl(finalTarget, currentSpeedKmh);
        }

        private void HandleButtons(IReadOnlyKeyboardHandlerPacket khp)
        {

            if (khp.AccToggle == true)
            {
                if (!this.isActive)
                {
                    int currentSpeed = this.virtualFunctionBus.PowertrainPacket?.Speed ?? 0;
                    if (currentSpeed >= MinSpeedKmh)
                    {
                        this.targetSpeedKmh = Math.Max(MinSpeedKmh, currentSpeed);
                        this.isActive = true;
                    }
                }
                else
                {
                    this.isActive = false;
                }
            }

            if (!this.isActive)
            {
                return;
            }

            if (khp.AccSpeedPlus == true)
            {
                this.targetSpeedKmh = Math.Min(MaxSpeedKmh, this.targetSpeedKmh + 10);
            }

            if (khp.AccSpeedMinus == true)
            {
                this.targetSpeedKmh = Math.Max(MinSpeedKmh, this.targetSpeedKmh - 10);
            }

            if (khp.AccTimeGap == true)
            {
                int idx = Array.IndexOf(this.timeGapValues, this.timeGapSec);
                idx = (idx + 1) % this.timeGapValues.Length;
                this.timeGapSec = this.timeGapValues[idx];
            }

            this.accPacket.IsActive = this.isActive;
            this.accPacket.TargetSpeedKmh = this.targetSpeedKmh;
            this.accPacket.TimeGapSec = this.timeGapSec;
        }

        private void HandleDeactivation(IReadOnlyKeyboardHandlerPacket khp)
        {
            if (khp.BrakePercentage.HasValue && khp.BrakePercentage.Value > 0)
            {
                this.isActive = false;
            }

            // TODO: AEB jel integrálása, ha lesz külön packet rá
        }

        private int GetSpeedLimitFromCamera()
        {
            // Amíg a táblafelismerő nincs kész, ne korlátozzon semmit.
            return MaxSpeedKmh;
        }
        private int ApplyFrontCarLogic(int targetSpeed, double ownSpeedMps)
        {
            var radar = this.virtualFunctionBus.RadarPacket;
            if (radar == null || radar.RelevantObjects == null || !radar.RelevantObjects.Any())
            {
                return targetSpeed;
            }

            var car = World.Instance.ControlledCar;

            var frontCar = radar.RelevantObjects
               .Where(o => o is NPCCar)
               .Where(o => o != car)
               .Where(o => o.Y < car.Y)
               .OrderBy(o => GeometryHelper.DistanceBetweenObjects(o, car))
               .FirstOrDefault();

            if (frontCar == null)
            {
                return targetSpeed;
            }

            double distance = GeometryHelper.DistanceBetweenObjects(frontCar, car);
            double required = ownSpeedMps * this.timeGapSec;

            if (distance < required * 1.1)
            {
                return Math.Min(targetSpeed, (int)(ownSpeedMps * 3.6) - 5);
            }

            return targetSpeed;
        }

        private void ApplySpeedControl(int targetSpeedKmh, int currentSpeedKmh)
        {
            double error = targetSpeedKmh - currentSpeedKmh;

            const double kp = 0.05;
            const double deadband = 1.0;

            int throttle = 0;
            int brake = 0;

            if (error > deadband)
            {
                throttle = (int)Math.Clamp(kp * error * 100.0, 0, 100);
            }
            else if (error < -deadband)
            {
                brake = (int)Math.Clamp(kp * -error * 100.0, 0, 100);
            }

            this.accPacket.ThrottlePercentage = throttle;
            this.accPacket.BrakePercentage = brake;
        }
    }
}
