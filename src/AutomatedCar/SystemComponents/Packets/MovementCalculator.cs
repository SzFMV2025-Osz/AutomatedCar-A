namespace AutomatedCar.SystemComponents
{
    using System.Numerics;
    using AutomatedCar.SystemComponents.GearBox_test;
    using System;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.GearShifter;
    using AutomatedCar.SystemComponents.InputHandling.Wheel;

    public class MovementCalculator
    {
        const float BRAKING = 0.03f;
        const float ROLLING_RESISTANCE = 0.003f;
        const double WHEEL_BASE = 2.6;

        private Vector2 aggregatedVelocity;
        private AutomatedCar car;

        public MovementCalculator(AutomatedCar car)
        {
            this.car = car;
        }

        public void Process(int brakePercentage, int wheelPercentage, IGearBox gearBox)
        {
            float brakingFroce = brakePercentage * BRAKING;
            float rollingResistance = (float)gearBox.Speed * ROLLING_RESISTANCE;
            float aggregatedForces = brakingFroce + rollingResistance;
            if (gearBox.GearStage == Gear.N)
            {
                ResistanceMethod(gearBox, Math.Abs(aggregatedForces));
            }
            else
            {
                ResistanceMethod(gearBox, brakingFroce);
            }

            double radius = WHEEL_BASE / Math.Sin(Wheel.ToDegrees(wheelPercentage) * Math.PI / 180);
            this.car.Rotation += gearBox.Speed / radius / 5;

            float rotationInRadian = -(float)(car.Rotation * Math.PI / 180);
            Vector2 directionVector = new Vector2((float)Math.Sin(rotationInRadian), (float)Math.Cos(rotationInRadian));
            Vector2 velocity = directionVector * gearBox.Speed;

            velocity = ConvertVelocity(velocity / 5);

            this.car.X += (int)velocity.X;
            this.car.Y += (int)velocity.Y;
        }

        private static void ResistanceMethod(IGearBox gearBox, float forces)
        {
            if (gearBox.Speed > 0)
            {
                if (gearBox.Speed - forces < 0)
                {
                    gearBox.Speed = 0;
                }
                else
                {
                    gearBox.Speed = (int)(gearBox.Speed - forces);
                }
            }
            else if (gearBox.Speed < 0)
            {
                if (gearBox.Speed - forces > 0)
                {
                    gearBox.Speed = 0;
                }
                else
                {
                    gearBox.Speed = (int)(gearBox.Speed + forces);
                }
            }
        }

        public Vector2 ConvertVelocity(Vector2 velocity)
        {
            Vector2 convertedVelocity = new Vector2();
            this.aggregatedVelocity += velocity;

            if (Math.Abs(this.aggregatedVelocity.X) >= 1)
            {
                convertedVelocity.X = (int)Math.Floor(this.aggregatedVelocity.X);
                this.aggregatedVelocity.X = this.aggregatedVelocity.X % 1;
            }
            if (Math.Abs(this.aggregatedVelocity.Y) >= 1)
            {
                convertedVelocity.Y = (int)Math.Floor(this.aggregatedVelocity.Y);
                this.aggregatedVelocity.Y = this.aggregatedVelocity.Y % 1;
            }

            return -convertedVelocity;
        }
    }
}
