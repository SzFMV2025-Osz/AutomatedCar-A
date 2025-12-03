using System;
using System.Linq;
using System.Numerics;
using AutomatedCar.SystemComponents.InputHandling.Brake;
using AutomatedCar.SystemComponents.Packets;
using AutomatedCar.Helpers;
using AutomatedCar.Models;
using AutomatedCar.Models.NPC;

namespace AutomatedCar.SystemComponents
{
    /// <summary>
    /// Emergency brake system component.
    /// </summary>
    public class EmergencyBrake : SystemComponent
    {
        private readonly IBrake brake;
        private double previousDistance = double.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyBrake"/> class.
        /// </summary>
        /// <param name="virtualFunctionBus">The virtual function bus.</param>
        /// <param name="brake">The brake interface.</param>
        public EmergencyBrake(VirtualFunctionBus virtualFunctionBus, IBrake brake)
            : base(virtualFunctionBus)
        {
            this.brake = brake;
        }

        /// <summary>
        /// Process method to check for emergency braking conditions.
        /// </summary>
        public override void Process()
        {
            var radarPacket = this.virtualFunctionBus.RadarPacket;
            if (radarPacket == null || radarPacket.RelevantObjects == null || !radarPacket.RelevantObjects.Any())
            {
                this.brake.SetBrake(0);
                return;
            }

            var powertrainPacket = this.virtualFunctionBus.PowertrainPacket;
            if (powertrainPacket == null)
            {
                this.brake.SetBrake(0);
                return;
            }

            int speed = powertrainPacket.Speed;
            if (speed >= 70)
            {
                this.brake.SetBrake(0);
                return;
            }

            var car = World.Instance.ControlledCar;
            var carVelocity = powertrainPacket.MovementVector;

            var closestObject = radarPacket.RelevantObjects
                .Where(o => o.Collideable)
                .OrderBy(o => GeometryHelper.DistanceBetweenObjects(o, car))
                .FirstOrDefault();

            if (closestObject == null)
            {
                this.brake.SetBrake(0);
                this.previousDistance = double.MaxValue;
                return;
            }

            double distance = GeometryHelper.DistanceBetweenObjects(closestObject, car);

            // Calculate relative velocity
            Vector2 objectVelocity = GetObjectVelocity(closestObject);
            Vector2 relativeVelocity = carVelocity - objectVelocity;

            // Vector from car to object
            Vector2 toObject = new Vector2(closestObject.X - car.X, closestObject.Y - car.Y);

            // Check if relative velocity is towards the object (approaching)
            float dot = Vector2.Dot(Vector2.Normalize(relativeVelocity), Vector2.Normalize(toObject));

            if (dot > -0.5f || relativeVelocity.Length() < 0.1f) // Not approaching or too slow
            {
                this.brake.SetBrake(0);
                this.previousDistance = distance;
                return;
            }

            // If distance is getting smaller, apply ramping brake
            if (distance < this.previousDistance)
            {
                // Ramping brake intensity based on inverse distance and relative speed
                double relativeSpeed = relativeVelocity.Length();
                int brakeIntensity = Math.Min(100, (int)(500 / (distance + 1) * (relativeSpeed / 10)));
                this.brake.SetBrake(brakeIntensity);
            }
            else
            {
                this.brake.SetBrake(0);
            }

            this.previousDistance = distance;
        }

        private Vector2 GetObjectVelocity(WorldObject obj)
        {
            if (obj is NPCCar npcCar)
            {
                // Assuming NPCCar has speed in m/s, convert to similar units
                double speedMps = npcCar.Speed; // Assuming Speed is in m/s
                double rotationRad = npcCar.Rotation * Math.PI / 180;
                return new Vector2((float)(speedMps * Math.Sin(rotationRad)), (float)(speedMps * Math.Cos(rotationRad)));
            }
            else
            {
                // Static object
                return Vector2.Zero;
            }
        }
    }
}
