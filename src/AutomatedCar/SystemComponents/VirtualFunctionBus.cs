namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.GearShifter;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.Input_Packets;
    using System.Collections.Generic;

    public class VirtualFunctionBus : GameBase
    {
        private List<SystemComponent> components = new List<SystemComponent>();

        public IReadOnlyDummyPacket DummyPacket { get;}

        internal DummyPacket WritableDummyPacket { get; }

        public IReadOnlyRadarPacket RadarPacket { get; set; }

        public IReadOnlyCameraPacket CameraPacket { get; set; }
        public IReadOnlyAccPacket AccPacket { get; set; }
        public IReadOnlyTempomatPacket TempomatPacket { get; set; }
        public IReadOnlyRelevantObjects RelevantObjectsPacket { get; set; }

        public VirtualFunctionBus()
        { 
         this.WritableDummyPacket = new DummyPacket();

         this.DummyPacket=this.WritableDummyPacket;
        }

        public IReadOnlyKeyboardHandlerPacket KeyboardHandlerPacket { get; set; }

        public IReadOnlyPowertrainPacket PowertrainPacket { get; set; }

        public void RegisterComponent(SystemComponent component)
        {
            this.components.Add(component);
        }

        protected override void Tick()
        {
            foreach (SystemComponent component in this.components)
            {
                component.Process();
            }
        }
    }
}