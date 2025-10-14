namespace AutomatedCar.SystemComponents.InputHandling.Throttle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Throttle : IThrottle
    {
        private int throttle;
        public int GetThrottle()
        {
            return throttle;
        }

        public void SetThrottle(int value)
        {
            if (value >= 0 && value <=100)
            {
                throttle = value;
            }
        }
    }
}
