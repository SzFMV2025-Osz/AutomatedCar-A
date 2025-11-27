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

        /// <summary>
        /// Gets The value of the Throttle.
        /// </summary>
        /// <returns>int.</returns>
        public int GetThrottle()
        {
            return this.throttle;
        }

        /// <summary>
        /// Sets the Throttle value.
        /// </summary>
        /// <param name="value">Between 0-100.</param>
        public void SetThrottle(int value)
        {
            if (value >= 0 && value <= 100)
            {
                this.throttle = value;
            }
        }
    }
}
