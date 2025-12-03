namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.GearShifter;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.Input_Packets;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class VirtualFunctionBus : GameBase, INotifyPropertyChanged
    {
        private List<SystemComponent> components = new List<SystemComponent>();

        public event PropertyChangedEventHandler PropertyChanged;

        public IReadOnlyDummyPacket DummyPacket { get;}

        internal DummyPacket WritableDummyPacket { get; }

        private IReadOnlyRadarPacket radarPacket;

        public IReadOnlyTempomatPacket TempomatPacket { get; set; }
        public IReadOnlyRelevantObjects RelevantObjectsPacket { get; set; }

        public VirtualFunctionBus()
        { 
         this.WritableDummyPacket = new DummyPacket();

         this.DummyPacket=this.WritableDummyPacket;
        }

        public IReadOnlyKeyboardHandlerPacket KeyboardHandlerPacket { get; set; }

        public IReadOnlyPowertrainPacket PowertrainPacket { get; set; }

        public IReadOnlyRadarPacket RadarPacket
        {
            get => this.radarPacket;
            set
            {
                this.radarPacket = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RadarPacket)));
            }
        }

        private IReadOnlyCameraPacket cameraPacket;

        public IReadOnlyCameraPacket CameraPacket
        {
            get => this.cameraPacket;
            set
            {
                this.cameraPacket = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CameraPacket)));
            }
        }

        private IReadOnlyLKAPacket  lkaPacket;

        public IReadOnlyLKAPacket LKAPacket
        {
            get => this.lkaPacket;  
            set
            {
                this.lkaPacket = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LKAPacket)));
            }
        }


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