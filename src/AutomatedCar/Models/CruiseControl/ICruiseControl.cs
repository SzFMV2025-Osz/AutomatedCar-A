namespace AutomatedCar.Models.CruiseControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a cruise control system interface for managing vehicle speed.
    /// </summary>
    /// <remarks>This interface defines the contract for implementing a cruise control system,  allowing for
    /// the management of vehicle speed. Implementations should provide  mechanisms to set, adjust, and maintain a
    /// desired speed.</remarks>
    public interface ICruiseControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is active.
        /// </summary>
        bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the desired speed of the vehicle in kilometers per hour.
        /// </summary>
        int DesiredSpeed { get; set; }
        /// <summary>
        /// Sets the speed of the object to the specified initial value.
        /// </summary>
        /// <param name="initialSpeed">The initial speed to set, in units per second. Must be non-negative.</param>
        /// <returns>The actual speed set, which may be adjusted based on internal constraints.</returns>
        int SetSpeed(int initialSpeed);

        /// <summary>
        /// Initiates the process or operation associated with this instance.
        /// </summary>
        /// <remarks>This method should be called to begin the primary functionality of the instance.
        /// Ensure that all necessary preconditions are met before invoking this method.</remarks>
        void Start();
    }
}
