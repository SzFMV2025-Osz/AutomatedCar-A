namespace AutomatedCar.Models;

using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A Triangle in the world, it is used mostly for sensor's field of vision.
/// </summary>
public class Triangle : ITriangle
{
    private float angle;
    private float facingAngle;
    private float distance;
    private Point carPoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="Triangle"/> class.
    /// </summary>
    /// <param name="angle">The triangle's base corner angle (The FoV in a sensor).</param>
    /// <param name="facingAngle">The angle relative to the current car's facing.</param>
    /// <param name="distance">The triangle's height.</param>
    /// <param name="carPoint">An absolute point on the car <see cref="CarPolygonPosition"/> for inspiration.</param>
    public Triangle(float angle, float facingAngle, float distance, Point carPoint)
    {
        this.angle = angle;
        this.facingAngle = facingAngle;
        this.distance = distance;
        this.carPoint = carPoint;
    }

    /// <summary>
    /// Gets the list of items that are in the space of the triangle.
    /// </summary>
    public List<WorldObject> IntersectsWith { get => this.GetIntersections(); }

    /// <summary>
    /// Gets the triangle visually.
    /// </summary>
    public Polygon TrianglePolygon => CreateTriangleAbsolutePolygon();

    /// <summary>
    /// Similar function to the one in GeometryHelper, but it needs other coordinates in order to work.
    /// </summary>
    /// <returns>A list of WorldObjects, which are in the triangle's space.</returns>
    public List<WorldObject> GetIntersections()
    {
        return World.Instance.WorldObjects
            .Where(x =>
                x != World.Instance.ControlledCar &&
                x.Geometries
                    .First().Points
                    .Any(GeometryHelper.CreateGeometryFromPoints(this.TrianglePolygon.Points.ToList()).FillContains))
            .ToList();
    }

    /// <summary>
    /// Creates a triangle shaped polygon, so it can be used for intersection detection, or drawing.
    /// </summary>
    /// <returns>The polygon of a triangle.</returns>
    // FIXME: be a bit cleaner
    public Polygon CreateTriangleAbsolutePolygon()
    {
        var currentCarAngle = World.Instance.ControlledCar.Rotation;

        List<Point> trianglePoints =
        [
            new (x: 0, y: 0),
            new (x: this.distance * Math.Tan(double.DegreesToRadians(this.angle / 2)), y: this.distance),
            new (x: -this.distance * Math.Tan(double.DegreesToRadians(this.angle / 2)), y: this.distance),
        ];

        var rotation = new RotateTransform
        {
            Angle = 180 - currentCarAngle + this.facingAngle,
            CenterX = 0,
            CenterY = 0,
        };
        var translation = new TranslateTransform
        {
            X = this.carPoint.X,
            Y = this.carPoint.Y,
        };

        Polygon triangle = new Polygon
        {
            Points =
                [
                    trianglePoints[0].Transform(translation.Value),
                    ..trianglePoints[1..].Select(x => x.Transform(rotation.Value).Transform(translation.Value))
                ],
        };

        return triangle;
    }
}