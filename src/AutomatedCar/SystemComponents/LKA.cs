namespace AutomatedCar.SystemComponents;
using AutomatedCar.SystemComponents.Packets;

public class LKA : SystemComponent
{
    
    private readonly LKAPacket lkaPacket;
    private bool wasPressed = false; 
    
    public LKA(VirtualFunctionBus virtualFunctionBus)
        : base(virtualFunctionBus)
    {
        this.lkaPacket = new LKAPacket();
            
        
        this.virtualFunctionBus.RegisterComponent(this);
        this.virtualFunctionBus.LKAPacket = this.lkaPacket;
    }
    
    public override void Process()
    {
        
        bool isPressedNow = this.virtualFunctionBus.KeyboardHandlerPacket.LKAKey;
        
        if (isPressedNow && !this.wasPressed)
        {
            this.lkaPacket.IsActive = !this.lkaPacket.IsActive;
        }
            
        
        this.wasPressed = isPressedNow;

        
        this.virtualFunctionBus.LKAPacket = this.lkaPacket;
    }
}
    