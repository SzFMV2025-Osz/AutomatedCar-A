namespace AutomatedCar.SystemComponents;

using Models;
using Packets;
using System;
using System.Linq;

/// <summary>
/// Camera sensor
/// </summary>
public class CameraSensor : SystemComponent
{
    /// <summary>
    /// Items that the camera cant see
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
    /// <param name="vision">Need an ITriangle.</param>
    public CameraSensor(VirtualFunctionBus virtualFunctionBus, ITriangle vision)
        : base(virtualFunctionBus)
    {
        this.vision = vision;
        this.cameraPacket = new CameraPacket();
        this.virtualFunctionBus.RegisterComponent(this);
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void Process()
    {
        this.cameraPacket.SetRelevantObjects(this.vision.IntersectsWith().Where(x => CanSee.Contains(x.WorldObjectType)).ToList());
        this.cameraPacket.SetHighlightedObject(this.GetHighlightedObject());
        this.virtualFunctionBus.CameraPacket = this.cameraPacket;
    }

    private WorldObject GetHighlightedObject()
    {
        var car = World.Instance.WorldObjects.OfType<AutomatedCar>().First();
        return this.cameraPacket.RelevantObjects.OrderBy(x => Math.Sqrt(Math.Pow(Math.Abs(car.X - x.X), 2) + Math.Pow(Math.Abs(car.Y - x.Y), 2))).FirstOrDefault();
    }
}