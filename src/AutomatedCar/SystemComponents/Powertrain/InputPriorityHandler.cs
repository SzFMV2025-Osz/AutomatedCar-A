namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.SystemComponents.GearBox_test;
    using AutomatedCar.SystemComponents.Packets.Input_Packets;

    public class InputPriorityHandler
    {
        private InputDevicePacket inputPacket = new InputDevicePacket();

        public InputDevicePacket GetInputs(VirtualFunctionBus vfb)
        {
            this.inputPacket.BrakePercentage = PriorityHandler<int?>(
                vfb.KeyboardHandlerPacket.BrakePercentage);

            this.inputPacket.WheelPercentage = PriorityHandler<double?>(
                vfb.KeyboardHandlerPacket.WheelPercentage);

            this.inputPacket.ThrottlePercentage = PriorityHandler<int?>(
                vfb.KeyboardHandlerPacket.ThrottlePercentage);

            this.inputPacket.ShiftUpOrDown = PriorityHandler<ShiftDir?>(
                vfb.KeyboardHandlerPacket.ShiftUpOrDown);

            return this.inputPacket;
        }

        private static T PriorityHandler<T>(T prio1)
        {
            if (prio1 != null) { return prio1; }

            return prio1;
        }
    }
}
