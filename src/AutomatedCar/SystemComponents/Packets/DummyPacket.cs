namespace AutomatedCar.SystemComponents.Packets
{
    using System.ComponentModel;
    using ReactiveUI;

    public class DummyPacket : ReactiveObject, IReadOnlyDummyPacket
    {
        private int distanceX;
        private int distanceY;
        private bool isColliding;

        public int DistanceX
        {
            get => this.distanceX;
            set => this.RaiseAndSetIfChanged(ref this.distanceX, value);
        }

        public int DistanceY
        {
            get => this.distanceY;
            set => this.RaiseAndSetIfChanged(ref this.distanceY, value);
        }

        public bool IsColliding
        {
            get => this.isColliding;
            set => this.RaiseAndSetIfChanged(ref this.isColliding, value);
        }
    }
}