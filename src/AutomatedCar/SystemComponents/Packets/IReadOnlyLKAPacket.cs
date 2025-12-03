namespace AutomatedCar.SystemComponents.Packets;

public interface IReadOnlyLKAPacket
{
    public bool IsActive { get; }
    bool IsLkaDisabled { get; }
    
    public bool IsReady { get; set; }
    
}