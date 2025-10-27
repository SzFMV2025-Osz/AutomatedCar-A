namespace AutomatedCar.Helpers;

using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

/// <summary>
/// The GeometryHelper class is responsible for mostly collision detection, and for common mathematical calculations in
/// the geometry of the World.
/// </summary>
public class GeometryHelper
{
    /// <summary>
    /// Gets the distance between two points in float.
    /// </summary>
    /// <param name="a">The first <c>WorldObject</c>.</param>
    /// <param name="b">The second <c>WorldObject</c>.</param>
    /// <returns>The total distance between the two absolute positions.</returns>
    public static float DistanceBetweenObjects(WorldObject a, WorldObject b) =>
        (float)Point.Distance(
            new (x: a.X, y: a.Y),
            new (x: b.X, y: b.Y));

    /// <summary>
    /// Check if a nearby object is in the drawn polygonal shape of the car.
    /// </summary>
    /// <param name="obj">The WorldObject which is near to the currently controlled car.</param>
    /// <returns>Whether the WorldObject is in the car.</returns>
    public static bool CheckWorldObjectInCar(WorldObject obj)
    {
        Polygon car = GetCarAbsolutePolygon();
        var geom = new StreamGeometry();

        // Draw the polygon using the geometry of the car.
        using (var drawer = geom.Open())
        {
            drawer.BeginFigure(car.Points[0], isFilled: true);

            foreach (var point in car.Points)
            {
                drawer.LineTo(point);
            }

            drawer.EndFigure(isClosed: true);
        }

        return PositionWorldObjectPointsToAbsolute(obj).Any(geom.FillContains);
    }

    private static Polygon GetCarAbsolutePolygon()
    {
        AutomatedCar car = World.Instance.ControlledCar;
        Rect carGeometryBounds = car.Geometry.Bounds;
        IList<Point> carPoints = car.Geometry.Points;

        // Creating transformational matrices for the polygon's points.
        Transform translation = new TranslateTransform
        {
            X = car.X - carGeometryBounds.Center.X,
            Y = car.Y - carGeometryBounds.Center.Y,
        };
        Transform rotation = new RotateTransform
        {
            CenterX = car.X - carGeometryBounds.Center.X + car.RotationPoint.X,
            CenterY = car.Y - carGeometryBounds.Center.Y + car.RotationPoint.Y,
            Angle = 180 + car.Rotation,
        };

        // Return the polygon with the transformed points.
        return new Polygon
        {
            Points = carPoints
                .Select(x => x.Transform(translation.Value).Transform(rotation.Value))
                .ToList(),
        };
    }

    private static IList<Point> PositionWorldObjectPointsToAbsolute(WorldObject obj)
    {
        var objectGeometry = obj.Geometries.First();

        // Creating the rotational matrices for the points.
        var rotation = new RotateTransform
        {
            CenterX = obj.RotationPoint.X,
            CenterY = obj.RotationPoint.Y,
            Angle = obj.Rotation,
        };
        var translation = new TranslateTransform
        {
            X = obj.X,
            Y = obj.Y,
        };

        // Executes transformations, then put's the newly transformed points in a list.
        return objectGeometry.Points
            .Select(x => x.Transform(rotation.Value).Transform(translation.Value))
            .ToList();
    }
}