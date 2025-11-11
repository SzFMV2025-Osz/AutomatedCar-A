namespace AutomatedCar.Models;

using Avalonia;
using Avalonia.Controls.Shapes;
using System.Collections.Generic;

/// <summary>
/// Interface for triangle
/// </summary>
public interface ITriangle
{
    /// <summary>
    /// Gets the elements which are in the triangle.
    /// </summary>
    List<WorldObject> IntersectsWith { get; }

    /// <summary>
    /// Gets the triangle's shape, so it can be visualized. Contains points,
    /// and should also contain an already drawn Geometry.
    /// </summary>
    Polygon TrianglePolygon { get; set; }

    /// <summary>
    /// Gets all objects in the triangle bounds
    /// </summary>
    /// <returns> All objects inside. </returns>
    public List<WorldObject> GetIntersections();

    /// <summary>
    /// Sets the position to the desired point.
    /// </summary>
    /// <param name="point">The point to go to.</param>
    public void RefreshTriangleTo(Point point);
}