namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.GearShifter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyPowertrainPacket
    {
        public Vector2 MovementVector { get; }

        public double Rotation { get; }

        public int RPM { get; }

        public int Speed { get; }

        public Gear GearStage { get; }
    }
}
