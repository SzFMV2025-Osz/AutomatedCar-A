namespace AutomatedCar.SystemComponents.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IEngine
    {
        public int Revolution { get; set; }

        /// <summary>
        /// Calculate the EngineRPM.
        /// </summary>
        public void CalculateRPM();

    }
}
