namespace AutomatedCar.SystemComponents;

using Avalonia;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///     This class is responsible for handling collisions with <c>Collidable</c> WorldObjects.
/// </summary>
public class CollisionDetectionService : SystemComponent
{
    private bool collide;



    /// <summary>
    ///     Initializes a new instance of the <see cref="CollisionDetectionService" /> class.
    /// </summary>
    /// <param name="virtualFunctionBus">The <c>VirtualFunctionBus</c> needs to be given, in order to register it.</param>
    public CollisionDetectionService(VirtualFunctionBus virtualFunctionBus)
        : base(virtualFunctionBus)
    {
        this.virtualFunctionBus.RegisterComponent(this);
    }

    /// <summary>
    ///     This event will be invoked, if the car has collided with a <c>Collidable</c> WorldObject.
    /// </summary>
    public event EventHandler<WorldObject> OnCollided;

    /// <summary>
    ///     Every Tick we inspect if the nearby collideable objects are indeed in the car's polygonal shape.
    /// </summary>
    public override void Process()
    {
        AutomatedCar currentCar = World.Instance.ControlledCar;

        this.collide = false;
        // Get all collidable WorldObjects in nearby vicinity of the currently controlled car.
        List<WorldObject> nearbyObjects = World.Instance.WorldObjects
            .Where(x => GeometryHelper.DistanceBetweenObjects(x, currentCar) < 435 && x.Collideable)
            .ToList();

        nearbyObjects.ForEach(this.InvokeEventIfCollided);
        if (this.virtualFunctionBus.WritableDummyPacket != null)
        {
            this.virtualFunctionBus.WritableDummyPacket.IsColliding = this.collide;
        }
    }

    /// <summary>
    ///     Checks if any of a WorldObjects's Points are in the car's absolute positioned polygonal shape.
    /// </summary>
    /// <param name="x">The WorldObject we want to check, can be assumed that it is a Collideable.</param>
    private void InvokeEventIfCollided(WorldObject x)
    {
        if (GeometryHelper.CheckWorldObjectInCar(x))
        {
           this.collide = true;
           this.OnCollided?.Invoke(this, x);
        }
    }
}