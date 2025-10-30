namespace AutomatedCar.SystemComponents.Enginee
{
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.InputHandling.Throttle;

    public class Engine :IEngine
    {
        private int revolution;
        private IGearBox gearBox;
        private IThrottle throttle;

        public int Revolution
        {
            get => this.revolution;
            set => this.revolution = value;
        }

        public Engine(IGearBox gearShifter, IThrottle throttle)
        {
            this.revolution = 1000;
            this.gearBox = gearShifter;
            this.throttle = throttle;
        }

        public void CalculateRPM()
        {
            Revolution = this.gearBox.CalculateGearSpeed(Revolution, throttle.GetThrottle() * 70);
        }
    }
}
