namespace AutomatedCar.SystemComponents.Packets
{
    public interface IReadOnlyAccPacket
    {
        bool IsActive { get; }
        int? TargetSpeedKmh { get; }
        double? TimeGapSec { get; }
        int? ThrottlePercentage { get; }
        int? BrakePercentage { get; }
    }

    public class AccPacket : IReadOnlyAccPacket
    {
        public bool IsActive { get; set; }
        public int? TargetSpeedKmh { get; set; }
        public double? TimeGapSec { get; set; }
        public int? ThrottlePercentage { get; set; }
        public int? BrakePercentage { get; set; }
    }
}
