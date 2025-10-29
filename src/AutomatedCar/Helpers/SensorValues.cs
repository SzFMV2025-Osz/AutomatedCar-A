namespace AutomatedCar.Helpers;

/// <summary>
/// Represents hardcoded values for sensors, in order to not use magic numbers in code.
/// </summary>
public static class SensorValues
{
    /// <summary>
    /// The hardcoded values of the Camera sensor.
    /// <seealso href="https://szfmv2025-osz.github.io/handout/sensors.html#kamera"/>
    /// </summary>
    public struct Camera
    {
        public const float FoV = 45;
        public const float RelativeAngle = 180;
        public const float Distance = 50 * 80;
        public const int PositionIndex = (int)CarPolygonPosition.FrontMiddle;
    }

    /// <summary>
    ///  The hardcoded values of the Radar sensor.
    /// <seealso href="https://szfmv2025-osz.github.io/handout/sensors.html#radar"/>
    /// </summary>
    public struct Radar
    {
        public const float FoV = 60;
        public const float RelativeAngle = 180;
        public const float Distance = 50 * 200;
        public const int PositionIndex = (int)CarPolygonPosition.FrontMiddle;
    }
}