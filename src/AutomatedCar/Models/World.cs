namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using Helpers;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Visualization;
    using Avalonia.Media;
    using SkiaSharp;
    using Svg;
    using System.Linq;
    using System.Diagnostics;
	using NpcNs = global::AutomatedCar.Models.NPC;

    public class World
    {
        
        public NpcNs.NPCManager npcManager = new NpcNs.NPCManager();

        private int controlledCarPointer = 0;
        public List<AutomatedCar> controlledCars = new();

        public static World Instance { get; } = new World();
        public List<WorldObject> WorldObjects { get; set; } = new List<WorldObject>();

        public AutomatedCar ControlledCar => this.controlledCars[this.controlledCarPointer];

        public int ControlledCarPointer
        {
            get => this.controlledCarPointer;
            set { this.controlledCarPointer = value; }
        }

        public void AddControlledCar(AutomatedCar controlledCar)
        {
            this.controlledCars.Add(controlledCar);
            this.AddObject(controlledCar);
        }

        public void NextControlledCar()
        {
            if (this.controlledCarPointer < this.controlledCars.Count - 1) this.ControlledCarPointer += 1;
            else this.ControlledCarPointer = 0;
        }

        public void PrevControlledCar()
        {
            if (this.controlledCarPointer > 0) this.ControlledCarPointer -= 1;
            else this.ControlledCarPointer = this.controlledCars.Count - 1;
        }

        public int Width { get; set; }
        public int Height { get; set; }

        private DebugStatus debugStatus = new DebugStatus();

        public void AddObject(WorldObject worldObject) => this.WorldObjects.Add(worldObject);

        public void PopulateFromJSON(string filename)
        {
            var rotationPoints = this.ReadRotationsPoints();
            var renderTransformOrigins = this.CalculateRenderTransformOrigins();
            var worldObjectPolygons = this.ReadPolygonJSON();

            using var reader = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(filename));

            RawWorld rawWorld = JsonConvert.DeserializeObject<RawWorld>(reader.ReadToEnd());
            this.Height = rawWorld.Height;
            this.Width = rawWorld.Width;

            foreach (RawWorldObject rwo in rawWorld.Objects)
            {
                var wo = new WorldObject(
                    rwo.X,
                    rwo.Y,
                    rwo.Type + ".png",
                    this.DetermineZIndex(rwo.Type),
                    this.DetermineCollidablity(rwo.Type),
                    this.DetermineType(rwo.Type));

                (int x, int y) rp = (0, 0);
                if (rotationPoints.ContainsKey(rwo.Type)) rp = rotationPoints[rwo.Type];
                wo.RotationPoint = new System.Drawing.Point(rp.x, rp.y);

                string rto = "0,0";
                if (renderTransformOrigins.ContainsKey(rwo.Type)) rto = renderTransformOrigins[rwo.Type];
                wo.RenderTransformOrigin = rto;

                wo.Rotation = this.RotationMatrixToDegree(rwo.M11, rwo.M12);

                if (worldObjectPolygons.ContainsKey(rwo.Type))
                {
                   
                    foreach (var g in worldObjectPolygons[rwo.Type])
                    {
                        wo.Geometries.Add(new PolylineGeometry(g.Points, false));
                        wo.RawGeometries.Add(new PolylineGeometry(g.Points, false));
                    }

                    
                    foreach (var geometry in wo.Geometries)
                    {
                        var rotate = new RotateTransform(wo.Rotation);
                        var translate = new TranslateTransform(-wo.RotationPoint.X, -wo.RotationPoint.Y);
                        var transformGroup = new TransformGroup();
                        transformGroup.Children.Add(rotate);
                        transformGroup.Children.Add(translate);

                        var mx2 = new SKMatrix
                        {
                            ScaleX = rwo.M11,
                            SkewX = rwo.M12,
                            TransX = wo.RotationPoint.X,
                            ScaleY = rwo.M22,
                            SkewY = rwo.M21,
                            TransY = wo.RotationPoint.Y,
                        };

                        PointF[] gpa = new PointF[geometry.Points.Count];
                        var m = SKMatrix.CreateRotationDegrees((float)wo.Rotation, wo.RotationPoint.X, wo.RotationPoint.Y);
                        m = m.PostConcat(SKMatrix.CreateTranslation(wo.RotationPoint.X, wo.RotationPoint.Y));
                        var gpa2 = geometry.Points.Select(x => new SKPoint((float)x.X, (float)x.Y)).ToArray();
                        this.ToDotNetPoints(geometry.Points).CopyTo(gpa);
                        mx2.MapPoints(gpa2);
                        geometry.Points = this.ToAvaloniaPoints(gpa2);
                    }
                }

                this.AddObject(wo);
            }

    
    var basePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Path");
    string mapKey = ExtractMapKey(filename);

    
    string alt1 = mapKey.Contains("world") ? mapKey.Replace("world", "word") : null;
    string alt2 = mapKey.Contains("_") ? mapKey.Replace("_", "") : null;

    var carCandidates = new List<string> { $"{mapKey}.car.json" };
    if (!string.IsNullOrEmpty(alt1)) carCandidates.Add($"{alt1}.car.json");
    if (!string.IsNullOrEmpty(alt2)) carCandidates.Add($"{alt2}.car.json");
    carCandidates.Add("car.json");

    string carFile = FirstExisting(basePath, carCandidates);

    
    if (carFile == null)
    {
        var asm = Assembly.GetExecutingAssembly();
        foreach (var candidate in carCandidates)
        {
            var resName = $"AutomatedCar.Assets.Path.{candidate}";
            using var s = asm.GetManifestResourceStream(resName);
            if (s != null)
            {
                var tempPath = Path.Combine(Path.GetTempPath(), candidate);
                using var sr = new StreamReader(s);
                File.WriteAllText(tempPath, sr.ReadToEnd());
                carFile = tempPath;
                break;
            }
        }
    }

   
    string pedFile = null;
    if (!string.Equals(mapKey, "oval", StringComparison.OrdinalIgnoreCase))
    {
        var pedCandidates = new List<string> { $"{mapKey}.ped.json" };
        if (!string.IsNullOrEmpty(alt1)) pedCandidates.Add($"{alt1}.ped.json");
        if (!string.IsNullOrEmpty(alt2)) pedCandidates.Add($"{alt2}.ped.json");
        pedCandidates.Add("ped.json");

        pedFile = FirstExisting(basePath, pedCandidates);

        if (pedFile == null)
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var candidate in pedCandidates)
            {
                var resName = $"AutomatedCar.Assets.Path.{candidate}";
                using var s = asm.GetManifestResourceStream(resName);
                if (s != null)
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), candidate);
                    using var sr = new StreamReader(s);
                    File.WriteAllText(tempPath, sr.ReadToEnd());
                    pedFile = tempPath;
                    break;
                }
            }
        }
    }

    Debug.WriteLine($"[World] mapKey={mapKey} carFile={(carFile ?? "null")} pedFile={(pedFile ?? "null")}");

    if (carFile != null && File.Exists(carFile))
        BootstrapNpcsFromFile(carFile, isPedestrian: false);

    if (pedFile != null && File.Exists(pedFile))
        BootstrapNpcsFromFile(pedFile, isPedestrian: true);

    try { npcManager.Start(); } catch { }
        }

        private static string ExtractMapKey(string resourceName)
        {
            
            var parts = resourceName.Split('.');
            return (parts.Length >= 3) ? parts[^2] : "default";
        }

        private static string FirstExisting(string dir, IEnumerable<string> names)
            => names.Select(n => Path.Combine(dir, n)).FirstOrDefault(File.Exists);


        private static string ResolveSpriteOrFallback(string desired, string fallback)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "Assets", "WorldObjects");
            var path = Path.Combine(dir, desired);

            if (File.Exists(path)) return desired;

           
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resName = $"AutomatedCar.Assets.WorldObjects.{desired}";
                using var s = asm.GetManifestResourceStream(resName);
                if (s != null)
                {
                    Directory.CreateDirectory(dir);
                    using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                    s.CopyTo(fs);
                    Debug.WriteLine($"[ResolveSpriteOrFallback] extracted embedded resource {resName} -> {path}");
                    return desired;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ResolveSpriteOrFallback] extraction failed: {ex.Message}");
            }

         
            return fallback;
        }

        private void BootstrapNpcsFromFile(string filePath, bool isPedestrian)
        {
            var (pts, repeat) = NpcNs.NpcPathLoader.LoadFromJson(filePath);
            Debug.WriteLine($"[BootstrapNpcsFromFile] file={filePath} pts={(pts?.Count ?? 0)} repeat={repeat}");
            if (pts == null || pts.Count == 0) return;

            int startIndex = 0;
            var cw = this.WorldObjects.FirstOrDefault(o => o.WorldObjectType == WorldObjectType.Crosswalk);
            if (cw != null)
            {
                int best = 0; double bestD = double.MaxValue;
                for (int i = 0; i < pts.Count; i++)
                {
                    var d = Math.Sqrt(Math.Pow(pts[i].X - cw.X, 2) + Math.Pow(pts[i].Y - cw.Y, 2));
                    if (d < bestD) { bestD = d; best = i; }
                }
                startIndex = best;
            }

            try
            {
                var fileKey = Path.GetFileNameWithoutExtension(filePath)?.ToLowerInvariant() ?? "";
                if (!isPedestrian && fileKey.Contains("oval"))
                {
                    // For oval maps we want the NPC to start at the first point in the path file.
                    repeat = true;
                    startIndex = 0;
                }
            }
            catch
            {
               
            }

           
            bool IsNearCollideable(NpcNs.NPCPath p, int threshold = 100)
            {
                return this.WorldObjects
                           .Where(o => o.Collideable)
                           .Any(o => Math.Sqrt(Math.Pow(o.X - p.X, 2) + Math.Pow(o.Y - p.Y, 2)) < threshold);
            }

            
            int chosen = startIndex;
            for (int offset = 0; offset < pts.Count; offset++)
            {
                int idx = (startIndex + offset) % pts.Count;
                if (!IsNearCollideable(pts[idx]))
                {
                    chosen = idx;
                    break;
                }
            }
            startIndex = chosen;
            Debug.WriteLine($"[BootstrapNpcsFromFile] chosen startIndex={startIndex} point=({pts[startIndex].X},{pts[startIndex].Y})");

            if (isPedestrian)
            {
                var ped = new NpcNs.Pedestrian(pts[startIndex].X, pts[startIndex].Y, "man.png");
                ped.ZIndex = 10;
                ped.Load(pts[startIndex].Speed, repeating: repeat, currentPoint: startIndex, points: pts);
                this.npcManager.Add(ped);
                this.AddObject(ped);
                return;
            }

          
            var preferred = "car_3_black.png";
            var fallback = "car_2_blue.png";
            var resolved = ResolveSpriteOrFallback(preferred, fallback);

            var fileKeyCheck = Path.GetFileNameWithoutExtension(filePath)?.ToLowerInvariant() ?? "";
            string sprite = resolved;
            if (fileKeyCheck.Contains("oval"))
            {
                var preferredPath = Path.Combine(AppContext.BaseDirectory, "Assets", "WorldObjects", preferred);
                sprite = File.Exists(preferredPath) ? preferred : resolved;
            }

            Debug.WriteLine($"[BootstrapNpcsFromFile] selected sprite={sprite} repeat={repeat}");

            var car = new NpcNs.NPCCar(pts[startIndex].X, pts[startIndex].Y, sprite);
            car.ZIndex = 10;

            
            int next = startIndex + 1;
            if (next >= pts.Count) next = repeat ? 0 : startIndex;

      
            if (pts.Count > 1 && next != startIndex)
            {
                double dx = pts[next].X - pts[startIndex].X;
                double dy = pts[next].Y - pts[startIndex].Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist > 1e-6)
                {
                    double ux = dx / dist;
                    double uy = dy / dist;
                    const int nudgePx = 220; 
                    car.X += (int)Math.Round(ux * nudgePx);
                    car.Y += (int)Math.Round(uy * nudgePx);

                
                    car.Rotation = pts[next].Rotation;
                }
            }

            car.Load(pts[startIndex].Speed, repeating: repeat, currentPoint: startIndex, points: pts);
            this.npcManager.Add(car);
            this.AddObject(car);
            Debug.WriteLine($"[BootstrapNpcsFromFile] spawned car at ({car.X},{car.Y}) filename={sprite}");
        }

       

        private List<System.Drawing.PointF> ToDotNetPoints(IList<Avalonia.Point> points)
        {
            var result = new List<System.Drawing.PointF>();
            foreach (var p in points) result.Add(new PointF(Convert.ToSingle(p.X), Convert.ToSingle(p.Y)));
            return result;
        }

        private List<System.Drawing.PointF> ToDotNetPoints(IList<Avalonia.Point> points, int x, int y)
        {
            var result = new List<System.Drawing.PointF>();
            foreach (var p in points) result.Add(new PointF(Convert.ToSingle(p.X) + x, Convert.ToSingle(p.Y) + y));
            return result;
        }

        private Avalonia.Points ToAvaloniaPoints(IEnumerable<SKPoint> points)
        {
            var result = new Avalonia.Points();
            foreach (var p in points) result.Add(new Avalonia.Point(p.X, p.Y));
            return result;
        }

        private Dictionary<string, (int x, int y)> ReadRotationsPoints(string filename = "reference_points.json")
        {
            using var reader = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.{filename}"));
            var rotationPoints = JsonConvert.DeserializeObject<List<RotationPoint>>(reader.ReadToEnd());
            Dictionary<string, (int x, int y)> result = new();
            foreach (RotationPoint rp in rotationPoints) result.Add(rp.Type, (rp.X, rp.Y));
            return result;
        }

        private Dictionary<string, List<PolylineGeometry>> ReadPolygonJSON(string filename = "worldobject_polygons.json")
        {
            using var reader = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.{filename}"));
            var objects = JsonConvert.DeserializeObject<Dictionary<string, List<RawWorldObjectPolygon>>>(reader.ReadToEnd())["objects"];
            var result = new Dictionary<string, List<PolylineGeometry>>();
            foreach (RawWorldObjectPolygon rwop in objects)
            {
                var polygonList = new List<PolylineGeometry>();
                foreach (RawPolygon rp in rwop.Polys)
                {
                    var points = new Avalonia.Points();
                    foreach (var p in rp.Points) points.Add(new Avalonia.Point(p[0], p[1]));
                    polygonList.Add(new PolylineGeometry(points, false));
                }
                result.Add(rwop.Type, polygonList);
            }
            return result;
        }

        private Dictionary<string, string> CalculateRenderTransformOrigins(string filename = "reference_points.json")
        {
            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
            using var reader = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.{filename}"));
            var rotationPoints = JsonConvert.DeserializeObject<List<RotationPoint>>(reader.ReadToEnd());
            var result = new Dictionary<string, string>();
            foreach (RotationPoint rp in rotationPoints)
            {
                SKBitmap im = SKBitmap.Decode(Assembly.GetExecutingAssembly().GetManifestResourceStream($"AutomatedCar.Assets.WorldObjects.{rp.Type}.png"));
                
                var x = rp.Y / (float)im.Width * 100.0;
                var y = rp.X / (float)im.Height * 100.0;
                result.Add(rp.Type, x.ToString("0.00", nfi) + "%," + y.ToString("0.00", nfi) + "%");
            }
            return result;
        }

        private double RotationMatrixToDegree(float m11, float m12)
        {
            var result = Math.Acos(m11) * (180.0 / Math.PI);
            if (m12 < 0) result = 360 - result;
            return result;
        }

        private int DetermineZIndex(string type)
        {
            int result = 1;
            if (type == "crosswalk") result = 5;
            if (type == "tree") result = 20;
            return result;
        }

        private bool DetermineCollidablity(string type)
        {
            List<string> collideables = new List<string> {
                "boundary", "garage", "parking_bollard",
                "roadsign_parking_right", "roadsign_priority_stop",
                "roadsign_speed_40", "roadsign_speed_50", "roadsign_speed_60", "tree"
            };
            return collideables.Contains(type);
        }

        private WorldObjectType DetermineType(string type)
        {
            switch (type)
            {
                case "boundary": return WorldObjectType.Boundary;
                case "garage": return WorldObjectType.Building;
                case string s when s.StartsWith("car_"): return WorldObjectType.Car;
                case "crosswalk": return WorldObjectType.Crosswalk;
                case string s when s.StartsWith("parking_space_"): return WorldObjectType.ParkingSpace;
                case string s when s.StartsWith("road_"): return WorldObjectType.Road;
                case string s when s.StartsWith("roadsign_"): return WorldObjectType.RoadSign;
                case "tree": return WorldObjectType.Tree;
                default: return WorldObjectType.Other;
            }
        }
        
        public SKPath AddGeometry()
        {
            List<SKPoint> points = new();
            points.Add(new SKPoint(50, 50));
            points.Add(new SKPoint(50, 100));
            points.Add(new SKPoint(100, 50));
            points.Add(new SKPoint(50, 50));

            var path = new SKPath();
            path.AddPoly(points.Select(x => new SKPoint(x.X, x.Y)).ToArray());
            path.Close();

            return path;
        }
    }
}
