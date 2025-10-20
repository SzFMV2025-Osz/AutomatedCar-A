namespace AutomatedCar.SystemComponents.Packets;

using Models;
using System.Collections.Generic;


/// <summary>
/// Radar packet for radar sensor
/// </summary>
public class RadarPacket : IReadOnlyRadarPacket
{
    /// <summary>
    /// Gets relevant objects and only private set
    /// </summary>
    public List<WorldObject> RelevantObjects { get; private set; }

    /// <summary>
    /// Gets the nearest object.
    /// </summary>
    public WorldObject HightlightedObject { get; private set; }

    /// <summary>
    /// Sets relevant objects privately
    /// </summary>
    /// <param name="relevantObjects"> Relevant objects from radar. </param>
    public void SetRelevantObjects(List<WorldObject> relevantObjects)
    {
        this.RelevantObjects = relevantObjects;
    }

    /// <summary>
    /// Sets the nearest object privately.
    /// </summary>
    /// <param name="highlightedObject">The nearest object.</param>
    public void SetHighlightedObject(WorldObject highlightedObject)
    {
        this.HightlightedObject = highlightedObject;
    }
}