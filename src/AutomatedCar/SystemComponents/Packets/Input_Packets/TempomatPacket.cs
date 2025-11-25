namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TempomatPacket 
    {
        public int UserSetSpeed { get; set; }
        public int LimitSpeed { get; set; }
        public int CurrentSpeed { get; set; }
        public bool IsEnabled { get; set; }
        public int BrakePercentage { get; set; }
        public int ThrottlePercentage { get; set; }
    }
}
