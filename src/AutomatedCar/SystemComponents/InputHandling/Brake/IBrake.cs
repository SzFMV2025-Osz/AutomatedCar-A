namespace AutomatedCar.SystemComponents.InputHandling.Brake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBrake
    {
        /// <summary>
        /// Returns the state of the brake pedal.
        /// </summary>
        /// <returns>A value between 0 and 100.</returns>
        public int GetBrake();

        /// <summary>
        /// Sets the brake pedal's value to an int between 0 and 100.
        /// </summary>
        /// <param name="value">Value between 0 and 100.</param>
        public void SetBrake(int value);
    }
}
