namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AEBInputPacket : InputDevicePacket, IReadOnlyAEBInputPacket
    {
        private bool warningOver70kmph;
        private bool warningAvoidableCollision;

        public bool WarningOver70kmph
        {
            get => this.warningOver70kmph;
            set => this.RaiseAndSetIfChanged(ref this.warningOver70kmph, value);
        }

        public bool WarningAvoidableCollision
        {
            get => this.warningAvoidableCollision;
            set => this.RaiseAndSetIfChanged(ref this.warningAvoidableCollision, value);
        }
    }
}
