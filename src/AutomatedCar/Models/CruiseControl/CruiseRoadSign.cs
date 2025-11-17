namespace AutomatedCar.Models.CruiseControl;

using Helpers;
using System.Linq;
using SystemComponents.Packets;

public class CruiseRoadSign
{
    private ICruiseControl tempomat;

    public void Update(CameraPacket packet)
    {
        var car = World.Instance.ControlledCar;
        var roadSigns = packet.RelevantObjects
            .Where(o => o.WorldObjectType == WorldObjectType.RoadSign)
            .OrderBy(o => GeometryHelper.DistanceBetweenObjects(o, car))
            .ToList();
        if (roadSigns.Count == 0)
        {
            return;
        }

        this.tempomat.DesiredSpeed = roadSigns[0].SpeedLimit ?? car.Speed;
    }
}
