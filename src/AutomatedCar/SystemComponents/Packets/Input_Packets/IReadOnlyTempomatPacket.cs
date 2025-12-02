namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyTempomatPacket
    {
        int UserSetSpeed { get; set; }
        int LimitSpeed { get; set; }
        int CurrentSpeed { get; set; }
        bool IsEnabled { get; set; }
        int BrakePercentage { get; set; }
        int ThrottlePercentage { get; set; }
    }
}
