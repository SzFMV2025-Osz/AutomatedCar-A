namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using AutomatedCar.SystemComponents.GearShifter;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InputDevicePacket : ReactiveObject, IReadOnlyInputDevicePacket
    {
        protected int? brakePercentage;

        protected int? throttlePercentage;

        protected double? wheelPercentage;

        protected GearShift shiftUpOrDown;

        public virtual int? BrakePercentage
        {
            get => brakePercentage;
            set => this.RaiseAndSetIfChanged(ref brakePercentage, value);
        }

        public virtual int? ThrottlePercentage
        {
            get => throttlePercentage;
            set => this.RaiseAndSetIfChanged(ref throttlePercentage, value);
        }

        public virtual double? WheelPercentage
        {
            get => wheelPercentage;
            set => this.RaiseAndSetIfChanged(ref wheelPercentage, value);
        }

        public GearShift ShiftUpOrDown
        {
            get => shiftUpOrDown;
            set => this.RaiseAndSetIfChanged(ref shiftUpOrDown, value);
        }
    }
}
