namespace AutomatedCar.SystemComponents
{
    using System.Collections.Generic;
    using AutomatedCar.SystemComponents.GearShifter;
    using AutomatedCar.SystemComponents.Packets;

    public class VirtualFunctionBus : GameBase
    {
        private List<SystemComponent> components = new List<SystemComponent>();

        public IReadOnlyDummyPacket DummyPacket { get; set; }

        public IGearShifter GearShifter { get; set; }

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