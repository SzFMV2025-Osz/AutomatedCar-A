namespace AutomatedCar.SystemComponents.GearShifter
{
    using System;
    using Avalonia.Input;

    public interface IGearShifter
    {
        public Gear CurrentGear { get; set; }

        public int CurrentGearNumber { get; }

        public double SpeedKph { get; } // convenience (km/h)

        public double RPM { get; } // engine RPM

        public void Shift(Key key); // keep your existing input method

        public void ShiftUp();

        public void ShiftDown();
    }
}
