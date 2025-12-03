namespace AutomatedCar.SystemComponents.Packets
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
        public bool? AccToggle { get; set; }
        public bool? AccSpeedPlus { get; set; }
        public bool? AccSpeedMinus { get; set; }
        public bool? AccTimeGap { get; set; }

        private bool lkaKey;
        public KeyboardHandlerPacket()
        {
            this.throttlePercentage = 0;
            this.brakePercentage = 0;
            this.wheelPercentage = 0;
            this.shiftUpOrDown = 0;
            this.lkaKey = false;
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

        public bool LKAKey
        {
            get=> this.lkaKey;
            set => this.RaiseAndSetIfChanged(ref this.lkaKey, value);
        }
    }
}
