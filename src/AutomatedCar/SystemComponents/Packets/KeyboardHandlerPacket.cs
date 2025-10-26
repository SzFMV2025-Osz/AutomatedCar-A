﻿namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.GearBox_test;
    using AutomatedCar.SystemComponents.GearShifter;
    using AutomatedCar.SystemComponents.Packets.Input_Packets;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class KeyboardHandlerPacket:InputDevicePacket,IReadOnlyKeyboardHandlerPacket
    {

        public KeyboardHandlerPacket()
        {
            this.throttlePercentage = 0;
            this.brakePercentage = 0;
            this.wheelPercentage = 0;
            this.shiftUpOrDown = 0;
        }

        public override int? BrakePercentage
        {
            get => this.brakePercentage;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.brakePercentage, value);
            }
        }

        public override int? ThrottlePercentage
        {
            get => this.throttlePercentage;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.throttlePercentage, value);
            }
        }

        public override double? WheelPercentage
        {
            get => this.wheelPercentage;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.wheelPercentage, value);
            }
        }

        public override ShiftDir? ShiftUpOrDown
        {
            get => this.shiftUpOrDown;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.shiftUpOrDown, value);
            }
        }

    }
}
