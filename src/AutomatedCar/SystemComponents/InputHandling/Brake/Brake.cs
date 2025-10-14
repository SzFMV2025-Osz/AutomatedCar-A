namespace AutomatedCar.SystemComponents.InputHandling.Brake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Brake :IBrake
    {
        private int brake;

        public int GetBrake()
        {
            return brake;
        }

        public void SetBrake(int value)
        {
            if (value >= 0 && value <=100)
            {
                brake = value;
            }
        }
    }
}
