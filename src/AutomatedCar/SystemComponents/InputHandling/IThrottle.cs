namespace AutomatedCar.SystemComponents.InputHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IThrottle
    {
        /// <summary>
        /// Returns the state of the throttle pedal.
        /// </summary>
        /// <returns>A value between 0 and 100.</returns>
        public int GetThrottle();

        /// <summary>
        /// Sets the value of the Throttle.
        /// </summary>
        /// <param name="value">value between 0 and 100.</param>
        public void SetThrottle(int value);
    }
}
