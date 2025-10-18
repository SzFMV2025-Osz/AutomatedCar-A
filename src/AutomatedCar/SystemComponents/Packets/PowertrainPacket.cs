namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.GearShifter;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class PowertrainPacket : ReactiveObject, IReadOnlyPowertrainPacket
    {
        private Vector2 movementVector;
        private double rotation;
        private int rpm;
        private int speed;
        private Gear gearStage;

        public Vector2 MovementVector
        {
            get => movementVector;
            set => this.RaiseAndSetIfChanged(ref movementVector, value);
        }

        public double Rotation
        {
            get => rotation;
            set => this.RaiseAndSetIfChanged(ref rotation, value);
        }

        public int RPM
        {
            get => rpm;
            set => this.RaiseAndSetIfChanged(ref rpm, value);
        }

        public int Speed
        {
            get => speed;
            set => this.RaiseAndSetIfChanged(ref speed, value);
        }

        public Gear GearStage
        {
            get => gearStage;
            set => this.RaiseAndSetIfChanged(ref gearStage, value);
        }
    }
}
