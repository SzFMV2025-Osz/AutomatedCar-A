namespace AutomatedCar.Helpers.Gearbox_Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Enum for change between the gears (P,R,N,D).
    /// </summary>
    public enum GearShift
    {
        /// <summary>
        /// Shifting Down.
        /// </summary>
        Down = -1,

        /// <summary>
        /// Shifting Up.
        /// </summary>
        Up = 1
    }
}
