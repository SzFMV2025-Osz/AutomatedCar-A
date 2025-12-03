namespace AutomatedCar.SystemComponents
{
    using System;
    using Models;
    using Packets;
    using Packets.Input_Packets;
    using Helpers;

    /// <summary>
    /// Automatic Emergency Braking system component.
    /// </summary>
    public class AEB : SystemComponent
    {
        private const int EMERGENCY_BRAKE_THRESHOLD_KMH = 70;
        private const int MAX_BRAKE_PERCENTAGE = 100;

        private AEBInputPacket aebInputPacket;

        /// <summary>
        /// Initializes a new instance of the <see cref="AEB"/> class.
        /// </summary>
        /// <param name="virtualFunctionBus">The virtual function bus.</param>
        public AEB(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.aebInputPacket = new AEBInputPacket();
            this.virtualFunctionBus.RegisterComponent(this);
        }

        /// <summary>
        /// Processes the AEB logic every tick.
        /// </summary>
        public override void Process()
        {
            var radarPacket = this.virtualFunctionBus.RadarPacket;
            var powertrainPacket = this.virtualFunctionBus.PowertrainPacket;

            if (radarPacket == null || powertrainPacket == null)
            {
                return;
            }

            bool shouldActivate = this.ShouldActivateEmergencyBrake(radarPacket, powertrainPacket);

            if (shouldActivate)
            {
                this.ActivateEmergencyBrake();
            }
            else
            {
                this.DeactivateEmergencyBrake();
            }

            // Update the AEB input packet on the bus
            this.virtualFunctionBus.AEBInputPacket = this.aebInputPacket;
        }

        private bool ShouldActivateEmergencyBrake(IReadOnlyRadarPacket radarPacket, IReadOnlyPowertrainPacket powertrainPacket)
        {
            // Check if speed is above threshold (emergency brake for high speeds)
            if (powertrainPacket.Speed <= EMERGENCY_BRAKE_THRESHOLD_KMH)
            {
                return false;
            }

            // Check if there are relevant objects
            if (radarPacket.RelevantObjects.Count == 0)
            {
                return false;
            }

            // Get the closest object
            WorldObject closestObject = radarPacket.RelevantObjects[0];

            // Check if car is heading towards the object
            return this.IsHeadingTowardsObject(closestObject);
        }

        private bool IsHeadingTowardsObject(WorldObject worldObject)
        {
            AutomatedCar car = World.Instance.ControlledCar;

            // Calculate vector from car to object
            double dx = worldObject.X - car.X;
            double dy = worldObject.Y - car.Y;

            // Get car's forward direction vector
            double carRotationRad = car.Rotation * Math.PI / 180.0;
            double carDirX = Math.Cos(carRotationRad);
            double carDirY = Math.Sin(carRotationRad);

            // Calculate dot product to see if object is in front of car
            double dotProduct = (dx * carDirX) + (dy * carDirY);

            // If dot product is positive, object is in front of car
            return dotProduct > 0;
        }

        private void ActivateEmergencyBrake()
        {
            this.aebInputPacket.BrakePercentage = MAX_BRAKE_PERCENTAGE;
            this.aebInputPacket.WarningOver70kmph = true;
            this.aebInputPacket.WarningAvoidableCollision = true;
        }

        private void DeactivateEmergencyBrake()
        {
            this.aebInputPacket.BrakePercentage = 0;
            this.aebInputPacket.WarningOver70kmph = false;
            this.aebInputPacket.WarningAvoidableCollision = false;
        }
    }
}
