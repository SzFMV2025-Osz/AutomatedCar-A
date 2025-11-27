namespace AutomatedCar.SystemComponents.Packets;

using Models;
using ReactiveUI;
using System.Collections.Generic;


/// <summary>
/// Radar packet for radar sensor
/// </summary>
public class CameraPacket : ReactiveObject, IReadOnlyCameraPacket
{
    private WorldObject? highlightedObject = null;
    private List<WorldObject> relevantObjects = new ();

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
    public WorldObject HighlightedObject
    {
        get => this.highlightedObject;
        set => this.RaiseAndSetIfChanged(ref this.highlightedObject, value);
    }
}