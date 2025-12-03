namespace AutomatedCar.SystemComponents.Packets;

using ReactiveUI;

public class LKAPacket : ReactiveObject, IReadOnlyLKAPacket
{
    private bool isActive = false;
    public bool IsActive
    {
        get => this.isActive;
        set
        {
            this.RaiseAndSetIfChanged(ref this.isActive, value);
            this.RaisePropertyChanged(nameof(IsLkaDisabled));
        }
    }

    public bool IsReady { get; set; } = false;
    public bool IsLkaDisabled => !this.IsActive && IsReady;
}