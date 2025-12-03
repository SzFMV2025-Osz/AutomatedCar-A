namespace AutomatedCar.SystemComponents;
using AutomatedCar.SystemComponents.Packets;
using Avalonia;
using Avalonia.Media;
using Helpers;
using Models;
using Packets.Input_Packets;
using System;
using System.Collections.Generic;
using System.Linq;

public class LKA : SystemComponent
{
    
    private readonly LKAPacket lkaPacket;
    private InputDevicePacket inputpacket;
    private bool wasPressed = false;
    private double xd;
    
    public LKA(VirtualFunctionBus virtualFunctionBus)
        : base(virtualFunctionBus)
    {
        this.lkaPacket = new LKAPacket();
        xd = 0.0;
        this.inputpacket = new InputDevicePacket();
        this.virtualFunctionBus.RegisterComponent(this);
        this.virtualFunctionBus.LKAPacket = this.lkaPacket;
    }
    
    public override void Process()
    {
        bool isPressedNow = this.virtualFunctionBus.KeyboardHandlerPacket.LKAKey;
        if (isPressedNow && !this.wasPressed)
        {
            this.lkaPacket.IsActive = !this.lkaPacket.IsActive;
        }
        this.wasPressed = isPressedNow;
        this.virtualFunctionBus.LKAPacket = this.lkaPacket;
        double wheelPosition = this.virtualFunctionBus.KeyboardHandlerPacket.WheelPercentage ?? 0;
        if (wheelPosition != 0)
        {
            this.lkaPacket.IsActive = false;
        }
        this.virtualFunctionBus.LKAPacket = this.lkaPacket;
        try
        {
            
            WorldObject? road = this.virtualFunctionBus
                .CameraPacket.RelevantObjects.FirstOrDefault(x => x.WorldObjectType == WorldObjectType.Road);
            
            
            

            var rot = World.Instance.ControlledCar.Rotation < 0 ? World.Instance.ControlledCar.Rotation + 360 : World.Instance.ControlledCar.Rotation;
            var rotation = new RotateTransform
            {
                CenterX = road.RotationPoint.X,
                CenterY = road.RotationPoint.Y,
                Angle = road.Rotation,
            };
            var translation = new TranslateTransform
            {
                X = road.X,
                Y = road.Y,
            };
            
            List<List<Point>> lanes = GeometryHelper.GetRoadLanePoints(road);
            
            lanes = lanes.Select(x=> x.Select(y=>y.Transform(rotation.Value).Transform(translation.Value)).ToList()).ToList();
     
            var currentlane = lanes.OrderBy(x => x.Min(y =>
                    Point.Distance(y, new Point(World.Instance.ControlledCar.X, World.Instance.ControlledCar.Y))))
                .ToList()[0];
            
            var car = World.Instance.ControlledCar;
        
            Point nextpoint;
            
            if (rot<315 || rot <= 45)
            {
                nextpoint = currentlane.Where(x=>x.Y<car.Y).OrderBy(x =>
                    Point.Distance(x, new Point(World.Instance.ControlledCar.X, World.Instance.ControlledCar.Y))).ToList()[0];
            }
            else if ( rot>45 && rot <= 135)
            {
                nextpoint = currentlane.Where(x=>x.X>car.X).OrderBy(x =>
                    Point.Distance(x, new Point(World.Instance.ControlledCar.X, World.Instance.ControlledCar.Y))).ToList()[0];
            }
            else if (rot>135 && rot <= 225)
            {
                nextpoint = currentlane.Where(x=>x.Y>car.Y).OrderBy(x =>
                    Point.Distance(x, new Point(World.Instance.ControlledCar.X, World.Instance.ControlledCar.Y))).ToList()[0];
            }
            else
            {
                nextpoint = currentlane.Where(x=>x.X<car.X).OrderBy(x =>
                    Point.Distance(x, new Point(World.Instance.ControlledCar.X, World.Instance.ControlledCar.Y))).ToList()[0];
            }

            lkaPacket.IsReady = true;
        }
        catch (Exception e)
        {
            this.lkaPacket.IsActive = false;
            this.lkaPacket.IsReady = false;
        }
        
    }
}
    