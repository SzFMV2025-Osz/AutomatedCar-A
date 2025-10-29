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

    /// <summary>
    /// Initializes a new instance of the <see cref="Triangle"/> class.
    /// </summary>
    /// <param name="angle">The triangle's base corner angle (The FoV in a sensor).</param>
    /// <param name="facingAngle">The angle relative to the current car's facing.</param>
    /// <param name="distance">The triangle's height.</param>
    /// <param name="carPoint">An absolute point on the car <see cref="CarPolygonPosition"/> for inspiration.</param>
    public Triangle(float angle, float facingAngle, float distance)
    {
        this.angle = angle;
        this.facingAngle = facingAngle;
        this.distance = distance;

        // Only used for initialization in order to not get an Out of Range exception.
        this.TrianglePolygon = new Polygon()
        {
            Points = this.CreateRelativeTrianglePoints(),
        };
    }

    /// <summary>
    /// Gets the list of items that are in the space of the triangle.
    /// </summary>
    public List<WorldObject> IntersectsWith { get => this.GetIntersections(); }

    /// <summary>
    /// Gets the triangle visually.
    /// </summary>
    public Polygon TrianglePolygon { get; set; }

    /// <summary>
    /// Similar function to the one in GeometryHelper, but it needs other coordinates in order to work.
    /// </summary>
    /// <returns>A list of WorldObjects, which are in the triangle's space.</returns>
    public List<WorldObject> GetIntersections()
    {
        return World.Instance.WorldObjects
            .Where(x =>
                x != World.Instance.ControlledCar &&
                this.ReturnGeometryOrAbsolutePoint(x).Points
                    .Any(GeometryHelper.CreateGeometryFromPoints(this.TrianglePolygon.Points.ToList()).FillContains))
            .ToList();
    }

    public void RefreshTriangleTo(Point point)
    {
        this.TrianglePolygon = this.CreateTriangleAbsolutePolygon(point);
    }

    /// <summary>
    /// Creates a triangle shaped polygon, so it can be used for intersection detection, or drawing.
    /// </summary>
    /// <param name="point">The point to place the triangle.</param>
    /// <returns>The polygon of a triangle.</returns>
    // FIXME: be a bit cleaner
    private Polygon CreateTriangleAbsolutePolygon(Point point)
    {
        var currentCarAngle = World.Instance.ControlledCar.Rotation;

        List<Point> trianglePoints = this.CreateRelativeTrianglePoints();

        var rotation = new RotateTransform
        {
            Angle = 180 - currentCarAngle + this.facingAngle,
            CenterX = 0,
            CenterY = 0,
        };
        var translation = new TranslateTransform
        {
            X = point.X,
            Y = point.Y,
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

    private List<Point> CreateRelativeTrianglePoints()
    {
        return
        [
            new (x: 0, y: 0),
            new (x: this.distance * Math.Tan(double.DegreesToRadians(this.angle / 2)), y: this.distance),
            new (x: -this.distance * Math.Tan(double.DegreesToRadians(this.angle / 2)), y: this.distance),
        ];
    }

    private PolylineGeometry ReturnGeometryOrAbsolutePoint(WorldObject obj)
    {
        var geom = obj.Geometries.FirstOrDefault();
        var polyGeom = new PolylineGeometry();

        if (geom == null)
        {
            polyGeom.Points =
            [
                new(x: obj.X, y: obj.Y)
            ];
        }
        else
        {
            polyGeom.Points = GeometryHelper.PositionWorldObjectPointsToAbsolute(obj);
        }

        return polyGeom;
    }
}