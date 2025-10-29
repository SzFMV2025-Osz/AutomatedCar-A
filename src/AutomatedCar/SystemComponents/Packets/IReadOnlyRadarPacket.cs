namespace AutomatedCar.SystemComponents.Packets;

using Models;
using System.Collections.Generic;

/// <summary>
/// Read only packet for virtual bus
/// </summary>
public interface IReadOnlyRadarPacket
{
    /// <summary>
    /// Gets relevant objects.
    /// </summary>
    List<WorldObject> RelevantObjects { get; }

    /// <summary>
    /// Gets the nearest object.
    /// </summary>
    WorldObject? HightlightedObject { get; }
}