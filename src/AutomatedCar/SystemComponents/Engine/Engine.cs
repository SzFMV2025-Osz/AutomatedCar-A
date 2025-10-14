namespace AutomatedCar.SystemComponents.Engine
{
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
        private IGearShifter gearBox;
        private IThrottle throttle;

        public int Revolution
        {
            get => revolution;
            set => revolution = value;
        }

        public Engine(IGearShifter gearShifter, IThrottle throttle)
        {
            this.revolution = 1000;
            this.gearBox = gearShifter;
            this.throttle = throttle;
        }

        public void CalculateRPM()
        {
            //To be continued
        }
    }
}
