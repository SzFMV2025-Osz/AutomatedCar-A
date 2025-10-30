namespace AutomatedCar.SystemComponents.Gearbox
{
    using AutomatedCar.SystemComponents.GearBox_test;
    using AutomatedCar.SystemComponents.GearShifter;

    /// <summary>
    /// Interface for the transmission types.
    /// </summary>
    public interface IGearBox
    {
        float Speed { get; set; }

        Gear GearStage { get; }

        void ShiftingGear(ShiftDir shift);

        int CalculateGearSpeed(int revolution, int enginespeed);
    }
}
