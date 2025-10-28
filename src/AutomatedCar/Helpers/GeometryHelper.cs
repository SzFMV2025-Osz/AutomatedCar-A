namespace AutomatedCar.Helpers;

using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

/// <summary>
/// The GeometryHelper class is responsible for mostly collision detection, and for common mathematical calculations in
/// the geometry of the World.
/// </summary>
public static class GeometryHelper
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

    public static StreamGeometry CreateGeometryFromPoints(List<Point> points)
    {
        var geometry = new StreamGeometry();

        using (var drawer = geometry.Open())
        {
            drawer.BeginFigure(isFilled: true, startPoint: points[0]);
            points[1..].ForEach(drawer.LineTo);
            drawer.EndFigure(isClosed: true);
        }

        return geometry;
    }

    public static float TranslateAlongAngleX(float x, float angle, float distance)
    {
        return (float)(x + (Math.Cos(angle) * distance));
    }

    public static float TranslateAlongAngleY(float y, float angle, float distance)
    {
        return (float)(y + (Math.Sin(angle) * distance));
    }

    /// <summary>
    /// Check if a nearby object is in the drawn polygonal shape of the car.
    /// </summary>
    /// <param name="obj">The WorldObject which is near to the currently controlled car.</param>
    /// <returns>Whether the WorldObject is in the car.</returns>
    public static bool CheckWorldObjectInCar(WorldObject obj)
    {
        Polygon car = GetCarAbsolutePolygon();
        var geom = CreateGeometryFromPoints(car.Points.ToList());
        return PositionWorldObjectPointsToAbsolute(obj).Any(geom.FillContains);
    }

    /// <summary>
    /// Transforms the World instance's <c>CurrentCar</c> to absolute positions.
    /// </summary>
    /// <returns>A <c>Polygon</c> representing a car on absolute coordinates.</returns>
    public static Polygon GetCarAbsolutePolygon()
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
            Angle = 180 - car.Rotation,
        };

        // Return the polygon with the transformed points.
        return new Polygon
        {
            Points = carPoints
                .Select(x => x.Transform(translation.Value).Transform(rotation.Value))
                .ToList(),
        };
    }

    /// <summary>
    /// Return a Point between a and b with ratio.
    /// </summary>
    /// <param name="a">First Point for divide.</param>
    /// <param name="b">Second Point for divide.</param>
    /// <param name="ratio">between 0 and 1 set the divide ratio.</param>
    /// <returns>Return a Point between "a" and "b" with ratio.</returns>
    public static Point DividePoints(Point a, Point b, double ratio)
    {
        return new Point(
            a.X + (b.X - a.X) * ratio,
            a.Y + (b.Y - a.Y) * ratio
            );
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

/// <summary>
/// Returns with the indices of the polygon defined within the car, mostly used for placing stuff on the car.
/// </summary>
public enum CarPolygonPosition
{
    /// <summary>
    /// The front left part of the bumper.
    /// </summary>
    FrontLeft = 41,

    /// <summary>
    /// The front right part of the front bumper.
    /// </summary>
    FrontRight = 27,

    /// <summary>
    /// The left mirror of the car.
    /// </summary>
    LeftMirror = 55,

    /// <summary>
    /// The right mirror of the car.
    /// </summary>
    RightMirror = 13,

    /// <summary>
    /// The bottom left part of the rear bumper.
    /// </summary>
    RearLeft = 66,

    /// <summary>
    /// The bottom right part of the rear bumper.
    /// </summary>
    RearRight = 2,

    /// <summary>
    /// The front middle part of the front bumber.
    /// </summary>
    FrontMiddle = 34,

    /// <summary>
    /// The rear middle part of the rear bumber.
    /// </summary>
    RearMiddle = 0,
}