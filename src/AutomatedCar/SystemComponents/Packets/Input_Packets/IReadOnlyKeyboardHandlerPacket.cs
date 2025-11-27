namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyKeyboardHandlerPacket : IReadOnlyInputDevicePacket
    {
        bool? AccToggle { get; }
        bool? AccSpeedPlus { get; }
        bool? AccSpeedMinus { get; }
        bool? AccTimeGap { get; }
    }
}
