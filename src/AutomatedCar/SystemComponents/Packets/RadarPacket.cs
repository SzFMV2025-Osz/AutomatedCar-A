namespace AutomatedCar.SystemComponents.Packets;

using Models;using ReactiveUI;
using System.Collections.Generic;


/// <summary>
/// Radar packet for radar sensor
/// </summary>
public class RadarPacket : ReactiveObject, IReadOnlyRadarPacket
{
    private List<WorldObject> relevantObjects = new ();
    private WorldObject? highlightedObject = null;


    /// <summary>
    /// Gets relevant objects and only private set
    /// </summary>
    public List<WorldObject> RelevantObjects
    {
        get => this.relevantObjects;
        set => this.RaiseAndSetIfChanged(ref this.relevantObjects, value);
    }

    /// <summary>
    /// Gets the nearest object.
    /// </summary>
    public WorldObject? HightlightedObject
    {
        get => this.highlightedObject;
        set => this.RaiseAndSetIfChanged(ref this.highlightedObject, value);
    }
}