namespace AutomatedCar.SystemComponents.Packets;

using ReactiveUI;

public class LKAPacket : ReactiveObject, IReadOnlyLKAPacket
{
    public bool IsActive
    {
        get => this.isActive;
        set => this.RaiseAndSetIfChanged(ref this.isActive, value);
    }


    private bool isActive = false;
}