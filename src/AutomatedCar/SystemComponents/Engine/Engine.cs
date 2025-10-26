namespace AutomatedCar.SystemComponents.Enginee
{
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.GearShifter;
    using AutomatedCar.SystemComponents.InputHandling.Throttle;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Engine :IEngine
    {
        private int revolution;
        private IGearBox gearBox;
        private IThrottle throttle;

        public int Revolution
        {
            get => revolution;
            set => revolution = value;
        }

        public Engine(IGearBox gearShifter, IThrottle throttle)
        {
            this.revolution = 1000;
            this.gearBox = gearShifter;
            this.throttle = throttle;
        }

        public void CalculateRPM()
        {
            Revolution = gearBox.CalculateGearSpeed(Revolution, throttle.GetThrottle() * 60);
        }
    }
}
