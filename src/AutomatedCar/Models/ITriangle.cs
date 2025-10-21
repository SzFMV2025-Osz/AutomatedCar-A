namespace AutomatedCar.Models;

using System.Collections.Generic;

/// <summary>
/// Interface for triangle
/// </summary>
public interface ITriangle
{
    /// <summary>
    /// Gets all objects in the triangle bounds
    /// </summary>
    /// <returns> All objects inside. </returns>
    public List<WorldObject> IntersectsWith();
}