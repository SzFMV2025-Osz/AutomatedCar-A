namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using AutomatedCar.SystemComponents.GearShifter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyInputDevicePacket
    {
        public int? BrakePercentage { get; }

        public int? ThrottlePercentage { get; }

        public double? WheelPercentage { get; }

    }
}
