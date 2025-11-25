namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Packets.Input_Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Tempomat : SystemComponent
    {
        VirtualFunctionBus virtualFunctionBus;
        public int UserSetSpeed { get; set; }
        public int LimitSpeed { get; set; }
        private TempomatPacket tempomatPacket;
        public int CurrentSpeed { get; set; }
        public bool IsEnabled { get; set; }
        public int BrakePercentage { get; set; }
        public int ThrottlePercentage { get; set; }
        private const int minimumSpeed = 30;
        private const int maximumSpeed = 160;
        private const int speedChangeInterval = 10;
        int GoalSpeed
        {
            get  ; set;
        }
        public Tempomat(VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            
        }

        public override void Process()
        {
            throw new NotImplementedException();
        }

        private int SpeedValid(int speed)
        {
            return Math.Min(Math.Max(speed, minimumSpeed), maximumSpeed);
        }

        private int GetGoalSpeed()
        {
            var temp = SpeedValid(Math.Min(UserSetSpeed, LimitSpeed));
            if (temp == null)
            {
                return maximumSpeed;
            }
            return Math.Min(SpeedValid(UserSetSpeed), LimitSpeed);
        }
    }
}
