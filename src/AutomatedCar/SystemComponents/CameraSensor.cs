namespace AutomatedCar.SystemComponents;

using Avalonia;
using Avalonia.Controls.Shapes;
using Helpers;
using Models;
using Packets;
using System;
using System.Linq;

/// <summary>
/// Camera sensor.
/// </summary>
public class CameraSensor : SystemComponent
{
    /// <summary>
    /// Items that the camera can see.
    /// </summary>
    private static readonly WorldObjectType[] CanSee =
    {
        WorldObjectType.Crosswalk,
        WorldObjectType.ParkingSpace,
        WorldObjectType.Road,
        WorldObjectType.RoadSign,
    };

    private readonly CameraPacket cameraPacket;

    private readonly ITriangle vision;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraSensor"/> class.
    /// </summary>
    /// <param name="virtualFunctionBus">Need a virtual bus.</param>
    /// <param name="vision">An <c>ITriangle</c> representing the sensor's vision.</param>
    public CameraSensor(VirtualFunctionBus virtualFunctionBus, ITriangle vision)
        : base(virtualFunctionBus)
    {
        this.vision = vision;
        this.cameraPacket = new CameraPacket();
        this.virtualFunctionBus.RegisterComponent(this);
    }

    /// <summary>
    /// Process.
    /// </summary>
    public override void Process()
    {
        this.vision.RefreshTriangleTo(GeometryHelper.GetCarAbsolutePolygon().Points[SensorValues.Camera.PositionIndex]);
        this.vision.GetIntersections();

        this.cameraPacket.RelevantObjects =
            this.vision.IntersectsWith
                .Where(x => CanSee.Contains(x.WorldObjectType))
                .ToList();
        this.cameraPacket.HightlightedObject = this.GetHighlightedObject();
        this.virtualFunctionBus.CameraPacket = this.cameraPacket;
    }

    private WorldObject GetHighlightedObject()
    {
        var car = World.Instance.ControlledCar;
        return this.cameraPacket.RelevantObjects
            .OrderBy(x =>
                Point.Distance(new Point(x.X, x.Y), new Point(car.X, car.Y)))
            .FirstOrDefault();
    }
}