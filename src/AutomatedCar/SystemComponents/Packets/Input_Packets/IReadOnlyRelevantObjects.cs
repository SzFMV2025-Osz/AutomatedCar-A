namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class IReadOnlyRelevantObjects
    {
        int LimitSpeed { get; }
        List<RelevantObject> RelevantObjects { get; set; }
    }
}
