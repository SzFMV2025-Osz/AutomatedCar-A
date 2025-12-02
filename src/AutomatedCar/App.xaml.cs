namespace AutomatedCar
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomatedCar.Models;
    using AutomatedCar.ViewModels;
    using AutomatedCar.Views;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;
    using System;

    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var world = this.CreateWorld();
                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel(world) };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public World CreateWorld()
        {
            var world = World.Instance;

            string[] args = Environment.GetCommandLineArgs();
            string key = args.Length > 1 ? args[1] : "oval";
            string resourceName = $"AutomatedCar.Assets.{key}.json";

            var asm = Assembly.GetExecutingAssembly();
            if (asm.GetManifestResourceStream(resourceName) == null)
            {
                resourceName = "AutomatedCar.Assets.test_world.json";
            }

            world.PopulateFromJSON(resourceName);

            this.AddControlledCarsTo(world);

            // --- DEBUG: list world objects in Output so you can confirm the NPC is present ---
            System.Diagnostics.Debug.WriteLine($"[CreateWorld] Loaded map: {resourceName}  WorldObjects: {world.WorldObjects.Count}");
            foreach (var o in world.WorldObjects)
                System.Diagnostics.Debug.WriteLine($"[CreateWorld] OBJ: {o.WorldObjectType} - {o.Filename} @({o.X},{o.Y})");

            // If oval map, try to place the controlled car near the black NPC so it's visible
            if (key.Equals("oval", StringComparison.OrdinalIgnoreCase))
            {
                var npcBlack = world.WorldObjects
                    .FirstOrDefault(w => w.WorldObjectType == WorldObjectType.Car &&
                                         (w.Filename?.IndexOf("car_3_black", StringComparison.OrdinalIgnoreCase) ?? -1) >= 0);
                if (npcBlack != null && world.controlledCars.Count > 0)
                {
                    // move the first controlled car close to the NPC (offset so they don't overlap)
                    var cc = world.controlledCars[0];
                    cc.X = npcBlack.X + 200;
                    cc.Y = npcBlack.Y + 200;
                    System.Diagnostics.Debug.WriteLine($"[CreateWorld] Moved controlled car to ({cc.X},{cc.Y}) to view NPC at ({npcBlack.X},{npcBlack.Y})");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[CreateWorld] car_3_black not found in WorldObjects");
                }
            }

            return world;
        }

        private PolylineGeometry GetControlledCarBoundaryBox()
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
    .GetManifestResourceStream($"AutomatedCar.Assets.worldobject_polygons.json"));
            string json_text = reader.ReadToEnd();
            dynamic stuff = JObject.Parse(json_text);
            var points = new List<Point>();
            foreach (var i in stuff["objects"][0]["polys"][0]["points"])
            {
                points.Add(new Point(i[0].ToObject<int>(), i[1].ToObject<int>()));
            }

            return new PolylineGeometry(points, false);
        }

        private void AddDummyCircleTo(World world)
        {
            var circle = new Circle(200, 200, "circle.png", 20);
            
            circle.Width = 40;
            circle.Height = 40;
            circle.ZIndex = 20;
            circle.Rotation = 45;

            world.AddObject(circle);
        }

        private AutomatedCar CreateControlledCar(int x, int y, int rotation, string filename)
        {
            var controlledCar = new Models.AutomatedCar(x, y, filename);
            
            controlledCar.Geometry = this.GetControlledCarBoundaryBox();
            controlledCar.RawGeometries.Add(controlledCar.Geometry);
            controlledCar.Geometries.Add(controlledCar.Geometry);
            controlledCar.RotationPoint = new System.Drawing.Point(54, 120);
            controlledCar.Rotation = rotation;

            controlledCar.Start();

            return controlledCar;
        }

        private void AddControlledCarsTo(World world)
        {
            var controlledCar = this.CreateControlledCar(480, 1425, 0, "car_1_white.png");
            var controlledCar2 = this.CreateControlledCar(4250, 1420, -90, "car_1_red.png");

            world.AddControlledCar(controlledCar);
            world.AddControlledCar(controlledCar2);
        }
    }
}