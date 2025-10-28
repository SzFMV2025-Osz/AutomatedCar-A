namespace AutomatedCar.SystemComponents;

using Avalonia;
using Models;
using Packets;
using System;
using System.Linq;

/// <summary>
/// Radar sensor
/// </summary>
public class RadarSensor : SystemComponent
{
    /// <summary>
    /// Items that the radar cant see
    /// </summary>
    private static readonly WorldObjectType[] CantSee =
    {
        WorldObjectType.Crosswalk,
        WorldObjectType.ParkingSpace,
        WorldObjectType.Road,
        WorldObjectType.RoadSign,
    };

    private readonly RadarPacket radarPacket;

    private readonly ITriangle vision;

    /// <summary>
    /// Initializes a new instance of the <see cref="RadarSensor"/> class.
    /// </summary>
    /// <param name="virtualFunctionBus">Need a virtual bus.</param>
    /// <param name="vision">Need an ITriangle.</param>
    public RadarSensor(VirtualFunctionBus virtualFunctionBus, ITriangle vision)
        : base(virtualFunctionBus)
    {
        this.vision = vision;
        this.radarPacket = new RadarPacket();
        this.virtualFunctionBus.RegisterComponent(this);
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void Process()
    {
        this.vision.GetIntersections();
        this.radarPacket.SetRelevantObjects(this.vision.IntersectsWith.Where(x => !CantSee.Contains(x.WorldObjectType)).ToList());
        this.radarPacket.SetHighlightedObject(this.GetHighlightedObject());
        this.virtualFunctionBus.RadarPacket = this.radarPacket;
    }

    private WorldObject GetHighlightedObject()
    {
        var car = World.Instance.ControlledCar;
        return this.radarPacket.RelevantObjects
            .OrderBy(x =>
                Point.Distance(new Point(x.X, x.Y), new Point(car.X, car.Y)))
            .FirstOrDefault();
    }
}
