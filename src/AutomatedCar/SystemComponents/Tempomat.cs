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

        private bool IsSpeedValid(int speed)
        {
            if (speed > maximumSpeed) { return false; }
            else if (speed < minimumSpeed) { return false; }
            return true;
        }

        public void ToggleACC(string key)
        {
            if (key == "OFF")
            {
                tempomatPacket.IsEnabled = false;
            }
            else
            {
                IsEnabled = !IsEnabled;
                tempomatPacket.IsEnabled = !tempomatPacket.IsEnabled;
                if (IsEnabled)
                {
                    UserSetSpeed = SpeedValid(CurrentSpeed);
                }
            }
        }

        private void Accelerate()
        {
            int result = (int)Math.Floor(Convert.ToDouble(CurrentSpeed + GetGoalSpeed() / 100 + CurrentSpeed / 2 + 50));
            if (result > 100)
                ThrottlePercentage = 100;
            else
                ThrottlePercentage = result;
            tempomatPacket.BrakePercentage = 0;
            tempomatPacket.ThrottlePercentage = ThrottlePercentage;

        }

        private void Decelerate()
        {
            int result = (int)Math.Floor(Convert.ToDouble(CurrentSpeed + GetGoalSpeed() / 100 + 50));
            if (result > 100)
                BrakePercentage = 33;
            else
                BrakePercentage = (int)Math.Floor(Convert.ToDouble(result / 3));
            tempomatPacket.ThrottlePercentage = 0;
            tempomatPacket.BrakePercentage = BrakePercentage;
        }

        public void IncreaseGoalSpeed()
        {
            if (IsSpeedValid(UserSetSpeed + speedChangeInterval))
                UserSetSpeed += speedChangeInterval;
            else
                UserSetSpeed = SpeedValid(UserSetSpeed);
        }

        public void DecreaseGoalSpeed()
        {
            if (IsSpeedValid(UserSetSpeed - speedChangeInterval))
                UserSetSpeed -= speedChangeInterval;
            else
                UserSetSpeed = SpeedValid(UserSetSpeed);
        }
    }
}
