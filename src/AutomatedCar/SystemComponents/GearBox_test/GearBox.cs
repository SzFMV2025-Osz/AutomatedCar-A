namespace AutomatedCar.SystemComponents.GearBox_test
{
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.GearShifter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GearBox : IGearBox // 7572rmp-nél akad ki.
    {
        private double[] gearRatios = { 0.004, 0.011, 0.016, 0.0225, 0.027, 0.035 }; // 0.05-el megy 328-at. (0.004, 0.009, 0.015, 0.0225, 0.025, 0.033)
        private int currentInsideGearStage = 0;
        private int nextLowRevolutionChangeValue = 1000;
        public float Speed { get; set; } // velocity = revolution * gearRation
        public Gear GearStage { get; private set; }
        public int CalculateGearSpeed(int revolution, int enginespeed)
        {
            switch (GearStage)
            {
                case Gear.R:
                    return ReverseCalculate(revolution, enginespeed);
                case Gear.N:
                    return NeutralCalculate(revolution, enginespeed);
                case Gear.D:
                    return DriveCalculate(revolution, enginespeed);
                default:
                    return 1000;
            }
        }

        private int ReverseCalculate(int revolution, int enginespeed)
        {
            revolution = ModifyRevolution(revolution, enginespeed);
            Speed = -((revolution - 1000) / 200);
            return revolution;
        }

        private int NeutralCalculate(int revolution, int enginespeed)
        {
            if (enginespeed > 0 && (revolution < enginespeed + 1000))
            {
                return CalculateRevolution(revolution, enginespeed);
            }

            return SlowsDownRevolution(revolution);
        }

        private int DriveCalculate(int revolution, int enginespeed)
        {
            if ((revolution > nextLowRevolutionChangeValue || currentInsideGearStage == 1) && (revolution <= 4000 || currentInsideGearStage == gearRatios.Length - 1))
            {
                revolution = ModifyRevolution(revolution, enginespeed);
            }
            else if (revolution <= nextLowRevolutionChangeValue)
            {
                // changing to lower gears
                --currentInsideGearStage;
                revolution = (int)(Speed / gearRatios[currentInsideGearStage]) + 1000;
                if (currentInsideGearStage - 1 > 0)
                {
                    // if not in first gear
                    nextLowRevolutionChangeValue = (int)(4000 * gearRatios[currentInsideGearStage - 1] / gearRatios[currentInsideGearStage]) - 500;
                }
                else
                {
                    // in a first gear
                    nextLowRevolutionChangeValue = 1000;
                }
            }
            else
            {
                ++currentInsideGearStage;
                revolution = (int)(Speed / gearRatios[currentInsideGearStage]) + 1000;
                nextLowRevolutionChangeValue = revolution - 500;
            }

            Speed = (int)((revolution - 1000) * gearRatios[currentInsideGearStage]);
            return revolution;
        }

        private int ModifyRevolution(int revolution, int enginespeed)
        {
            if (enginespeed > 0 && (revolution < enginespeed + 1000))
            {
                return CalculateRevolution(revolution, enginespeed);
            }
            else
            {
                if (Math.Abs(Speed) != (int)((revolution - 1000) * gearRatios[currentInsideGearStage])) // aka has break input
                {
                    revolution = (int)(Math.Abs(Speed) / gearRatios[currentInsideGearStage]) + 1000;
                }

                return SlowsDownRevolution(revolution);
            }
        }

        private int CalculateRevolution(int revolution, int enginespeed)
        {
            double throttenmultiply = (1 - ((double)revolution) / (enginespeed + 1000));
            return (int)(revolution + (enginespeed * throttenmultiply) * gearRatios[gearRatios.Length - 1 - currentInsideGearStage] / 1.5);
        }

        private int SlowsDownRevolution(int revolution)
        {
            int newRevolution = revolution - revolution / (GearStage == Gear.N ? 30 : 600);
            if (newRevolution < 1000)
            {
                return 1000;
            }
            else
            {
                return newRevolution;
            }
        }

        public void ShiftingGear(ShiftDir shift)
        {
            Gear nextGearStage = GearStage + ((int)shift);
            if ((int)shift != 0 && Enum.IsDefined(typeof(Gear), GearStage + ((int)shift)))
            {
                if (GearStage == Gear.R)
                {
                    if (nextGearStage == Gear.N)
                    {
                        GearStage = nextGearStage;
                        currentInsideGearStage = 0;
                    }
                    else if (Speed == 0)
                    {
                        GearStage = nextGearStage;
                    }

                    return;
                }
                else if (GearStage == Gear.N)
                {
                    if ((nextGearStage == Gear.R && Speed <= 0) || (nextGearStage == Gear.D && Speed >= 0))
                    {
                        GearStage = nextGearStage;
                        currentInsideGearStage = 1;
                    }
                }
                else // Park -> Reverse, Drive -> Neutral
                {
                    GearStage = nextGearStage;
                    if (GearStage == Gear.N)
                    {
                        currentInsideGearStage = 0;
                    }
                    else
                    {
                        currentInsideGearStage = 1;
                    }
                }
            }
        }

    }

}
