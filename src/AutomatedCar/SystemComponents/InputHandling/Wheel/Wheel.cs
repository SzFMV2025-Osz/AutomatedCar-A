namespace AutomatedCar.SystemComponents.InputHandling.Wheel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Wheel : IWheel
    {
        private double angleAsDegree;
        public double AngleAsDegree
        {
            get
            {
                return this.angleAsDegree;
            }
            set
            {
                if (value >= -60 && value <= 60)
                {
                    this.angleAsDegree = value;
                }
            }
        }
    }
}
