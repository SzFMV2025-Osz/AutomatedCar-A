namespace AutomatedCar.Helpers
{
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Drawing;               // a bitmap olvasáshoz

    // --- ALIASOK: ez szünteti meg az ütközést ---
    using APoint = Avalonia.Point;
    using APoints = Avalonia.Points;
    using DPoint = System.Drawing.Point;

    /// <summary>
    /// NPC-khez poligon hitbox: először worldobject_polygons.json,
    /// ha nincs, akkor a sprite átlátszatlan pixeleiből konvex-hull.
    /// </summary>
    public static class NpcHitboxHelper
    {
        public static bool TryGetLibraryPolys(string spriteFileName, out List<PolylineGeometry> polys)
        {
            polys = null;
            try
            {
                var key = Path.GetFileNameWithoutExtension(spriteFileName);

                using var s = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("AutomatedCar.Assets.worldobject_polygons.json");
                if (s is null) return false;

                using var reader = new StreamReader(s);
                var text = reader.ReadToEnd();

                // JSON: { "objects": [ { "type": "...", "polys":[{"points":[[x,y],...]}, ...] } ] }
                var root = System.Text.Json.JsonDocument.Parse(text).RootElement;
                if (!root.TryGetProperty("objects", out var arr) || arr.ValueKind != System.Text.Json.JsonValueKind.Array)
                    return false;

                polys = new List<PolylineGeometry>();

                foreach (var obj in arr.EnumerateArray())
                {
                    if (!obj.TryGetProperty("type", out var t) || !t.GetString().Equals(key, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!obj.TryGetProperty("polys", out var polysArr)) continue;

                    foreach (var rp in polysArr.EnumerateArray())
                    {
                        if (!rp.TryGetProperty("points", out var ptsArr)) continue;

                        var pts = new APoints();
                        foreach (var p in ptsArr.EnumerateArray())
                        {
                            var x = p[0].GetInt32();
                            var y = p[1].GetInt32();
                            pts.Add(new APoint(x, y));   // <-- kifejezetten Avalonia.Point
                        }

                        polys.Add(new PolylineGeometry(pts, false));
                    }
                }

                return polys.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        // Fallback: sprite átlátszatlan pixeleiből konvex burok
        public static PolylineGeometry BuildHullFromSprite(string spriteFileName, int sampleStep = 4, byte alphaThresh = 10)
        {
            using var s = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.WorldObjects.{spriteFileName}");
            if (s is null) return new PolylineGeometry(new APoints(), false);

            using var bmp = new Bitmap(s);
            var pts = new List<DPoint>();

            for (int y = 0; y < bmp.Height; y += sampleStep)
            {
                for (int x = 0; x < bmp.Width; x += sampleStep)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.A > alphaThresh) pts.Add(new DPoint(x, y));
                }
            }

            if (pts.Count < 3)
            {
                var rect = new APoints
                {
                    new APoint(0, 0),
                    new APoint(bmp.Width, 0),
                    new APoint(bmp.Width, bmp.Height),
                    new APoint(0, bmp.Height),
                    new APoint(0, 0),
                };
                return new PolylineGeometry(rect, false);
            }

            var hull = ConvexHull(pts);
            var outPts = new APoints();
            foreach (var p in hull) outPts.Add(new APoint(p.X, p.Y));
            if (hull.Count > 0) outPts.Add(new APoint(hull[0].X, hull[0].Y));
            return new PolylineGeometry(outPts, false);
        }

        // Monotone chain konvex-hull
        private static List<DPoint> ConvexHull(List<DPoint> pts)
        {
            var p = pts.Distinct().OrderBy(q => q.X).ThenBy(q => q.Y).ToList();
            if (p.Count <= 2) return p.ToList();

            var lower = new List<DPoint>();
            foreach (var v in p)
            {
                while (lower.Count >= 2 && Cross(lower[^2], lower[^1], v) <= 0) lower.RemoveAt(lower.Count - 1);
                lower.Add(v);
            }
            var upper = new List<DPoint>();
            for (int i = p.Count - 1; i >= 0; --i)
            {
                var v = p[i];
                while (upper.Count >= 2 && Cross(upper[^2], upper[^1], v) <= 0) upper.RemoveAt(upper.Count - 1);
                upper.Add(v);
            }
            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            return lower;
        }

        private static long Cross(DPoint a, DPoint b, DPoint c)
            => (long)(b.X - a.X) * (c.Y - a.Y) - (long)(b.Y - a.Y) * (c.X - a.X);

        public static (int x, int y) GetSpriteCenter(string spriteFileName)
        {
            using var s = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.WorldObjects.{spriteFileName}");
            using var bmp = new Bitmap(s);
            return (bmp.Width / 2, bmp.Height / 2);
        }
    }
}
